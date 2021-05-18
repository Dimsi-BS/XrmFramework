
mv $safeprojectname$/* ./
mv Utils/* ../Utils/

rmdir $safeprojectname$
rmdir Utils
rm initXrm.ps1

$solutionName = Get-ChildItem .. -File '*.sln'
cd ..
dotnet sln $solutionName.Name add .\$safeprojectname$\$safeprojectname$.csproj

dotnet sln $solutionName.Name add .\Utils\Deploy.$safeprojectname$\Deploy.$safeprojectname$.csproj

$solutionUniqueName = Read-Host -Prompt 'Solution unique name'

$configFileName = "$PSScriptRoot\..\Config\xrmFramework.config"

$exists = Test-Path $configFileName -PathType leaf

Write-Host "config : $configFileName ($exists)"

[xml]$xmlDoc = Get-Content $configFileName

Write-Host $xmlDoc.xrmFramework.projects.ChildNodes[0].name

$project = $xmlDoc.CreateElement("add")

$project.SetAttribute("name", "$safeprojectname$")
$project.SetAttribute("targetSolution", $solutionUniqueName)
$project.SetAttribute("type", "PluginsWorkflows")

$xmlDoc.xrmFramework.projects.AppendChild($project)

$xmlDoc.Save($configFileName)


dotnet add .\Utils\RemoteDebugger\RemoteDebugger.csproj reference .\$safeprojectname$\$safeprojectname$.csproj
