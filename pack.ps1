param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = 'Stop'

Write-Host "== Building ($Configuration) =="
& dotnet build ParanoidAndroid.sln -c $Configuration | Out-Null

$dll = Get-ChildItem -Recurse -Filter ParanoidAndroid.dll | Where-Object { $_.FullName -match "ParanoidAndroid\\bin\\$Configuration" } | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $dll) { throw "Could not locate built DLL." }

if (-not (Test-Path Assemblies)) { New-Item -ItemType Directory -Path Assemblies | Out-Null }
Copy-Item $dll.FullName Assemblies/ParanoidAndroid.dll -Force

$packageName = "ParanoidAndroid-$(Get-Date -Format 'yyyyMMdd-HHmm')"
$zip = "$packageName.zip"

$include = @('About', 'Assemblies', 'Textures', 'Patches', 'Defs', 'Sounds', 'Languages', 'LICENSE', 'README.md')
$existing = $include | Where-Object { Test-Path $_ }

Write-Host "== Packaging: $zip =="
if (Test-Path $zip) { Remove-Item $zip -Force }
Compress-Archive -Path $existing -DestinationPath $zip -Force
Write-Host "Package created: $zip"