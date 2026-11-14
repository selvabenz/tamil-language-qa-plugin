param(
    [string]$ParatextDir = 'C:\Program Files\Paratext 9',
    [string]$BuildNumber = '0',
    [switch]$CoreOnly
)
$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output = Join-Path $repoRoot 'artifacts\build'
New-Item -ItemType Directory -Force -Path $output | Out-Null
$compiler = Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw 'Install the Windows .NET Framework 4.8 developer tools.' }
$baseVersion = (Get-Content -LiteralPath (Join-Path $repoRoot 'VERSION') -Raw).Trim()
if ($baseVersion -notmatch '^\d+\.\d+\.\d+-dev$' -or $BuildNumber -notmatch '^\d{1,5}$' -or [int]$BuildNumber -gt 65534) { throw 'Invalid version/build number.' }
. (Join-Path $PSScriptRoot 'source-state.ps1')
$sourceState=Get-SourceState -RepoRoot $repoRoot
$commit=$sourceState.ShortCommit
$dirty=$sourceState.Dirty
$version = "$baseVersion.$BuildNumber+g$commit"
if ($dirty) {$version += '.dirty'}
$numeric = ($baseVersion -replace '-dev$','') + ".$BuildNumber"
$generated = Join-Path $output 'BuildInfo.cs'
@"
using System.Reflection;
[assembly: AssemblyTitle("Tamil Sandhi QA")]
[assembly: AssemblyVersion("$numeric")]
[assembly: AssemblyFileVersion("$numeric")]
[assembly: AssemblyInformationalVersion("$version")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETFramework,Version=v4.8", FrameworkDisplayName=".NET Framework 4.8")]
namespace TamilSandhi { public static class BuildInfo { public const string Version = "$version"; public const string Commit = "$commit"; } }
"@ | Set-Content -LiteralPath $generated -Encoding UTF8
$coreSources = @(Get-ChildItem (Join-Path $repoRoot 'src\TamilSandhi.Core\*.cs') | ForEach-Object FullName)
$core = Join-Path $output 'TamilSandhi.Core.dll'
& $compiler /nologo /target:library /optimize+ /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll "/out:$core" $coreSources $generated
if ($LASTEXITCODE -ne 0) { throw 'Core compilation failed.' }
$test = Join-Path $output 'TamilSandhi.Tests.exe'
$testSources = @(Get-ChildItem (Join-Path $repoRoot 'tests\*.cs') | ForEach-Object FullName)
& $compiler /nologo /target:exe /optimize+ /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll "/r:$core" "/out:$test" $testSources
if ($LASTEXITCODE -ne 0) { throw 'Test compilation failed.' }
& $test $repoRoot
if ($LASTEXITCODE -ne 0) { throw 'Core tests failed.' }
if (!$CoreOnly) {
    $references = @('CorePluginInterfaces.dll','PluginInterfaces.dll','EmbeddedUiPluginInterfaces.dll') | ForEach-Object {
        $file = Join-Path $ParatextDir $_
        if (!(Test-Path -LiteralPath $file)) { throw "Missing host SDK assembly: $file" }
        "/r:$file"
    }
    $pluginSources = @(Get-ChildItem (Join-Path $repoRoot 'src\TamilSandhi.Plugin\*.cs') | ForEach-Object FullName)
    $netstandard = Join-Path ([Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory()) 'netstandard.dll'
    & $compiler /nologo /target:library /optimize+ /warnaserror+ /r:System.Core.dll /r:System.Web.Extensions.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll "/r:$netstandard" "/r:$core" $references "/out:$(Join-Path $output 'TamilSandhi.ptxplg')" $pluginSources
    if ($LASTEXITCODE -ne 0) { throw 'Plugin compilation failed.' }
    $hostTests=Join-Path $output 'HostContractTests.exe'
    & $compiler /nologo /target:exe /warnaserror+ /r:System.Core.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll "/out:$hostTests" (Join-Path $repoRoot 'tests-host\HostContractTests.cs') (Join-Path $repoRoot 'tests-host\WindowRegressions.cs')
    if ($LASTEXITCODE -ne 0) { throw 'Host contract test compilation failed.' }
    & $hostTests $output $ParatextDir
    if ($LASTEXITCODE -ne 0) { throw 'Host contract tests failed.' }
}
Copy-Item -LiteralPath (Join-Path $repoRoot 'reference\tamil_punarchi_agent_package\rules.json') -Destination $output
$catalogHash=(Get-FileHash -LiteralPath (Join-Path $output 'rules.json') -Algorithm SHA256).Hash.ToLowerInvariant()
$fileHashes=@{};foreach($name in @('TamilSandhi.Core.dll','rules.json')){$fileHashes[$name]=(Get-FileHash -LiteralPath (Join-Path $output $name) -Algorithm SHA256).Hash.ToLowerInvariant()}
if(!$CoreOnly){$fileHashes['TamilSandhi.ptxplg']=(Get-FileHash -LiteralPath (Join-Path $output 'TamilSandhi.ptxplg') -Algorithm SHA256).Hash.ToLowerInvariant()}
@{ version=$version; commit=$commit; sourceCommit=$sourceState.Commit; dirty=$dirty; coreOnly=[bool]$CoreOnly; fileHashes=$fileHashes; catalogSha256=$catalogHash; targetHost='9.5.110.1'; catalog='2026-11-13'; builtUtc=[DateTime]::UtcNow.ToString('o'); channel='prerelease' } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $output 'build.json') -Encoding UTF8
Write-Output "Build and tests passed: $version"
