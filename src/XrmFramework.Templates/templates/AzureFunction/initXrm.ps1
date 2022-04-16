rm initXrm.ps1

$solutionName = Get-ChildItem .. -File '*.sln'
cd ..
dotnet sln $solutionName.Name add .\$safeprojectname$\$safeprojectname$.csproj