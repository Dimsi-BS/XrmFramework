mv gitignore .gitignore
cp Config/connectionStrings.config.sample Config/connectionStrings.config

rm initXrm.ps1

# Read the existing file

[xml]$remoteDebuggerProj = Get-Content 'Utils\RemoteDebugger\RemoteDebugger.csproj'

# Since there are multiple elements that need to be 
# changed use a foreach loop
foreach ($element in $remoteDebuggerProj.Project.ItemGroup)
{
    foreach ($project in $element.ProjectReference) {
        if ($project.Include -like '*$safeprojectname$.Plugins.csproj') {
           if ($project.Aliases -eq $null) {
               $aliases = $remoteDebuggerProj.CreateElement('Aliases')
               $textNode =  $remoteDebuggerProj.CreateTextNode([System.String]::Copy("$safeprojectname$.Plugins").Replace(".",""))
               $aliases.AppendChild($textNode)
               $project.AppendChild($aliases)
           }
        }
    }
}
$remoteDebuggerProj.Save('Utils\RemoteDebugger\RemoteDebugger.csproj')
