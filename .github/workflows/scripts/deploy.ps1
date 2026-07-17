param (
  [string]$SitePath,
  [string]$AppPool
)

Import-Module WebAdministration

Write-Host "Stopping App Pool..."
Stop-WebAppPool -Name $AppPool
Start-Sleep -Seconds 3
$poolStatus = Get-WebAppPoolState -Name $AppPool
Write-Host "App Pool status after stop: $($poolStatus.Value)"

Write-Host "Deploying files..."
Copy-Item ".\publish\*" $SitePath -Recurse -Force
Write-Host "Files deployed successfully"

Write-Host "Starting App Pool..."
Start-WebAppPool -Name $AppPool
Start-Sleep -Seconds 2

$poolStatus = Get-WebAppPoolState -Name $AppPool
Write-Host "App Pool status after start: $($poolStatus.Value)"

if ($poolStatus.Value -ne "Started") {
  Write-Host "ERROR: App Pool failed to start!"
  exit 1
}
