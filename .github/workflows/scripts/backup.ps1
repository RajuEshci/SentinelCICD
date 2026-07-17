param (
    [Parameter(Mandatory = $true)]
    [string]$SitePath,

    [Parameter(Mandatory = $true)]
    [string]$AppPool,

    [Parameter(Mandatory = $true)]
    [string]$DbServer,

    [Parameter(Mandatory = $true)]
    [string]$DbName,

    [Parameter(Mandatory = $true)]
    [string]$DbUser,

    [Parameter(Mandatory = $true)]
    [string]$DbPassword,

    # Extra folder outside SitePath that should also be backed up (e.g. shared
    # API files/config not part of the IIS site content).
    [string]$ApiFilesPath,

    # NOTE: This path is used by the SQL Server *service account*, not by this
    # script/runner. If SQL Server runs on a different machine than IIS/the
    # runner, this must be a UNC path (e.g. \\fileserver\sql-backups) that the
    # SQL Server service account can write to. If SQL Server is on the same
    # box as IIS, a local path works fine.
    [string]$DbBackupDir
)

$ErrorActionPreference = "Stop"

Import-Module WebAdministration

# =========================================================================
# Generate timestamp for all backups
# =========================================================================
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
Write-Host "Backup timestamp: $timestamp"

# =========================================================================
# STOP Application Pool
# =========================================================================
Write-Host "Checking App Pool: $AppPool"
$appPoolState = Get-WebAppPoolState -Name $AppPool -ErrorAction SilentlyContinue

if ($appPoolState -and $appPoolState.Value -eq "Started") {
    Write-Host "Stopping App Pool: $AppPool"
    Stop-WebAppPool -Name $AppPool
    Start-Sleep -Seconds 5
    Write-Host "App Pool stopped successfully."
} else {
    if ($appPoolState) {
        Write-Host "App Pool is already in state: $($appPoolState.Value)"
    } else {
        Write-Host "App Pool not found or already stopped."
    }
}

# =========================================================================
# MAIN SITE BACKUP
# =========================================================================
Write-Host "=== Starting Main Site Backup ==="
Write-Host "Site Path: $SitePath"

# Create backup directory if it doesn't exist
$backupRoot = Split-Path $SitePath -Parent
$backupDir = "$backupRoot\backups"
if (-not (Test-Path $backupDir)) {
    Write-Host "Creating backup directory: $backupDir"
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
}

$backupZip = "$backupDir\backup_$timestamp.zip"
$tempCopy  = "$backupDir\_backup_temp"

if (Test-Path $tempCopy) {
    Remove-Item $tempCopy -Recurse -Force
}

Write-Host "Creating temp backup copy of site files..."
robocopy $SitePath $tempCopy /E /R:1 /W:1 /XF *.log /NFL /NDL | Out-Null

Write-Host "Creating zip archive..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempCopy, $backupZip)

Remove-Item $tempCopy -Recurse -Force

Write-Host "Main site backup SUCCESS: $backupZip"
$fileSize = (Get-Item $backupZip).Length / 1MB
Write-Host "Backup size: $([math]::Round($fileSize, 2)) MB"

# =========================================================================
# API FILES BACKUP (if ApiFilesPath is provided)
# =========================================================================
if ($ApiFilesPath) {
    Write-Host "=== Starting API Files Backup ==="
    Write-Host "API Files Path: $ApiFilesPath"
    
    if (-not (Test-Path $ApiFilesPath)) {
        throw "ApiFilesPath not found: $ApiFilesPath"
    }

    $apiFilesBackupZip = "$backupDir\backup_apifiles_$timestamp.zip"
    $apiFilesTempCopy = "$backupDir\_backup_apifiles_temp"

    if (Test-Path $apiFilesTempCopy) {
        Remove-Item $apiFilesTempCopy -Recurse -Force
    }

    Write-Host "Creating temp copy of API files..."
    robocopy $ApiFilesPath $apiFilesTempCopy /E /R:1 /W:1 /XF *.log /NFL /NDL | Out-Null

    Write-Host "Creating API files zip archive..."
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($apiFilesTempCopy, $apiFilesBackupZip)

    Remove-Item $apiFilesTempCopy -Recurse -Force

    Write-Host "API files backup SUCCESS: $apiFilesBackupZip"
    $fileSize = (Get-Item $apiFilesBackupZip).Length / 1MB
    Write-Host "API backup size: $([math]::Round($fileSize, 2)) MB"
} else {
    Write-Host "ApiFilesPath not provided, skipping API files backup."
}

