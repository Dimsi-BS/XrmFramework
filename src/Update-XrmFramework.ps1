# Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.


$rootFolder = (Get-Item -Path $PSScriptRoot).FullName
$targetNugetExe = "$rootFolder\.NuGet\nuget.exe"

if ([System.IO.File]::Exists($targetNugetExe) -eq $false) {
    Write-Host "Downloading Nuget.exe..." -ForegroundColor Blue
    $sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
    Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe

    Write-Host "Nuget.exe downloaded" -ForegroundColor Green
}

Write-Host "Installing XrmFramework.Sources package..." -ForegroundColor Blue

& $targetNugetExe install XrmFramework.Sources -NoCache -PreRelease -OutputDirectory $rootFolder

Write-Host "XrmFramework.Sources package installation succeedeed" -ForegroundColor Green

$sourceFolder = (Get-ChildItem $rootFolder | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $true -and $_.PSIsContainer -eq $true }).FullName

function SyncFolders {
    param( $FolderName )

    Write-Host "Syncing $FolderName folder..." -ForegroundColor Blue

    $sourceFolderName = "$sourceFolder\content\$FolderName"

    $destination = Get-ChildItem $rootFolder -Recurse -Filter $FolderName | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $false -and $_.PSIsContainer -eq $true }
    $destinationPath = $destination.FullName

    $folder1Files = Get-ChildItem -Recurse -path $sourceFolderName
    $folder2Files = Get-ChildItem -Recurse -path $destinationPath
    $file_Diffs = Compare-Object -ReferenceObject $folder1Files -DifferenceObject $folder2Files -IncludeEqual
    $file_Diffs | 
    ForEach-Object {
        $copyParams = @{'Path' = $_.InputObject.FullName }
        $copyParams.Destination = $_.InputObject.FullName -replace [regex]::Escape($sourceFolderName), $destinationPath
        if ($_.SideIndicator -eq '<=') { 
            copy-Item @copyParams -force
        }
        elseif ($_.SideIndicator -eq '=>') {
            Remove-Item @copyParams -Recurse -force -ErrorAction Ignore
        }
        elseif ( $_.InputObject.PSIsContainer -eq $false) {
            $diffContent = Compare-Object -ReferenceObject $(Get-Content $_.InputObject.FullName) -DifferenceObject $(Get-Content $copyParams.Destination)
            
            if ($diffContent.Length -gt 0) {
                copy-Item @copyParams -force
            }
        }
    }
    Write-Host "$FolderName folder synced" -ForegroundColor Green
}

SyncFolders -folderName "XrmFramework.Common"
SyncFolders -folderName "XrmFramework.SdkExtension"
SyncFolders -folderName "DefinitionManager"
SyncFolders -folderName "RemoteDebugger.Common"
SyncFolders -folderName "XrmProject.Utils"


Write-Host "Deletion of XrmFramework.Sources package..." -ForegroundColor Blue
Remove-Item $sourceFolder -Force -Recurse -ErrorAction Stop
Write-Host "XrmFramework.Sources package deleted" -ForegroundColor Green


Write-Host "XrmFramework succesfully updated" -ForegroundColor Green