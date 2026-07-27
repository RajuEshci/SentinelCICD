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

Write-Host "Deploying files (preserving config files)..."

# Copy all files EXCEPT appsettings.json and web.config
$publishPath = ".\publish"
if (-not (Test-Path $publishPath)) {
    Write-Host "ERROR: Publish folder not found: $publishPath"
    exit 1
}

Get-ChildItem -Path $publishPath -Recurse -File | ForEach-Object {
    $relativePath = $_.FullName.Substring((Get-Item $publishPath).FullName.Length + 1)
    $destinationPath = Join-Path $SitePath $relativePath
    
    # Skip appsettings.json and web.config
    if ($_.Name -eq "appsettings.json" -or $_.Name -eq "web.config") {
        Write-Host "Skipping: $($_.Name) (preserved from existing deployment)"
        return
    }
    
    # Create destination directory if it doesn't exist
    $destDir = Split-Path $destinationPath -Parent
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }
    
    # Copy the file
    Copy-Item $_.FullName -Destination $destinationPath -Force
}

Write-Host "Files deployed successfully (config files preserved)"

Write-Host "Starting App Pool..."
Start-WebAppPool -Name $AppPool
Start-Sleep -Seconds 2

$poolStatus = Get-WebAppPoolState -Name $AppPool
Write-Host "App Pool status after start: $($poolStatus.Value)"

if ($poolStatus.Value -ne "Started") {
  Write-Host "ERROR: App Pool failed to start!"
  exit 1
}