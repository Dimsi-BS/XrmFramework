# Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

Write-Host "Downloading Nuget.exe..." -ForegroundColor Blue

$rootFolder = (Get-Item -Path $PSScriptRoot).FullName

$sourceNugetExe = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$targetNugetExe = "$rootFolder\nuget.exe"
Invoke-WebRequest $sourceNugetExe -OutFile $targetNugetExe
Set-Alias nuget $targetNugetExe

Write-Host "Nuget.exe downloaded" -ForegroundColor Green

Write-Host "Installing XrmFramework.Sources package..." -ForegroundColor Blue

nuget install XrmFramework.Sources -NoCache -PreRelease -ExcludeVersion

Write-Host "XrmFramework.Sources package installation succeedeed" -ForegroundColor Green

$newVersionCommonPath = "$rootFolder\XrmFramework.Sources\content\XrmFramework.Common" 

$destination = Get-ChildItem $rootFolder -Recurse -Filter XrmFramework.Common | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $false -and $_.PSIsContainer -eq $true }
$destinationParentPath = $destination.Parent.FullName

Write-Host "Deletion of existing XrmFramework.Common folder..." -ForegroundColor Blue
Remove-Item $destination.FullName -Force -Recurse -ErrorAction SilentlyContinue
Write-Host "Existing XrmFramework.Common folder deleted" -ForegroundColor Green

Write-Host "Copy of new XrmFramework.Common folder content..." -ForegroundColor Blue
Copy-Item $newVersionCommonPath -Destination $destinationParentPath -Force -Recurse -ErrorAction Stop
Write-Host "New XrmFramework.Common folder content done" -ForegroundColor Green

$newVersionSdkExtensionPath = "$rootFolder\XrmFramework.Sources\content\XrmFramework.SdkExtension" 

$destination = Get-ChildItem $rootFolder -Recurse -Filter XrmFramework.SdkExtension | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $false -and $_.PSIsContainer -eq $true }
$destinationParentPath = $destination.Parent.FullName

Write-Host "Deletion of existing XrmFramework.SdkExtension folder..." -ForegroundColor Blue
Remove-Item $destination.FullName -Force -Recurse -ErrorAction SilentlyContinue
Write-Host "Existing XrmFramework.SdkExtension folder deleted" -ForegroundColor Green

Write-Host "Copy of new XrmFramework.SdkExtension folder content..." -ForegroundColor Blue
Copy-Item $newVersionSdkExtensionPath -Destination $destinationParentPath -Force -Recurse -ErrorAction Stop
Write-Host "New XrmFramework.SdkExtension folder content done" -ForegroundColor Green


Write-Host "Deletion of XrmFramework.Sources package..." -ForegroundColor Blue
Remove-Item "$rootFolder/XrmFramework.Sources" -Force -Recurse -ErrorAction Stop
Remove-Item "$rootFolder/nuget.exe" -Force -ErrorAction Stop
Write-Host "XrmFramework.Sources package deleted" -ForegroundColor Green


Write-Host "XrmFramework succesfully updated" -ForegroundColor Green