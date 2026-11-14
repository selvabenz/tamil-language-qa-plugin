$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
. (Join-Path $repoRoot 'scripts\source-state.ps1')
$testRoot=Join-Path $repoRoot ('artifacts\build-integrity-'+[Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path (Join-Path $testRoot 'reference\tamil_punarchi_agent_package') -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $testRoot 'artifacts\build') -Force | Out-Null
$catalogPath=Join-Path $testRoot 'reference\tamil_punarchi_agent_package\rules.json'
'{}' | Set-Content $catalogPath -Encoding ASCII
'artifacts/' | Set-Content (Join-Path $testRoot '.gitignore') -Encoding ASCII
& git -C $testRoot init -q
if($LASTEXITCODE -ne 0){throw 'Test repository creation failed.'}
& git -C $testRoot add .
& git -C $testRoot -c user.name=Regression -c user.email=regression@example.invalid commit -qm baseline
if($LASTEXITCODE -ne 0){throw 'Test baseline commit failed.'}
$baseline=Get-SourceState $testRoot
if($baseline.Dirty){throw 'Committed baseline should be clean.'}
$buildDir=Join-Path $testRoot 'artifacts\build'
Copy-Item $catalogPath $buildDir
$manifest=[PSCustomObject]@{dirty=$false;sourceCommit=$baseline.Commit;catalogSha256=(Get-FileHash $catalogPath).Hash.ToLowerInvariant()}
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll')){[IO.File]::WriteAllText((Join-Path $buildDir $name),'synthetic compiled output '+$name)}
$hashes=@{};foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json')){$hashes[$name]=(Get-FileHash (Join-Path $buildDir $name)).Hash.ToLowerInvariant()}
$manifest | Add-Member -NotePropertyName fileHashes -NotePropertyValue (($hashes | ConvertTo-Json) | ConvertFrom-Json)
Assert-PackageSource $testRoot $manifest $buildDir
function Expect-Refusal([scriptblock]$Action,[string]$Name){$refused=$false;try{& $Action}catch{$refused=$true};if(!$refused){throw "Expected refusal: $Name"}}
'{"changed":true}' | Set-Content $catalogPath -Encoding ASCII
if(!(Get-SourceState $testRoot).Dirty){throw 'Uncommitted rule catalog must mark source dirty.'}
Expect-Refusal {Assert-PackageSource $testRoot $manifest $buildDir} 'rules changed after build'
& git -C $testRoot checkout -- reference/tamil_punarchi_agent_package/rules.json
'new input' | Set-Content (Join-Path $testRoot 'new-input.txt')
if(!(Get-SourceState $testRoot).Dirty){throw 'Untracked package input must mark source dirty.'}
Expect-Refusal {Assert-PackageSource $testRoot $manifest $buildDir} 'untracked input'
& git -C $testRoot add new-input.txt
& git -C $testRoot -c user.name=Regression -c user.email=regression@example.invalid commit -qm next
Expect-Refusal {Assert-PackageSource $testRoot $manifest $buildDir} 'different source commit'
$manifest.sourceCommit=(Get-SourceState $testRoot).Commit
'tampered output' | Set-Content (Join-Path $buildDir 'rules.json')
Expect-Refusal {Assert-PackageSource $testRoot $manifest $buildDir} 'catalog output tampering'
Copy-Item $catalogPath $buildDir -Force
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll')){
    $binary=Join-Path $buildDir $name;$original=[IO.File]::ReadAllText($binary)
    [IO.File]::WriteAllText($binary,'replaced binary')
    Expect-Refusal {Assert-PackageSource $testRoot $manifest $buildDir} 'binary output tampering'
    [IO.File]::WriteAllText($binary,$original)
}
$manifest | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $buildDir 'build.json')
'install guide' | Set-Content (Join-Path $buildDir 'INSTALL.md')
'source attribution' | Set-Content (Join-Path $buildDir 'SOURCES.md')
$packageNames=@('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json','INSTALL.md','SOURCES.md')
$sums=$packageNames | ForEach-Object {(Get-FileHash (Join-Path $buildDir $_)).Hash.ToLowerInvariant()+'  '+$_}
$sums | Set-Content (Join-Path $buildDir 'SHA256SUMS.txt')
Assert-PackageChecksums $buildDir
'bad binary' | Set-Content (Join-Path $buildDir 'TamilSandhi.ptxplg')
Expect-Refusal {Assert-PackageChecksums $buildDir} 'tampered package before installation'
[IO.File]::WriteAllText((Join-Path $buildDir 'TamilSandhi.ptxplg'),'synthetic compiled output TamilSandhi.ptxplg')
'bad manifest' | Set-Content (Join-Path $buildDir 'build.json')
Expect-Refusal {Assert-PackageChecksums $buildDir} 'tampered package manifest'
$sums[0] | Set-Content (Join-Path $buildDir 'SHA256SUMS.txt')
Expect-Refusal {Assert-PackageChecksums $buildDir} 'incomplete checksum list'
Write-Output 'PASS: source provenance, catalog/binary tampering, package manifest and pre-install checksum gates.'
