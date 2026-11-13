$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$build=Join-Path $repoRoot 'artifacts\build'
$info=Get-Content -LiteralPath (Join-Path $build 'build.json') -Raw | ConvertFrom-Json
if($info.dirty){throw 'Commit source changes and rebuild before packaging a release.'}
$package=Join-Path $repoRoot ('artifacts\TamilSandhiQA-'+$info.version)
New-Item -ItemType Directory -Force -Path $package | Out-Null
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json')){Copy-Item -LiteralPath (Join-Path $build $name) -Destination $package -Force}
Copy-Item -LiteralPath (Join-Path $repoRoot 'docs\INSTALL.md') -Destination $package -Force
Copy-Item -LiteralPath (Join-Path $repoRoot 'reference\tamil_punarchi_agent_package\SOURCES.md') -Destination $package -Force
$names=@('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json','INSTALL.md','SOURCES.md')
$names | ForEach-Object { $hash=Get-FileHash -LiteralPath (Join-Path $package $_) -Algorithm SHA256; $hash.Hash.ToLowerInvariant()+'  '+$_ } | Set-Content -LiteralPath (Join-Path $package 'SHA256SUMS.txt') -Encoding ASCII
$zip=$package+'.zip'
Compress-Archive -LiteralPath (@($names)+@('SHA256SUMS.txt') | ForEach-Object {Join-Path $package $_}) -DestinationPath $zip -Force
Write-Output $zip
