function Get-SourceState {
    param([Parameter(Mandatory=$true)][string]$RepoRoot)
    $sourceCommit=(& git -C $RepoRoot rev-parse HEAD).Trim()
    if($LASTEXITCODE -ne 0){throw 'Cannot resolve source commit.'}
    # Include all package inputs: rules, attribution, installation docs and code.
    $sourceChanges=@(& git -C $RepoRoot status --porcelain --untracked-files=all)
    if($LASTEXITCODE -ne 0){throw 'Cannot determine source changes.'}
    [PSCustomObject]@{Commit=$sourceCommit; ShortCommit=$sourceCommit.Substring(0,8); Dirty=($sourceChanges.Count -gt 0)}
}

function Assert-PackageSource {
    param([Parameter(Mandatory=$true)][string]$RepoRoot,[Parameter(Mandatory=$true)]$Manifest,[Parameter(Mandatory=$true)][string]$BuildDir)
    $current=Get-SourceState -RepoRoot $RepoRoot
    if($Manifest.dirty -or $current.Dirty){throw 'Commit source changes and rebuild before packaging a release.'}
    if(!$Manifest.sourceCommit -or $Manifest.sourceCommit -ne $current.Commit){throw 'Build commit differs from current source; rebuild before packaging.'}
    $builtCatalog=(Get-FileHash -LiteralPath (Join-Path $BuildDir 'rules.json') -Algorithm SHA256).Hash.ToLowerInvariant()
    $sourceCatalog=(Get-FileHash -LiteralPath (Join-Path $RepoRoot 'reference\tamil_punarchi_agent_package\rules.json') -Algorithm SHA256).Hash.ToLowerInvariant()
    if(!$Manifest.catalogSha256 -or $Manifest.catalogSha256 -ne $builtCatalog -or $sourceCatalog -ne $builtCatalog){throw 'Rule catalog differs from the recorded source/build; rebuild before packaging.'}
}
