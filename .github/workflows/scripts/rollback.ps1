param (
    [Parameter(Mandatory = $true)]
    [string]$SitePath,

    [Parameter(Mandatory = $true)]
    [string]$AppPool,

    [Parameter(Mandatory = $true)]
    [string]$DbServer,

    [Parameter(Mandatory = $true)]
    [string]$DbName,

    # SQL Server Authentication (REQUIRED for SQL Express)
    [Parameter(Mandatory = $true)]
    [string]$DbUser,
    
    [Parameter(Mandatory = $true)]
    [string]$DbPassword,

    # Must match the DbBackupDir used in backup.ps1 (same UNC/local path rules apply).
    [string]$DbBackupDir,

    # Must match the ApiFilesPath used in backup.ps1 (the destination folder to restore into).
    [string]$ApiFilesPath
    
)

$ErrorActionPreference = "Stop"

# Import IIS module
Import-Module WebAdministration -ErrorAction Stop

Write-Host "=== Starting Rollback Procedure ==="
Write-Host "Target Site Path: $SitePath"
Write-Host "App Pool: $AppPool"

# =========================================================================
# STOP IIS Application Pool
# =========================================================================
Write-Host "=== Stopping Application Pool ==="
$appPoolState = Get-WebAppPoolState -Name $AppPool -ErrorAction SilentlyContinue

if ($appPoolState -and $appPoolState.Value -eq "Started") {
    Write-Host "Stopping App Pool: $AppPool"
    Stop-WebAppPool -Name $AppPool
    Start-Sleep -Seconds 5
    
    # Verify it stopped
    $newState = Get-WebAppPoolState -Name $AppPool
    if ($newState.Value -eq "Stopped") {
        Write-Host "App Pool stopped successfully."
    } else {
        Write-Host "WARNING: App Pool is in state: $($newState.Value)"
    }
} else {
    if ($appPoolState) {
        Write-Host "App Pool is already in state: $($appPoolState.Value)"
    } else {
        Write-Host "App Pool not found."
    }
}

# =========================================================================
# STOP IIS Website (if you have a specific site)
# =========================================================================
# If you know the website name, uncomment and modify:
# $websiteName = "Default Web Site"
# Write-Host "Stopping website: $websiteName"
# Stop-WebSite -Name $websiteName
# Start-Sleep -Seconds 3

# Validate SitePath exists
if (-not (Test-Path $SitePath)) {
    Write-Host "WARNING: Site path does not exist: $SitePath"
    Write-Host "Creating directory..."
    New-Item -ItemType Directory -Path $SitePath -Force | Out-Null
}

# Find latest backup
$backupDir = $ApiFilesPath 
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

# Extract backup (preserving config files)
Write-Host "Extracting backup (preserving config files)..."

