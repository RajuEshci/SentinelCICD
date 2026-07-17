param (
    [Parameter(Mandatory = $true)]
    [string]$SitePath,

    [Parameter(Mandatory = $true)]
    [string]$AppPool
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