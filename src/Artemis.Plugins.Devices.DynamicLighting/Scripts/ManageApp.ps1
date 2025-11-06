[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet('install','uninstall')]
    [string]$Action,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$PackageName
)

$ErrorActionPreference = 'Stop'

function Get-ScriptDirectory {
    if ($PSCommandPath) { return Split-Path -Parent $PSCommandPath }
    if ($MyInvocation.MyCommand.Path) { return Split-Path -Parent $MyInvocation.MyCommand.Path }
    return (Get-Location).Path
}

function Get-InstalledPackage {
    try {
        return Get-AppxPackage -Name $PackageName -ErrorAction Stop
    }
    catch {
        return $null
    }
}


function Install-App {
    Write-Host "Installing Artemis Dynamic Lighting companion app..."

    $pkg = Get-InstalledPackage
    if ($pkg) {
        Write-Host 'App is already installed. Nothing to do.'
        return 0
    }

    $scriptDir = Get-ScriptDirectory
    $resourcesDir = Join-Path (Split-Path $scriptDir -Parent) 'Resources'

    # Look in the Resources directory for the MSIX file
    $packagePath = Get-ChildItem -Path $resourcesDir -Filter '*.msix' -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $packagePath) {
        Write-Error 'No .msix file found in the Resources directory.'
        return 1
    }

    Write-Host ("Found package in Resources directory: {0}" -f $packagePath.FullName)

    try {
        $params = @{ Path = $packagePath.FullName; ForceUpdateFromAnyVersion = $true; ForceApplicationShutdown = $true }
        Add-AppxPackage @params
    }
    catch {
        Write-Error ("Failed to install app package: {0}" -f $_.Exception.Message)
        return 1
    }

    # Verify
    $installed = Get-InstalledPackage
    if ($installed) {
        Write-Host 'Companion app successfully installed.'
        return 0
    }
    else {
        Write-Error 'Installation finished but package was not found via Get-AppxPackage.'
        return 1
    }
}

function Uninstall-App {
    Write-Host 'Uninstalling Artemis Dynamic Lighting companion app...'

    $pkg = Get-InstalledPackage
    if (-not $pkg) {
        Write-Host 'App is not installed. Nothing to remove.'
        return 0
    }

    $procs = Get-Process -Name 'Artemis.DynamicLighting' -ErrorAction SilentlyContinue
    if ($procs) { $procs | Stop-Process -Force -ErrorAction SilentlyContinue }
    
    try {
        Remove-AppxPackage -Package $pkg.PackageFullName -ErrorAction Stop
        Write-Host 'Companion app removed.'
        return 0
    }
    catch {
        Write-Error ("Failed to remove app package: {0}" -f $_.Exception.Message)
        return 1
    }
}

# Main
switch ($Action.ToLowerInvariant()) {
    'install'   { exit (Install-App) }
    'uninstall' { exit (Uninstall-App) }
    default     { Write-Error "Unknown action: $Action"; exit 2 }
}
