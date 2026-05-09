[CmdletBinding()]
param(
    [string]$CecilPath,
    [string]$GameRoot,
    [string]$Configuration = "Release",
    [string]$OutputDirectory
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$sourcePath = Join-Path $repoRoot "src\PoliticsJaPatch.cs"
if (-not (Test-Path -LiteralPath $sourcePath)) {
    throw "Source file not found: $sourcePath"
}

if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repoRoot ("bin\" + $Configuration)
}

function Resolve-CecilPath {
    param(
        [string]$ExplicitPath,
        [string]$ExplicitGameRoot
    )

    if ($ExplicitPath) {
        if (-not (Test-Path -LiteralPath $ExplicitPath)) {
            throw "Mono.Cecil.dll was not found at -CecilPath: $ExplicitPath"
        }

        return (Resolve-Path -LiteralPath $ExplicitPath).Path
    }

    $candidateRoots = @()
    if ($ExplicitGameRoot) {
        $candidateRoots += $ExplicitGameRoot
    }

    $candidateRoots += @(
        "D:\SteamLibrary\steamapps\common\Cities_Skylines",
        "C:\Program Files (x86)\Steam\steamapps\common\Cities_Skylines",
        "C:\Program Files\Steam\steamapps\common\Cities_Skylines"
    )

    foreach ($root in $candidateRoots | Where-Object { $_ }) {
        $candidate = Join-Path $root "Mono\lib\mono\unity\Mono.Cecil.dll"
        if (Test-Path -LiteralPath $candidate) {
            return (Resolve-Path -LiteralPath $candidate).Path
        }
    }

    throw "Mono.Cecil.dll was not found. Pass -CecilPath or -GameRoot."
}

function Resolve-CscPath {
    $candidates = @(
        "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\csc.exe",
        "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    throw "The .NET Framework C# compiler was not found."
}

$resolvedCecil = Resolve-CecilPath -ExplicitPath $CecilPath -ExplicitGameRoot $GameRoot
$csc = Resolve-CscPath

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$outputExe = Join-Path $OutputDirectory "PoliticsJaPatch.exe"
$runtimeCecil = Join-Path $OutputDirectory "Mono.Cecil.dll"

Copy-Item -LiteralPath $resolvedCecil -Destination $runtimeCecil -Force

& $csc /nologo /target:exe /optimize+ /out:$outputExe /reference:$resolvedCecil $sourcePath
if ($LASTEXITCODE -ne 0) {
    throw "Compilation failed."
}

[PSCustomObject]@{
    ExePath   = (Resolve-Path -LiteralPath $outputExe).Path
    CecilPath = (Resolve-Path -LiteralPath $runtimeCecil).Path
}
