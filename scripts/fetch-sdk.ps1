param([string]$Destination)
$ErrorActionPreference='Stop'
if (!$Destination) {$Destination=Join-Path $PSScriptRoot '..\artifacts\sdk'}
$Destination=[IO.Path]::GetFullPath($Destination)
New-Item -ItemType Directory -Force -Path $Destination | Out-Null
Add-Type -AssemblyName System.IO.Compression.FileSystem
foreach($package in @('paratextcoreplugininterfaces','paratextplugininterfaces','paratextembeddeduiplugininterfaces')) {
    $archive=Join-Path $Destination ($package+'.nupkg')
    Invoke-WebRequest -UseBasicParsing -Uri "https://api.nuget.org/v3-flatcontainer/$package/2.0.100/$package.2.0.100.nupkg" -OutFile $archive
    $zip=[IO.Compression.ZipFile]::OpenRead($archive)
    try {
        foreach($entry in $zip.Entries) {
            if($entry.FullName -match '^lib/(netstandard2.0|net48)/([^/]+\.dll)$') {
                [IO.Compression.ZipFileExtensions]::ExtractToFile($entry,(Join-Path $Destination $Matches[2]),$true)
            }
        }
    } finally {$zip.Dispose()}
}
Write-Output "Downloaded official compile-only SDK 2.0.100 to $Destination. Do not include SDK binaries in the plugin package."
