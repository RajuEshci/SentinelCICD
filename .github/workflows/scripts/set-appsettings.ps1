param (
    [Parameter(Mandatory = $true)]
    [string]$AppSettingsPath,   # e.g. publish\appsettings.json

    [Parameter(Mandatory = $true)]
    [string]$DbServer,
    [Parameter(Mandatory = $true)]
    [string]$DbName,
    [Parameter(Mandatory = $true)]
    [string]$DbUser,
    [Parameter(Mandatory = $true)]
    [string]$DbPassword,
    [Parameter(Mandatory = $true)]
    [string]$JwtKey,
    [Parameter(Mandatory = $true)]
    [string]$GraphTenantId,
    [Parameter(Mandatory = $true)]
    [string]$GraphClientId,
    [Parameter(Mandatory = $true)]
    [string]$GraphClientSecret,
    [Parameter(Mandatory = $true)]
    [string]$GraphFromUser,
    [string]$SmtpUsername = "",
    [string]$SmtpPassword = "",
    [Parameter(Mandatory = $true)]
    [string]$EncryptionCryptKey,
    [Parameter(Mandatory = $true)]
    [string]$EncryptionInitVector,
    [Parameter(Mandatory = $true)]
    [string[]]$RequestAccessCodeMailId,
    [Parameter(Mandatory = $true)]
    [string]$ApiUrl
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $AppSettingsPath)) {
    throw "appsettings.json not found at: $AppSettingsPath"
}

Write-Host "Injecting configuration into: $AppSettingsPath (by key path, values not logged)"

# This overwrites known JSON keys directly by path, whatever their current
# value is (real local-dev default, blank, etc). The checked-in appsettings.json
# needs no special placeholder syntax and can be run as-is by developers.
$rawContent = Get-Content -Path $AppSettingsPath -Raw
$config = $rawContent | ConvertFrom-Json

$config.ConnectionStrings.DefaultConnection = "server=$DbServer;database=$DbName;user id=$DbUser;password=$DbPassword;Encrypt=True;TrustServerCertificate=True;"
$config.Jwt.Key = $JwtKey
$config.GraphEmail.TenantId = $GraphTenantId
$config.GraphEmail.ClientId = $GraphClientId
$config.GraphEmail.ClientSecret = $GraphClientSecret
$config.GraphEmail.FromUser = $GraphFromUser
$config.SMTP.UserName = $SmtpUsername
$config.SMTP.Password = $SmtpPassword
$config.EncryptionSettings.CryptKey = $EncryptionCryptKey
$config.EncryptionSettings.InitVector = $EncryptionInitVector
$config.RequestAccessCodeMailId = ($RequestAccessCodeMailId -join ",")
$config.ApiUrl = $ApiUrl

# Depth 20 to make sure the nested Serilog/Cors sections round-trip fully.
$config | ConvertTo-Json -Depth 20 | Set-Content -Path $AppSettingsPath -Encoding UTF8

Write-Host "Configuration injected successfully."
