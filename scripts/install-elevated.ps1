$ErrorActionPreference='Stop'
$installer=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot 'install.ps1'))
$log=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\artifacts\install-result.txt'))
Start-Transcript -LiteralPath $log -Force | Out-Null
try {
    if(Get-Process -Name Paratext -ErrorAction SilentlyContinue){throw 'Close Paratext normally before installing.'}
    & $installer
    Write-Output 'INSTALL_SUCCESS'
} catch {Write-Output ('INSTALL_FAILED: '+$_.Exception.Message);throw} finally {Stop-Transcript | Out-Null}
