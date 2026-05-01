[CmdletBinding()]
param(
    [string] $FeedPath = "artifacts/local-nuget",

    [string] $Configuration = "Release",

    [string[]] $Project,

    [switch] $NoRestore,

    [switch] $AddSource,

    [string] $SourceName = "AtyaLocal"
)

$ErrorActionPreference = "Stop"

$repoRoot = $PSScriptRoot

if (-not (Test-Path (Join-Path $repoRoot "Testing.sln"))) {
    $parentRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

    if (Test-Path (Join-Path $parentRoot "Testing.sln")) {
        $repoRoot = $parentRoot
    }
    else {
        throw "Unable to locate Testing.sln from script path '$PSScriptRoot'."
    }
}

if ([System.IO.Path]::IsPathRooted($FeedPath)) {
    $resolvedFeedPath = [System.IO.Path]::GetFullPath($FeedPath)
}
else {
    $resolvedFeedPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $FeedPath))
}

New-Item -ItemType Directory -Force -Path $resolvedFeedPath | Out-Null

function Get-IsPackableProject {
    param(
        [Parameter(Mandatory = $true)]
        [string] $ProjectPath
    )

    [xml] $projectXml = Get-Content -LiteralPath $ProjectPath -Raw
    $isPackableValues = @(
        $projectXml.Project.PropertyGroup |
            ForEach-Object { $_.IsPackable } |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    )

    if ($isPackableValues.Count -eq 0) {
        return $true
    }

    return [bool]::Parse($isPackableValues[-1])
}

function Get-ProjectPackageMetadata {
    param(
        [Parameter(Mandatory = $true)]
        [string] $ProjectPath
    )

    [xml] $projectXml = Get-Content -LiteralPath $ProjectPath -Raw

    $packageIds = @(
        $projectXml.Project.PropertyGroup |
            ForEach-Object { $_.PackageId } |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    )

    $versions = @(
        $projectXml.Project.PropertyGroup |
            ForEach-Object { if ($_.PackageVersion) { $_.PackageVersion } else { $_.Version } } |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    )

    $packageId = if ($packageIds.Count -gt 0) {
        $packageIds[-1]
    }
    else {
        [System.IO.Path]::GetFileNameWithoutExtension($ProjectPath)
    }

    $version = if ($versions.Count -gt 0) {
        $versions[-1]
    }
    else {
        "1.0.0"
    }

    [pscustomobject]@{
        PackageId = $packageId
        Version = $version
    }
}

if ($Project -and $Project.Count -gt 0) {
    $projectFiles = foreach ($projectPath in $Project) {
        $candidate = if ([System.IO.Path]::IsPathRooted($projectPath)) {
            $projectPath
        }
        else {
            Join-Path $repoRoot $projectPath
        }

        Get-Item -LiteralPath $candidate
    }
}
else {
    $projectFiles = Get-ChildItem -Path (Join-Path $repoRoot "src") -Recurse -Filter "*.csproj" |
        Where-Object { Get-IsPackableProject -ProjectPath $_.FullName }
}

if (-not $projectFiles -or $projectFiles.Count -eq 0) {
    throw "No packable projects were found."
}

Write-Host "Local NuGet feed: $resolvedFeedPath"
Write-Host "Configuration: $Configuration"
Write-Host ""

foreach ($projectFile in $projectFiles) {
    Write-Host "Packing $($projectFile.FullName)"

    $metadata = Get-ProjectPackageMetadata -ProjectPath $projectFile.FullName
    $existingPackageFiles = @(
        Join-Path $resolvedFeedPath "$($metadata.PackageId).$($metadata.Version).nupkg"
        Join-Path $resolvedFeedPath "$($metadata.PackageId).$($metadata.Version).snupkg"
    )

    foreach ($existingPackageFile in $existingPackageFiles) {
        if (Test-Path -LiteralPath $existingPackageFile) {
            Remove-Item -LiteralPath $existingPackageFile -Force
        }
    }

    $packArguments = @(
        "pack",
        $projectFile.FullName,
        "--configuration",
        $Configuration,
        "--output",
        $resolvedFeedPath,
        "/p:UseSharedCompilation=false",
        "--verbosity",
        "minimal"
    )

    if ($NoRestore) {
        $packArguments += "--no-restore"
    }

    & dotnet @packArguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed for $($projectFile.FullName)."
    }
}

if ($AddSource) {
    $sourceList = & dotnet nuget list source

    if ($LASTEXITCODE -ne 0) {
        throw "Unable to list NuGet sources."
    }

    if ($sourceList -match [regex]::Escape($SourceName)) {
        Write-Host ""
        Write-Host "NuGet source '$SourceName' already exists."
    }
    else {
        & dotnet nuget add source $resolvedFeedPath --name $SourceName

        if ($LASTEXITCODE -ne 0) {
            throw "Unable to add NuGet source '$SourceName'."
        }
    }
}

Write-Host ""
Write-Host "Done."
Write-Host "Use this feed from other projects with:"
Write-Host "dotnet nuget add source `"$resolvedFeedPath`" --name $SourceName"
