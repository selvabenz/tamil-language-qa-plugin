param([string]$ParatextDir='C:\Program Files\Paratext 9',[string]$PackageDir)
$ErrorActionPreference='Stop'
if(Get-Process -Name Paratext -ErrorAction SilentlyContinue){throw 'Close Paratext normally before installing.'}
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$build=if($PackageDir){[IO.Path]::GetFullPath($PackageDir)}else{Join-Path $repoRoot 'artifacts\build'}
$hostExe=Join-Path $ParatextDir 'Paratext.exe'
if (!(Test-Path -LiteralPath $hostExe)) {throw 'Paratext installation not found.'}
$hostVersion=(Get-Item -LiteralPath $hostExe).VersionInfo.FileVersion
if ($hostVersion -ne '9.5.110.1') {throw "This development build targets 9.5.110.1; found $hostVersion."}
$plugins=[IO.Path]::GetFullPath((Join-Path $ParatextDir 'plugins'))
$target=[IO.Path]::GetFullPath((Join-Path $plugins 'TamilSandhiQA'))
if ([IO.Path]::GetDirectoryName($target) -ne $plugins) {throw 'Invalid installation target.'}
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json')) {
    if (!(Test-Path -LiteralPath (Join-Path $build $name))) {throw "Build first: missing $name"}
}
$identity=Get-Content -LiteralPath (Join-Path $build 'build.json') -Raw | ConvertFrom-Json
if($identity.dirty){throw 'Install a committed release build, not a dirty working-tree build.'}
New-Item -ItemType Directory -Path $target -Force | Out-Null
foreach($name in @('TamilSandhi.ptxplg','TamilSandhi.Core.dll','rules.json','build.json')) {
    $destination=Join-Path $target $name
    if(Test-Path -LiteralPath $destination) {Copy-Item -LiteralPath $destination -Destination ($destination+'.previous') -Force}
    Copy-Item -LiteralPath (Join-Path $build $name) -Destination $destination -Force
    if((Get-FileHash -LiteralPath $destination).Hash -ne (Get-FileHash -LiteralPath (Join-Path $build $name)).Hash){throw "Installed checksum mismatch: $name"}
}
Write-Output "Installed $($identity.version) and checksum-verified in $target. Restart Paratext, then open project menu > Tools > Checks > Tamil Sandhi QA."
