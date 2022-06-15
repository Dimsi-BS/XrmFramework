# Quick start


## Create a project

- ### Using the CLI
Open a powershell windows in the folder of your choice.
Enter the following command to download the project templates for visual studio :
 
 ```PS
 dotnet new -i XrmFramework.Templates
 ```
 
 Now create a project using the following command :
 
 ```PS
PS C:\Temp> dotnet new xrmSolution -n {solutionName}
 ```
 
 The templating service will prompt you to accept the execution of a PowerShell initialization script :

```PS
Processing post-creation actions...
Template is configured to run the following action:
Description: Finalize XrmFramework solution initialization
Manual instructions: Initialisation XrmFramework
Actual command: powershell -File initXrm.ps1
Do you want to run this action (Y|N)?
```

You need to accept this execution to be sure to have the solution configured correctly.
 
- ### Using the ProjectManagerTool

Open the tool 

![Tool](https://github.com/PeteGuy/XrmFramework/blob/master/docs/images/ToolPic.PNG)

Click the button to create a project.


![CreateProj](https://github.com/PeteGuy/XrmFramework/blob/master/docs/images/CreateProjTool.PNG)
