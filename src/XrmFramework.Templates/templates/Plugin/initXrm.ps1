
mv $safeprojectname$/* ./
mv Utils/* ../Utils/

rmdir $safeprojectname$
rmdir Utils
rm initXrm.ps1

$solutionName = Get-ChildItem .. -File '*.sln'
cd ..
dotnet sln $solutionName.Name add .\$safeprojectname$\$safeprojectname$.csproj

dotnet sln $solutionName.Name add .\Utils\Deploy.$safeprojectname$\Deploy.$safeprojectname$.csproj