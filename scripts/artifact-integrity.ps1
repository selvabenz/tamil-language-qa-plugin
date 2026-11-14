function Assert-BuildArtifacts {
    param([Parameter(Mandatory=$true)][string]$BuildDir,[Parameter(Mandatory=$true)]$Manifest)
    if($Manifest.coreOnly){throw 'A core-only build cannot be packaged or installed.'}
    foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json')){
        $property=if($Manifest.fileHashes){$Manifest.fileHashes.PSObject.Properties[$name]}else{$null}
        if(!$property -or $property.Value -notmatch '^[0-9a-f]{64}$'){throw "Missing build-time hash: $name"}
        $actual=(Get-FileHash -LiteralPath (Join-Path $BuildDir $name) -Algorithm SHA256).Hash.ToLowerInvariant()
        if($actual -ne $property.Value){throw "Build artifact changed since compilation: $name"}
    }
}
function Assert-PackageChecksums {
    param([Parameter(Mandatory=$true)][string]$PackageDir)
    $expected=@('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json','INSTALL.md','SOURCES.md')
    $seen=@{}
    foreach($line in Get-Content -LiteralPath (Join-Path $PackageDir 'SHA256SUMS.txt')){
        if($line -notmatch '^([a-fA-F0-9]{64})  ([^/\\]+)$'){throw 'Invalid package checksum entry.'}
        $hash=$Matches[1].ToLowerInvariant();$name=$Matches[2]
        if($expected -cnotcontains $name -or $seen.ContainsKey($name)){throw 'Unexpected or duplicate package checksum entry.'}
        if((Get-FileHash -LiteralPath (Join-Path $PackageDir $name) -Algorithm SHA256).Hash.ToLowerInvariant() -ne $hash){throw "Package checksum mismatch: $name"}
        $seen[$name]=$true
    }
    if($seen.Count -ne $expected.Count){throw 'Incomplete package checksum list.'}
}
