# Xrm Framework 
The XrmFramework project is the result of 15+ years working on Dynamics 365 / Dataverse projects.

- [Xrm Framework](#xrm-framework)
  - [Design Pattern](#design-pattern)
  - [Quick start](#quick-start)
    - [Download XrmFramework project templates](#download-xrmframework-project-templates)
    - [Create a new solution for you Dynamics 365 / Dataverse project](#create-a-new-solution-for-you-dynamics-365--dataverse-project)
    - [Configure the new project](#configure-the-new-project)
  - [Generate model definitions](#generate-model-definitions)
  - [Create your first plugin](#create-your-first-plugin)
    - [Defining Steps to register](#defining-steps-to-register)
    - [Adding details to the registered steps](#adding-details-to-the-registered-steps)
    - [Choosing method arguments](#choosing-method-arguments)
  - [Utilities](#utilities)
  - [Contribute](#contribute)

## Design Pattern
Several design pattern are included in this framework :
1.	Definition of Services to access CRM Data from Plugins or external code (Webservices, console apps, ...)
2.	Automatic Plugin Step registration from source code (use of attributes to describe plugin and plugin steps)
3.	Advanced plugin traces (service calls are logged)
4.  Metadata Definition extraction tool (no more plain string attribute references)
5.  Definition and automatic registration of Custom Apis

## Quick start

### Download XrmFramework project templates
The XrmFramework project uses the `dotnet new` templating capabilities.

To start using XrmFramework on a new project you have to download the XrmFramework.Templates NuGet package by running the following command :

```PS
dotnet new -i XrmFramework.Templates
```

### Create a new solution for you Dynamics 365 / Dataverse project
To instantiate a new solution you will need to run the following command:

```PS
PS C:\Temp> dotnet new xrmSolution -n {solutionName}
```

The -n argument will create the solution in the `C:\Temp\{solutionName}`

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


You can add `--accept-scripts` to install the solution and run the script without the `dotnet new` script acceptance prompt.



### Configure the new project
A `connectionStrings.config` file as been created in the `Config\` folder near of the new solution.

This file will contain the connectionStrings needed for the tools to connect to your Dynamics 365 / Dataverse environments.
```xml

  <connectionStrings>
    <!-- You must specify here the connection string to your environment
         You can specify several connection strings, you will select the one corresponding to the deployment
         environnement in the xrmFramework selectedConnection attribute below    
    -->
    <add name="XrmDev" connectionString="AuthType=Office365; Url=https://yourorg.crm.dynamics.com; Username=****@***.**; Password=*****"/>
    <add name="XrmDev2" connectionString="AuthType=ClientSecret; Url=https://yourorg.crm.dynamics.com; ClientId=00000000-0000-0000-0000-000000000000; ClientSecret=*****"/>
  </connectionStrings>

```

The connectionStrings allowed are thoses documented in the Microsoft documentation [Use connection strings in XRM tooling to connect to Microsoft Dataverse](https://docs.microsoft.com/en-us/powerapps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect)

The ``connectionStrings.config`` file is added to the .gitignore so your connectionString will not be sent to the repository and each developer can use his credentials.

Edit the `xrmFramework.config` file in the ``Config`` solution folder to configure the connection to Dynamics 365 and the configuration of project deployments.

```xml
<xrmFramework selectedConnection="XrmDev">
    <entitySolution name="EntitiesSolutionUniqueName" />
    <projects>
        <add name="Contoso.Plugins" targetSolution="PluginsSolutionUniqueName" type="PluginsWorkflows" />
        <add name="Contoso.Webresources" targetSolution="WebResourcesSolutionUniqueName" type="WebResources" />
    </projects>
</xrmFramework>
```

You can specify several connection strings and pick the one to use in deployment tools by modifying the attribute `selectedConnection`.
```xml
<xrmFramework selectedConnection="XrmDev">
    ...    
</xrmFramework>
```

Specify the solution unique name of the solution that is holding the entities in your Dynamics 365 implementation.
```xml
<entitySolution name="EntitiesSolutionUniqueName" />
```

Define project list you have in your solution and the corresponding deployment target solution

```xml

<projects>
    <add name="Contoso.Plugins" targetSolution="PluginsSolutionUniqueName" type="PluginsWorkflows" />
    <add name="Contoso.Webresources" targetSolution="WebResourcesSolutionUniqueName" type="WebResources" />
</projects>

```

## Generate model definitions
Launch the project executable `Utils\DefinitionManager` ( you can set the project as Startup project and run it pressing Ctrl + F5 in Visual Studio )

<img src="docs/images/definitionManager1.png" width="800" alt="Start of DefinitionManager" />

The program retrieves all the entities that are referenced in your entities solution.

The definitions already added to you solution will be automatically selected.

<img src="docs/images/definitionManager2.png" width="800" alt="Loaded entities" />

You can select attributes to add them to already generated definitions or select new entities to generate definition files for them.

<img src="docs/images/definitionManager3.png" width="300" alt="Select attribute" />

Optionset attributes, when selected, generate corresponding enums

<img src="docs/images/definitionManager5.png" width="500" alt="Select attribute" />

```csharp
	[OptionSetDefinition(AccountDefinition.EntityName, AccountDefinition.Columns.AccountCategoryCode)]
	public enum Category
	{
		Null = 0,
		[Description("Preferred Customer")]
		PreferredCustomer = 1,
		[Description("Standard")]
		Standard = 2,
	}
```

When you have finished selecting the needed elements you can click on the "Generate Definitions" button.

<img src="docs/images/definitionManager4.png" width="300" alt="Generate definitions" />

The ``Contoso.Core`` project is now updated with the definitions you chose.

## Create your first plugin

Implement a plugin using the `Plugin` base class.

```csharp
using Contoso.Core;
using XrmFramework;

public class SamplePlugin : Plugin
{
    #region .ctor

    public SamplePlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig)
    {
    }

    #endregion
    ...
}

```

### Defining Steps to register

Implement the ``AddSteps`` method to define the steps that this plugin will manage

```csharp
    protected override void AddSteps()
    {
        AddStep(Stages.PreValidation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName, nameof(Method1));
        AddStep(Stages.PreValidation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(Method1));

        AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(Method2));
    }

```

### Adding details to the registered steps
For each method that you reference you can specify several information using method attributes :

```csharp
   
    [PreImage(AccountDefinition.Columns.Name)]
    [PostImage(AccountDefinition.Columns.Name)]
    [FilteringAttributes(AccountDefinition.Columns.Name, AccountDefinition.Columns.AccountNumber)]
    [ExecutionOrder(100)]
    public void Method(...)

```

- ``PreImageAttribute`` is used to define the fields that will be added in the preImage that will be registered

- ``PostImageAttribute`` is used to define the fields that will be added in the postImage that will be registered

- ``FilteringAttributesAttribute`` is used to define on which attribute change the method should launch

- ``ExecutionOrderAttribute`` allows specifying the execution order that will be registered

### Choosing method arguments

The step method registered in the ``AddSteps`` registration can be injected with services 

```csharp
public void Method(IPluginContext context, IAccountService accountService, ...)
```

## Utilities

XrmFramework contains a bunch of utility methods or Extensions to better work with the SDK.

[XrmFramework Utilities](docs/XrmFrameworkUtilities.md)

## Contribute
The code is currently on production on several big project but is not at all finished. If you have time and motivation to contribute to it you are welcome to make pull requests. Il will study them and include the changes in this repo. 
