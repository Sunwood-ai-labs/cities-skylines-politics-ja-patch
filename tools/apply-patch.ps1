[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$DllPath,
    [string]$WorkshopRoot,
    [string]$ModId = "3718153788",
    [string]$GameRoot,
    [string]$CecilPath,
    [switch]$NoBuild,
    [switch]$NoBackup,
    [switch]$ForceStopGame
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot

function Resolve-TargetDll {
    param(
        [string]$ExplicitDllPath,
        [string]$ExplicitWorkshopRoot,
        [string]$WorkshopModId
    )

    if ($ExplicitDllPath) {
        if (-not (Test-Path -LiteralPath $ExplicitDllPath)) {
            throw "Target DLL not found: $ExplicitDllPath"
        }

        return (Resolve-Path -LiteralPath $ExplicitDllPath).Path
    }

    $candidateRoots = @()
    if ($ExplicitWorkshopRoot) {
        $candidateRoots += $ExplicitWorkshopRoot
    }

    $candidateRoots += @(
        "D:\SteamLibrary\steamapps\workshop\content\255710",
        "C:\Program Files (x86)\Steam\steamapps\workshop\content\255710",
        "C:\Program Files\Steam\steamapps\workshop\content\255710"
    )

    foreach ($root in $candidateRoots | Where-Object { $_ }) {
        $candidate = Join-Path $root "$WorkshopModId\Cities-Skyline-Politics-Mod.dll"
        if (Test-Path -LiteralPath $candidate) {
            return (Resolve-Path -LiteralPath $candidate).Path
        }
    }

    throw "Politics & Elections DLL was not found. Pass -DllPath or -WorkshopRoot."
}

$targetDll = Resolve-TargetDll -ExplicitDllPath $DllPath -ExplicitWorkshopRoot $WorkshopRoot -WorkshopModId $ModId

$runningGame = @(Get-Process -Name "Cities" -ErrorAction SilentlyContinue)
if ($runningGame.Count -gt 0) {
    if (-not $ForceStopGame) {
        throw "Cities: Skylines is running. Close it first, or pass -ForceStopGame."
    }

    foreach ($process in $runningGame) {
        if ($PSCmdlet.ShouldProcess("process $($process.Id)", "Stop Cities: Skylines")) {
            Stop-Process -Id $process.Id -Force
        }
    }
}

$buildOutput = Join-Path $repoRoot "bin\Release"
$patcherExe = Join-Path $buildOutput "PoliticsJaPatch.exe"
if (-not $NoBuild) {
    & (Join-Path $PSScriptRoot "build.ps1") -CecilPath $CecilPath -GameRoot $GameRoot -Configuration Release -OutputDirectory $buildOutput | Out-Host
}

if (-not (Test-Path -LiteralPath $patcherExe)) {
    throw "Patcher executable not found. Run tools\build.ps1 first, or remove -NoBuild."
}

if (-not $NoBackup) {
    $stamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $backupPath = "$targetDll.codex-ja-backup-$stamp"
    if ($PSCmdlet.ShouldProcess($targetDll, "Create backup $backupPath")) {
        Copy-Item -LiteralPath $targetDll -Destination $backupPath -Force
    }
}

if ($PSCmdlet.ShouldProcess($targetDll, "Apply Japanese localization patch")) {
    & $patcherExe $targetDll
    if ($LASTEXITCODE -ne 0) {
        throw "Patch command failed."
    }
}
