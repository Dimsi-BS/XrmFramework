# Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
# Licensed under the MIT License. See License.txt in the project root for license information.

.\.nuget\nuget.exe install XrmFramework.Sources -NoCache -PreRelease -ExcludeVersion

$rootFolder = (Get-Item -Path $PSScriptRoot).FullName
$newVersionCommonPath = "$rootFolder\XrmFramework.Sources\content\XrmFramework.Common" 

$destination = Get-ChildItem $rootFolder -Recurse -Filter XrmFramework.Common | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $false -and $_.PSIsContainer -eq $true }
$destinationParentPath = $destination.Parent.FullName

Remove-Item $destination.FullName -Force -Recurse -ErrorAction SilentlyContinue

Copy-Item $newVersionCommonPath -Destination $destinationParentPath -Force -Recurse

$newVersionSdkExtensionPath = "$rootFolder\XrmFramework.Sources\content\XrmFramework.SdkExtension" 

$destination = Get-ChildItem $rootFolder -Recurse -Filter XrmFramework.SdkExtension | Where-Object { $_.FullName.Contains("XrmFramework.Sources") -eq $false -and $_.PSIsContainer -eq $true }
$destinationParentPath = $destination.Parent.FullName

Remove-Item $destination.FullName -Force -Recurse -ErrorAction SilentlyContinue

Copy-Item $newVersionSdkExtensionPath -Destination $destinationParentPath -Force -Recurse

Remove-Item "$rootFolder/XrmFramework.Sources" -Force -Recurse