param (
    [Parameter(Mandatory = $true)]
    [string]$SitePath,

    [Parameter(Mandatory = $true)]
    [string]$DbServer,

    [Parameter(Mandatory = $true)]
    [string]$DbName,

    # Must match the DbBackupDir used in backup.ps1 (same UNC/local path rules apply).
    [string]$DbBackupDir
)

$ErrorActionPreference = "Stop"

Write-Host "=== Starting Rollback Procedure ==="
Write-Host "Target Site Path: $SitePath"

# Validate SitePath exists
if (-not (Test-Path $SitePath)) {
    Write-Host "WARNING: Site path does not exist: $SitePath"
    Write-Host "Creating directory..."
    New-Item -ItemType Directory -Path $SitePath -Force | Out-Null
}

# Find latest backup
$backupDir = Split-Path $SitePath -Parent
Write-Host "Searching for backups in: $backupDir"

$backupFiles = Get-ChildItem -Path $backupDir -Filter "backup_*.zip" -ErrorAction SilentlyContinue

if ($backupFiles -eq $null -or $backupFiles.Count -eq 0) {
    Write-Host "ERROR: No backup files found matching pattern: backup_*.zip"
    Write-Host "Available files in directory:"
    Get-ChildItem -Path $backupDir | Select-Object Name, LastWriteTime, Length | Format-Table -AutoSize
    throw "No backup found for rollback"
}

$latestBackup = $backupFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "Latest backup selected: $($latestBackup.Name)"
Write-Host "Backup created: $($latestBackup.LastWriteTime)"
Write-Host "Backup size: $([math]::Round($latestBackup.Length / 1MB, 2)) MB"

# Backup current site before rollback (safety measure)
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$preRollbackBackup = "$backupDir\pre_rollback_$timestamp.zip"

$currentSiteFiles = Get-ChildItem $SitePath -Force -ErrorAction SilentlyContinue
if ($currentSiteFiles -ne $null -and $currentSiteFiles.Count -gt 0) {
    Write-Host "Creating pre-rollback backup of current site..."
    try {
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        [System.IO.Compression.ZipFile]::CreateFromDirectory($SitePath, $preRollbackBackup, [System.IO.Compression.CompressionLevel]::Fastest, $false)
        Write-Host "Pre-rollback backup created: $(Split-Path $preRollbackBackup -Leaf)"
    } catch {
        Write-Host "WARNING: Could not create pre-rollback backup: $($_.Exception.Message)"
    }
}

# Clear site directory with retry logic
Write-Host "Clearing site directory..."
$maxRetries = 3
$retryCount = 0
$cleaned = $false

while ($cleaned -eq $false -and $retryCount -lt $maxRetries) {
    try {
        # Remove only contents, not the directory itself
        Get-ChildItem -Path $SitePath -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction Stop
        $cleaned = $true
        Write-Host "Site directory cleared successfully."
    } catch {
        $retryCount++
        Write-Host "Attempt $retryCount failed: $($_.Exception.Message)"
        if ($retryCount -lt $maxRetries) {
            Write-Host "Retrying in 2 seconds..."
            Start-Sleep -Seconds 2
        } else {
            Write-Host "Failed to clear directory after $maxRetries attempts."
            Write-Host "Attempting alternative cleanup method..."

            # Try removing files individually
            Get-ChildItem -Path $SitePath -Force -ErrorAction SilentlyContinue | ForEach-Object {
                try {
                    Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
                } catch {
                    Write-Host "Could not remove: $($_.Name)"
                }
            }
        }
    }
}

# Extract backup
Write-Host "Extracting backup..."
try {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($latestBackup.FullName, $SitePath)
    Write-Host "Backup extracted successfully."

    # Verify extraction
    $extractedFiles = Get-ChildItem $SitePath -Recurse -File -ErrorAction SilentlyContinue
    if ($extractedFiles -ne $null) {
        $extractedCount = $extractedFiles.Count
        Write-Host "Extracted $extractedCount files."
    } else {
        Write-Host "No files found after extraction."
    }

} catch {
    # Attempt recovery if extraction failed
    Write-Host "ERROR: Failed to extract backup: $($_.Exception.Message)"

    if (Test-Path $preRollbackBackup) {
        Write-Host "Attempting to restore from pre-rollback backup..."
        try {
            Get-ChildItem -Path $SitePath -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
            [System.IO.Compression.ZipFile]::ExtractToDirectory($preRollbackBackup, $SitePath)
            Write-Host "Restored from pre-rollback backup."
        } catch {
            Write-Host "Failed to restore from pre-rollback backup."
        }
    }
    throw "Rollback failed: $($_.Exception.Message)"
}

Write-Host "=== File Rollback COMPLETED Successfully ==="
Write-Host "Site restored from: $($latestBackup.Name)"
Write-Host "Restored to: $SitePath"

# =========================================================================
# Database restore (SQL Server, native RESTORE DATABASE via sqlcmd)
# =========================================================================
if (-not $DbBackupDir) {
    $DbBackupDir = "$backupDir\db_backups"
}

Write-Host "=== Starting Database Rollback ==="
Write-Host "Searching for database backups in: $DbBackupDir"

if (-not (Test-Path $DbBackupDir)) {
    throw "Database backup directory not found: $DbBackupDir. Cannot restore database."
}

$dbBackupFiles = Get-ChildItem -Path $DbBackupDir -Filter "${DbName}_*.bak" -ErrorAction SilentlyContinue

if ($dbBackupFiles -eq $null -or $dbBackupFiles.Count -eq 0) {
    throw "No database backup files found matching pattern: ${DbName}_*.bak in $DbBackupDir"
}

$latestDbBackup = $dbBackupFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "Latest DB backup selected: $($latestDbBackup.Name)"
Write-Host "Backup created: $($latestDbBackup.LastWriteTime)"
Write-Host "Backup size: $([math]::Round($latestDbBackup.Length / 1MB, 2)) MB"

$restoreSql = @"
ALTER DATABASE [$DbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [$DbName] FROM DISK = N'$($latestDbBackup.FullName)' WITH REPLACE, RECOVERY;
ALTER DATABASE [$DbName] SET MULTI_USER;
"@

Write-Host "Restoring database [$DbName] on $DbServer from $($latestDbBackup.Name)..."
sqlcmd -S $DbServer -E -Q $restoreSql

if ($LASTEXITCODE -ne 0) {
    # Try to make sure the DB isn't left stuck in SINGLE_USER mode after a failed restore
    Write-Host "ERROR: Database restore failed (sqlcmd exit code $LASTEXITCODE). Attempting to set database back to MULTI_USER..."
    sqlcmd -S $DbServer -E -Q "ALTER DATABASE [$DbName] SET MULTI_USER;" | Out-Null
    throw "Database rollback failed. See sqlcmd output above."
}

Write-Host "Database restore SUCCESS: $DbName restored from $($latestDbBackup.Name)"
Write-Host "=== Rollback COMPLETED Successfully (files + database) ==="