# =========================================================================
# DATABASE BACKUP
# =========================================================================
if (-not $DbBackupDir) {
    $DbBackupDir = "$backupDir\db_backups"
}

Write-Host "=== Starting Database Backup ==="
Write-Host "Database: $DbName on $DbServer"
Write-Host "Database Backup Directory: $DbBackupDir"

if (-not (Test-Path $DbBackupDir)) {
    Write-Host "Creating DB backup directory: $DbBackupDir"
    New-Item -ItemType Directory -Path $DbBackupDir -Force | Out-Null
    
    # Grant permissions to Everyone (since SQL Auth is used)
    try {
        Write-Host "Setting permissions on backup directory..."
        icacls $DbBackupDir /grant "Everyone:(OI)(CI)F" /T 2>$null
    } catch {
        Write-Host "Warning: Could not set permissions. Ensure SQL Server can write to the directory."
    }
}

# Use the same timestamp as the file backup
$dbBackupFile = Join-Path $DbBackupDir "${DbName}_$timestamp.bak"

# Using SQL Authentication
Write-Host "Using SQL Server Authentication with user: $DbUser"
$escapedPassword = $DbPassword -replace "'", "''"

# Check SQL Server edition to determine if compression is supported
Write-Host "Checking SQL Server edition..."
$editionQuery = "SELECT SERVERPROPERTY('Edition') as Edition"

$editionResult = sqlcmd -S $DbServer -U $DbUser -P $escapedPassword -C -Q "$editionQuery" -W -h-1 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Warning: Could not determine SQL Server edition. Assuming Standard/Enterprise."
    $edition = $false
} else {
    $edition = $editionResult | Select-String -Pattern "Express" -Quiet
}

if ($edition) {
    Write-Host "SQL Server Express Edition detected - compression not supported"
    $sqlQuery = "BACKUP DATABASE [$DbName] TO DISK = N'$dbBackupFile' WITH INIT, STATS = 10;"
} else {
    Write-Host "SQL Server Standard/Enterprise Edition - compression supported"
    $sqlQuery = "BACKUP DATABASE [$DbName] TO DISK = N'$dbBackupFile' WITH INIT, COMPRESSION, STATS = 10;"
}

# Execute the backup
Write-Host "Running: sqlcmd -S $DbServer -U $DbUser -P *** -C -Q `"$sqlQuery`""
$sqlcmdOutput = sqlcmd -S $DbServer -U $DbUser -P $escapedPassword -C -Q "$sqlQuery" 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host "SQLCMD Error Output:"
    Write-Host $sqlcmdOutput
    throw "Database backup FAILED (sqlcmd exit code $LASTEXITCODE)."
}

# Verify backup file exists
if (-not (Test-Path $dbBackupFile)) {
    throw "Database backup command succeeded but backup file was not found at '$dbBackupFile'."
}

$fileSize = (Get-Item $dbBackupFile).Length / 1MB
Write-Host "Database backup SUCCESS: $dbBackupFile ($([math]::Round($fileSize, 2)) MB)"

# =========================================================================
# SUMMARY
# =========================================================================
Write-Host ""
Write-Host "=== BACKUP COMPLETED SUCCESSFULLY ==="
Write-Host "Main site backup: $backupZip"
if ($ApiFilesPath) {
    Write-Host "API files backup: $apiFilesBackupZip"
}
Write-Host "Database backup: $dbBackupFile"
Write-Host "All backups are in: $backupDir"
Write-Host ""
Write-Host "NOTE: App Pool is stopped. The Deploy step will start it after publishing."

# Starting the app pool back up is intentionally left to the Deploy step,
# which starts it after publishing new files (matches existing pipeline flow).