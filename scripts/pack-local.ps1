<#
.SYNOPSIS
    Packs NovoNordisk.OpenTelemetry.Exporter.Bifrost into a local .nupkg for development.

.DESCRIPTION
    Until a new version is published to nuget.org via GitHub Actions, consumers
    can point a local NuGet feed at the output of this script. Defaults emit a
    timestamped version suffix (e.g. 1.1.6-dev.20260608-1430) so that re-packing
    without bumping the version still produces a distinct package — NuGet
    caches by id+version, so reusing a version silently serves stale content
    from the global cache.

    The csproj does not declare a <VersionPrefix>, so this script passes one
    explicitly via -p:VersionPrefix. Override with -VersionPrefix when you want
    to mirror the next intended release.

    The script:
      1. Restores and builds the library in Release.
      2. Runs `dotnet pack` into <OutputPath>.
      3. Prints the resulting .nupkg path and the exact PackageReference line
         a consumer should use.

.PARAMETER OutputPath
    Folder where the .nupkg is written. Defaults to <repo>/artifacts/packages.

.PARAMETER VersionPrefix
    The base version (without suffix) baked into the package. Default: 1.2.0
    (one patch ahead of the last published 1.1.5). Bump this to match whatever
    version the next release will carry.

.PARAMETER VersionSuffix
    Optional pre-release suffix appended to VersionPrefix.
    Pass an empty string to pack a "stable" version (just VersionPrefix, no
    suffix). Default: dev.<yyyyMMdd-HHmm>.

.PARAMETER Configuration
    Build configuration. Default: Release.

.PARAMETER NoBuild
    Skip the build step (assumes the project was just built).

.EXAMPLE
    .\scripts\pack-local.ps1
    Packs into artifacts/packages as 1.2.0-dev.<timestamp>.

.EXAMPLE
    .\scripts\pack-local.ps1 -VersionPrefix 1.2.0 -VersionSuffix ''
    Packs as the bare 1.2.0. Use sparingly: NuGet caches will serve a
    previously-restored copy of the same version.

.EXAMPLE
    .\scripts\pack-local.ps1 -OutputPath C:\dev\some-consumer\local-packages
    Drops the .nupkg straight into a consumer repo's local feed folder.
#>
[CmdletBinding()]
param(
    [string] $OutputPath,
    [string] $VersionPrefix = '1.1.6',
    [string] $VersionSuffix = ("dev." + (Get-Date -Format 'yyyyMMdd-HHmm')),
    [ValidateSet('Debug','Release')]
    [string] $Configuration = 'Release',
    [switch] $NoBuild
)

$ErrorActionPreference = 'Stop'

$repoRoot   = Resolve-Path (Join-Path $PSScriptRoot '..')
$projectDir = Join-Path $repoRoot 'src\NovoNordisk.OpenTelemetry.Exporter.Bifrost'
$projectPath = Join-Path $projectDir 'NovoNordisk.OpenTelemetry.Exporter.Bifrost.csproj'

if (-not (Test-Path $projectPath)) {
    throw "Could not find project at '$projectPath'."
}

if (-not $OutputPath) {
    $OutputPath = Join-Path $repoRoot 'artifacts\packages'
}

if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath | Out-Null
}

Write-Host "Project       : $projectPath"
Write-Host "Configuration : $Configuration"
Write-Host "VersionPrefix : $VersionPrefix"
Write-Host "VersionSuffix : $(if ($VersionSuffix) { $VersionSuffix } else { '<none>' })"
Write-Host "Output        : $OutputPath"
Write-Host ''

$packArgs = @(
    'pack', $projectPath,
    '--configuration', $Configuration,
    '--output', $OutputPath,
    "-p:VersionPrefix=$VersionPrefix"
)

if ($NoBuild) {
    $packArgs += '--no-build'
}

if ($VersionSuffix) {
    $packArgs += @('--version-suffix', $VersionSuffix)
}

Write-Host "> dotnet $($packArgs -join ' ')"
& dotnet @packArgs
if ($LASTEXITCODE -ne 0) {
    throw "dotnet pack failed with exit code $LASTEXITCODE."
}

$produced = Get-ChildItem -Path $OutputPath -Filter 'NovoNordisk.OpenTelemetry.Exporter.Bifrost.*.nupkg' |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $produced) {
    throw "Pack succeeded but no .nupkg was found in '$OutputPath'."
}

$version = [System.IO.Path]::GetFileNameWithoutExtension($produced.Name) -replace '^NovoNordisk\.OpenTelemetry\.Exporter\.Bifrost\.',''

Write-Host ''
Write-Host "Packed: $($produced.FullName)"
Write-Host ''
Write-Host "Consumer reference (Directory.Packages.props):"
Write-Host "  <PackageVersion Include=`"NovoNordisk.OpenTelemetry.Exporter.Bifrost`" Version=`"$version`" />"
Write-Host ''
Write-Host "Tip: if the consumer doesn't see the new version, clear its NuGet cache for this id:"
Write-Host "  dotnet nuget locals global-packages --clear  # nuclear option"
Write-Host "  # or remove just this package:"
Write-Host "  Remove-Item `"`$env:USERPROFILE\.nuget\packages\novonordisk.opentelemetry.exporter.bifrost`" -Recurse -Force -ErrorAction SilentlyContinue"
