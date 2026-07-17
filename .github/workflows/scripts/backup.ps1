param (
    [Parameter(Mandatory = $true)]
    [string]$SitePath,

    [Parameter(Mandatory = $true)]
    [string]$AppPool,

    [Parameter(Mandatory = $true)]
    [string]$DbServer,

    [Parameter(Mandatory = $true)]
    [string]$DbName,

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

# Check if App Pool exists and is running before stopping
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

$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$backupZip = "$SitePath\..\backup_$timestamp.zip"
$tempCopy  = "$SitePath\..\_backup_temp"

if (Test-Path $tempCopy) {
    Remove-Item $tempCopy -Recurse -Force
}

Write-Host "Creating temp backup copy..."
robocopy $SitePath $tempCopy /E /R:1 /W:1 /XF *.log /NFL /NDL | Out-Null

Write-Host "Creating zip archive..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempCopy, $backupZip)

Remove-Item $tempCopy -Recurse -Force

Write-Host "Backup SUCCESS: $backupZip"

# =========================================================================
# API files backup (separate folder outside SitePath)
# =========================================================================
if ($ApiFilesPath) {
    if (-not (Test-Path $ApiFilesPath)) {
        throw "ApiFilesPath not found: $ApiFilesPath"
    }

    Write-Host "Backing up API files folder: $ApiFilesPath"

    $apiFilesBackupZip = "$SitePath\..\backup_apifiles_$timestamp.zip"
    $apiFilesTempCopy   = "$SitePath\..\_backup_apifiles_temp"

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
} else {
    Write-Host "ApiFilesPath not provided, skipping API files backup."
}

# =========================================================================
# Database backup (SQL Server, native BACKUP DATABASE via sqlcmd)
# =========================================================================
if (-not $DbBackupDir) {
    $DbBackupDir = "$SitePath\..\db_backups"
}

Write-Host "Starting database backup for [$DbName] on $DbServer..."

if (-not (Test-Path $DbBackupDir)) {
    Write-Host "Creating DB backup directory: $DbBackupDir"
    New-Item -ItemType Directory -Path $DbBackupDir -Force | Out-Null
}

# Use the same timestamp as the file backup so file + DB backups can be
# paired up later if needed.
$dbBackupFile = Join-Path $DbBackupDir "${DbName}_$timestamp.bak"

$sqlQuery = "BACKUP DATABASE [$DbName] TO DISK = N'$dbBackupFile' WITH INIT, COMPRESSION, STATS = 10;"

Write-Host "Running: sqlcmd -S $DbServer -E -Q ""$sqlQuery"""
sqlcmd -S $DbServer -U $DbUser -P $DbPassword -C -Q "$sqlQuery"

if ($LASTEXITCODE -ne 0) {
    throw "Database backup FAILED (sqlcmd exit code $LASTEXITCODE). Check that the SQL Server service account can write to '$dbBackupFile'."
}

if (-not (Test-Path $dbBackupFile)) {
    throw "Database backup command succeeded but backup file was not found at '$dbBackupFile'. If SQL Server is on a different machine, DbBackupDir must be a UNC path reachable by the SQL Server service account."
}

Write-Host "Database backup SUCCESS: $dbBackupFile"

# Starting the app pool back up is intentionally left to the Deploy step,
# which starts it after publishing new files (matches existing pipeline flow).