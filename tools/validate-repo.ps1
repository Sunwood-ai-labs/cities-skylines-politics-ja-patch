[CmdletBinding()]
param(
    [string]$RepoPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not $RepoPath) {
    $RepoPath = Split-Path -Parent $PSScriptRoot
}

$required = @(
    "README.md",
    "README.ja.md",
    "LICENSE",
    "NOTICE.md",
    "src\PoliticsJaPatch.cs",
    "tools\build.ps1",
    "tools\apply-patch.ps1",
    "docs\index.md",
    "docs\ja\index.md",
    ".github\workflows\validate.yml"
)

foreach ($relativePath in $required) {
    $path = Join-Path $RepoPath $relativePath
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required file is missing: $relativePath"
    }
}

$blocked = Get-ChildItem -LiteralPath $RepoPath -Recurse -File |
    Where-Object {
        $_.FullName -notmatch "\\bin\\" -and
        $_.Extension -in @(".dll", ".exe", ".pdb")
    }

if ($blocked) {
    $names = ($blocked | Select-Object -ExpandProperty FullName) -join [Environment]::NewLine
    throw "Binary files should not be committed:`n$names"
}

$scripts = Get-ChildItem -LiteralPath (Join-Path $RepoPath "tools") -Filter "*.ps1" -File
foreach ($script in $scripts) {
    $tokens = $null
    $errors = $null
    [System.Management.Automation.Language.Parser]::ParseFile($script.FullName, [ref]$tokens, [ref]$errors) | Out-Null
    if ($errors.Count -gt 0) {
        throw "PowerShell parse error in $($script.FullName): $($errors[0].Message)"
    }
}

Write-Host "Repository validation passed."
