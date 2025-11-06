[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidateSet('install','uninstall')]
    [string]$Action,

    [Parameter(Mandatory = $true, Position = 1)]
    [string]$Thumbprint,

    [Parameter(Mandatory = $true, Position = 2)]
    [string]$Password
)

$ErrorActionPreference = 'Stop'

function Get-ScriptDirectory {
    if ($PSCommandPath) { return Split-Path -Parent $PSCommandPath }
    if ($MyInvocation.MyCommand.Path) { return Split-Path -Parent $MyInvocation.MyCommand.Path }
    return (Get-Location).Path
}

function Get-ExistingRootCertByThumbprint([string]$tp) {
    try {
        $storePath = 'Cert:\LocalMachine\Root'
        return Get-ChildItem -Path $storePath | Where-Object { $_.Thumbprint -ieq $tp }
    }
    catch {
        return $null
    }
}

function Install-Certificate {
    Write-Host "Installing root certificate (thumbprint: $Thumbprint) to LocalMachine\\Root..."

    $existing = Get-ExistingRootCertByThumbprint -tp $Thumbprint
    if ($existing) {
        Write-Host 'Certificate already installed. Nothing to do.'
        return 0
    }

    $scriptDir = Get-ScriptDirectory
    $resourcesDir = Join-Path (Split-Path $scriptDir -Parent) 'Resources'
    $pfx = Get-ChildItem -Path $resourcesDir -Filter '*.pfx' -File -ErrorAction SilentlyContinue | Select-Object -First 1
    if (-not $pfx) {
        Write-Error 'No .pfx file found in the Resources directory.'
        return 1
    }

    Write-Host ("Found PFX: {0}" -f $pfx.FullName)

    $params = @{ FilePath = $pfx.FullName; CertStoreLocation = 'Cert:\\LocalMachine\\Root' }
    $secure = ConvertTo-SecureString -String $Password -AsPlainText -Force
    $params.Password = $secure

    try {
        $result = Import-PfxCertificate @params
        if (-not $result) {
            Write-Error 'Import-PfxCertificate returned no result; import may have failed.'
            return 1
        }
    }
    catch {
        Write-Error ("Failed to import PFX: {0}" -f $_.Exception.Message)
        return 1
    }

    # Verify
    $installed = Get-ExistingRootCertByThumbprint -tp $Thumbprint
    if ($installed) {
        Write-Host 'Certificate successfully installed.'
        return 0
    }
    else {
        Write-Error 'Certificate import finished but expected thumbprint was not found.'
        return 1
    }
}

function Uninstall-Certificate {
    Write-Host "Uninstalling root certificate (thumbprint: $Thumbprint) from LocalMachine\\Root..."
    $existing = Get-ExistingRootCertByThumbprint -tp $Thumbprint
    if (-not $existing) {
        Write-Host 'Certificate not found. Nothing to remove.'
        return 0
    }

    try {
        foreach ($cert in $existing) {
            Remove-Item -Path $cert.PSPath -Force -ErrorAction Stop
        }
        Write-Host 'Certificate removed.'
        return 0
    }
    catch {
        Write-Error ("Failed to remove certificate: {0}" -f $_.Exception.Message)
        return 1
    }
}

# Main
switch ($Action.ToLowerInvariant()) {
    'install'   { exit (Install-Certificate) }
    'uninstall' { exit (Uninstall-Certificate) }
    default     { Write-Error "Unknown action: $Action"; exit 2 }
}
