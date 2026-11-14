param([string]$ParatextDir='C:\Program Files\Paratext 9')
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$build=Join-Path $repoRoot 'artifacts\build'
$runRoot=Join-Path $repoRoot ('artifacts\resource-benchmark-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $runRoot | Out-Null
$compiler=Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'csc.exe'
$escapedRoot=$runRoot.Replace('"','""')
'internal static class ResourceBenchmarkPaths {public const string Root=@"'+$escapedRoot+'";}' | Set-Content (Join-Path $runRoot 'Paths.cs') -Encoding UTF8
$refs=@('/r:'+(Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'netstandard.dll'))
$refs+=@('CorePluginInterfaces.dll','PluginInterfaces.dll')|ForEach-Object{'/r:'+(Join-Path $ParatextDir $_)}
& $compiler /nologo /target:library /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll ('/r:'+(Join-Path $build 'TamilSandhi.Core.dll')) $refs ('/out:'+(Join-Path $runRoot 'TamilSandhi.ResourceBenchmark.ptxplg')) (Join-Path $repoRoot 'tests-host\ReadOnlyResourceBenchmark.cs') (Join-Path $runRoot 'Paths.cs')
if($LASTEXITCODE -ne 0){throw 'Read-only benchmark compilation failed'}
Copy-Item -LiteralPath (Join-Path $build 'rules.json') -Destination $runRoot
'User selected Tamil BSI OV Edited and OTCV for read-only local comparison'|Set-Content (Join-Path $runRoot 'request.txt')
Write-Output $runRoot
