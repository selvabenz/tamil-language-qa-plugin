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
Write-Output 'PASS: build/package integrity covers dirty catalog, untracked inputs, stale commit and catalog tampering.'
