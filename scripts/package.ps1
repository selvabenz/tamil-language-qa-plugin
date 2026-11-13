$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$build=Join-Path $repoRoot 'artifacts\build'
$info=Get-Content -LiteralPath (Join-Path $build 'build.json') -Raw | ConvertFrom-Json
$package=Join-Path $repoRoot ('artifacts\TamilSandhiQA-'+$info.version)
New-Item -ItemType Directory -Force -Path $package | Out-Null
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json')){Copy-Item -LiteralPath (Join-Path $build $name) -Destination $package -Force}
Copy-Item -LiteralPath (Join-Path $repoRoot 'docs\INSTALL.md') -Destination $package -Force
Copy-Item -LiteralPath (Join-Path $repoRoot 'reference\tamil_punarchi_agent_package\SOURCES.md') -Destination $package -Force
Get-ChildItem -LiteralPath $package -File | ForEach-Object { $hash=Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256; $hash.Hash.ToLowerInvariant()+'  '+$_.Name } | Set-Content -LiteralPath (Join-Path $package 'SHA256SUMS.txt') -Encoding ASCII
$zip=$package+'.zip'
Compress-Archive -LiteralPath (Get-ChildItem -LiteralPath $package -File | ForEach-Object FullName) -DestinationPath $zip -Force
Write-Output $zip
