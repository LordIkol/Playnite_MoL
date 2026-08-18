# Build and Deploy script for Mythos of Loki Theme & MythosHelper Plugin
param(
    [switch]$KillPlaynite = $false
)

$ErrorActionPreference = "Stop"

# Optional: stop Playnite if running to release file locks
if ($KillPlaynite) {
    Write-Host "Stopping Playnite processes if running..." -ForegroundColor Yellow
    Get-Process -Name "Playnite.DesktopApp" -ErrorAction SilentlyContinue | Stop-Process -Force
    Start-Sleep -Seconds 1
}

$themeSource = "i:\AntiGravity\Theme_Playnite"
$pluginProj = Join-Path $themeSource "Plugin\MythosHelper.csproj"
$pluginExtDest = "C:\Users\marcr\AppData\Local\Playnite\Extensions\MythosHelper_Loki"
$themeDest = "C:\Users\marcr\AppData\Roaming\Playnite\Themes\Desktop\Mythos_of_Loki"

Write-Host "=== 1. Building MythosHelper Plugin ===" -ForegroundColor Cyan
dotnet build $pluginProj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Error "Plugin build failed!"
    exit 1
}

Write-Host "`n=== 2. Deploying Plugin to Extensions ===" -ForegroundColor Cyan
if (-not (Test-Path $pluginExtDest)) {
    New-Item -ItemType Directory -Path $pluginExtDest -Force | Out-Null
}

$pluginDll = Join-Path $themeSource "Plugin\bin\Release\net462\MythosHelper.dll"
$pluginManifest = Join-Path $themeSource "Plugin\extension.yaml"

try {
    Copy-Item $pluginDll -Destination $pluginExtDest -Force
    Copy-Item $pluginManifest -Destination $pluginExtDest -Force
    Write-Host "Plugin deployed successfully to $pluginExtDest" -ForegroundColor Green
} catch {
    Write-Warning "Could not copy DLL - Playnite is likely running. Close Playnite or run with -KillPlaynite."
    Write-Warning $_.Exception.Message
}

Write-Host "`n=== 3. Deploying Theme Files ===" -ForegroundColor Cyan
if (-not (Test-Path $themeDest)) {
    New-Item -ItemType Directory -Path $themeDest -Force | Out-Null
}

# Theme directories to copy
$directories = @(
    "CustomControls",
    "DefaultControls",
    "DerivedStyles",
    "Icons",
    "Images",
    "Localization",
    "Views"
)

foreach ($dir in $directories) {
    $srcDir = Join-Path $themeSource $dir
    $destDir = Join-Path $themeDest $dir
    if (Test-Path $srcDir) {
        Write-Host "Copying directory $dir..."
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        Copy-Item -Path "$srcDir\*" -Destination $destDir -Recurse -Force
    }
}

# Theme root files to copy
$files = @(
    "Common.xaml",
    "Constants.xaml",
    "Media.xaml",
    "theme.yaml",
    "themeExtras.yaml",
    "thememodifier.yaml"
)

foreach ($file in $files) {
    $srcFile = Join-Path $themeSource $file
    if (Test-Path $srcFile) {
        Write-Host "Copying file $file..."
        Copy-Item -Path $srcFile -Destination $themeDest -Force
    }
}

Write-Host "`n=== Deployment Complete ===" -ForegroundColor Green
Write-Host "Theme copied to: $themeDest"
Write-Host "Plugin copied to: $pluginExtDest"
