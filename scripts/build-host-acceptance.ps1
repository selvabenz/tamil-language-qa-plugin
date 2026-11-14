param([string]$ParatextDir='C:\Program Files\Paratext 9')
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$runRoot=Join-Path $repoRoot ('artifacts\host-acceptance-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $runRoot | Out-Null
$compiler=Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'csc.exe'
$escapedRoot=$runRoot.Replace('"','""')
'internal static class AcceptancePaths { public const string Root = @"'+$escapedRoot+'"; }' | Set-Content (Join-Path $runRoot 'Paths.cs') -Encoding UTF8
$references=@('/r:'+(Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'netstandard.dll'))
$references+=@('CorePluginInterfaces.dll','PluginInterfaces.dll') | ForEach-Object { '/r:'+(Join-Path $ParatextDir $_) }
& $compiler /nologo /target:library /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll ('/r:'+(Join-Path $repoRoot 'artifacts\build\TamilSandhi.Core.dll')) $references ('/out:'+(Join-Path $runRoot 'TamilSandhi.Acceptance.ptxplg')) (Join-Path $repoRoot 'tests-host\InHostAcceptance.cs') (Join-Path $repoRoot 'src\TamilSandhi.Plugin\HostStore.cs') (Join-Path $runRoot 'Paths.cs')
if($LASTEXITCODE -ne 0){throw 'Acceptance helper compilation failed.'}
Copy-Item (Join-Path $repoRoot 'tests\fixtures\smoke.usfm') (Join-Path $runRoot 'fixture.usfm')
Copy-Item (Join-Path $repoRoot 'artifacts\build\rules.json') $runRoot
'Explicit one-shot disposable NSTest acceptance request' | Set-Content (Join-Path $runRoot 'request.txt')
Write-Output $runRoot
