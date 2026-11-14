param([Parameter(Mandatory=$true)][string]$ReferenceDir,[ValidateSet('development','holdout')][string]$Stage='development',[ValidateSet('bsi','otcv')][string]$ReferenceId='bsi')
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$build=Join-Path $repoRoot 'artifacts\build'
$output=Join-Path $repoRoot ('artifacts\'+$ReferenceId+'-reference-'+$Stage+'-'+[DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss'))
$compiler=Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'csc.exe'
$exe=Join-Path $build 'ReferenceBenchmark.exe'
& $compiler /nologo /target:exe /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll ('/r:'+(Join-Path $build 'TamilSandhi.Core.dll')) ('/out:'+$exe) (Join-Path $repoRoot 'tests-corpus\ReferenceBenchmark.cs')
if($LASTEXITCODE -ne 0){throw 'Reference evaluator compilation failed'}
$referenceName=if($ReferenceId -eq 'otcv'){'User-supplied local OTCV USFM'}else{'User-supplied local BSI OV USFM'}
& $exe ([IO.Path]::GetFullPath($ReferenceDir)) $output $Stage (Join-Path $build 'rules.json') $referenceName
if($LASTEXITCODE -ne 0){throw 'Reference evaluation failed'}
Write-Output ('Private evaluation artifacts: '+$output)