try {
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    
    # Extract all files except appsettings.json and web.config
    $zip = [System.IO.Compression.ZipFile]::OpenRead($latestBackup.FullName)
    try {
        $extractedCount = 0
        foreach ($entry in $zip.Entries) {
            $entryName = $entry.FullName
            $fileName = Split-Path $entryName -Leaf
            
            # Skip config files
            if ($fileName -eq "appsettings.json" -or $fileName -eq "web.config") {
                Write-Host "Skipping: $entryName (preserved from existing deployment)"
                continue
            }
            
            $targetPath = Join-Path $SitePath $entryName
            $targetDir = Split-Path $targetPath -Parent
            
            if (-not (Test-Path $targetDir)) {
                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
            }
            
            # Extract the file
            [System.IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $targetPath, $true)
            $extractedCount++
        }
        Write-Host "Extracted $extractedCount files (config files preserved)."
    } finally {
        $zip.Dispose()
    }

    # Verify extraction
    $extractedFiles = Get-ChildItem $SitePath -Recurse -File -ErrorAction SilentlyContinue
    if ($extractedFiles -ne $null) {
        $extractedCount = $extractedFiles.Count
        Write-Host "Total files in site after extraction: $extractedCount"
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
Write-Host "Configuration files (appsettings.json, web.config) preserved from current deployment."

# # =========================================================================
# # API files restore (separate folder outside SitePath)
# # =========================================================================
# if ($ApiFilesPath) {
#     Write-Host "=== Starting API Files Rollback ==="

#     $apiFilesBackups = Get-ChildItem -Path $backupDir -Filter "backup_apifiles_*.zip" -ErrorAction SilentlyContinue

#     if ($apiFilesBackups -eq $null -or $apiFilesBackups.Count -eq 0) {
#         throw "No API files backup found matching pattern: backup_apifiles_*.zip in $backupDir"
#     }

#     $latestApiFilesBackup = $apiFilesBackups | Sort-Object LastWriteTime -Descending | Select-Object -First 1
#     Write-Host "Latest API files backup selected: $($latestApiFilesBackup.Name)"

#     if (-not (Test-Path $ApiFilesPath)) {
#         Write-Host "ApiFilesPath does not exist, creating: $ApiFilesPath"
#         New-Item -ItemType Directory -Path $ApiFilesPath -Force | Out-Null
#     } else {
#         Write-Host "Clearing API files directory: $ApiFilesPath"
#         Get-ChildItem -Path $ApiFilesPath -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
#     }

#     Write-Host "Extracting API files backup..."
#     Add-Type -AssemblyName System.IO.Compression.FileSystem
#     [System.IO.Compression.ZipFile]::ExtractToDirectory($latestApiFilesBackup.FullName, $ApiFilesPath)

#     Write-Host "API files restore SUCCESS: restored from $($latestApiFilesBackup.Name) to $ApiFilesPath"
# } else {
#     Write-Host "ApiFilesPath not provided, skipping API files restore."
# }

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

# Escape password for SQL
$escapedPassword = $DbPassword -replace "'", "''"

$restoreSql = @"
ALTER DATABASE [$DbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [$DbName] FROM DISK = N'$($latestDbBackup.FullName)' WITH REPLACE, RECOVERY;
ALTER DATABASE [$DbName] SET MULTI_USER;
"@

Write-Host "Restoring database [$DbName] on $DbServer from $($latestDbBackup.Name)..."
Write-Host "Running: sqlcmd -S $DbServer -U $DbUser -P *** -C -Q `"$restoreSql`""

$restoreOutput = sqlcmd -S $DbServer -U $DbUser -P $escapedPassword -C -Q $restoreSql 2>&1

if ($LASTEXITCODE -ne 0) {
    # Try to make sure the DB isn't left stuck in SINGLE_USER mode after a failed restore
    Write-Host "ERROR: Database restore failed (sqlcmd exit code $LASTEXITCODE)."
    Write-Host "Error Output: $restoreOutput"
    Write-Host "Attempting to set database back to MULTI_USER..."
    sqlcmd -S $DbServer -U $DbUser -P $escapedPassword -C -Q "ALTER DATABASE [$DbName] SET MULTI_USER;" | Out-Null
    throw "Database rollback failed. See sqlcmd output above."
}

Write-Host "Database restore SUCCESS: $DbName restored from $($latestDbBackup.Name)"

# =========================================================================
# START IIS Application Pool
# =========================================================================
Write-Host "=== Starting Application Pool ==="
$appPoolState = Get-WebAppPoolState -Name $AppPool -ErrorAction SilentlyContinue

if ($appPoolState -and $appPoolState.Value -eq "Stopped") {
    Write-Host "Starting App Pool: $AppPool"
    Start-WebAppPool -Name $AppPool
    Start-Sleep -Seconds 5
    
    # Verify it started
    $newState = Get-WebAppPoolState -Name $AppPool
    if ($newState.Value -eq "Started") {
        Write-Host "App Pool started successfully."
    } else {
        Write-Host "WARNING: App Pool is in state: $($newState.Value)"
    }
} else {
    if ($appPoolState) {
        Write-Host "App Pool is already in state: $($appPoolState.Value)"
    } else {
        Write-Host "App Pool not found."
    }
}

# =========================================================================
# START IIS Website (if you stopped it earlier)
# =========================================================================
# If you know the website name, uncomment and modify:
# $websiteName = "Default Web Site"
# Write-Host "Starting website: $websiteName"
# Start-WebSite -Name $websiteName
# Start-Sleep -Seconds 3

Write-Host "=== Rollback COMPLETED Successfully (files + database) ==="