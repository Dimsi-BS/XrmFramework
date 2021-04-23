

$targetZip = "$PSScriptRoot\XrmFramework.zip"
$workingFolder = "$PSSCriptRoot\WorkingFolder"

$projectName = Read-Host -Prompt "New project name"

Invoke-WebRequest "https://github.com/cgoconseils/XrmFramework/archive/master.zip" -OutFile $targetZip

Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Unzip $targetZip $workingFolder

Get-ChildItem $workingFolder -Recurse -Filter "XrmFramework.sln" | Rename-Item -NewName "$projectName.sln"

Get-ChildItem $workingFolder -Recurse -Filter "connectionStrings.sample" | Rename-Item -NewName "connectionStrings.config"

