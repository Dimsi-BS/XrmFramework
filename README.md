# Xrm Framework 
This project is a compilation of tools and design pattern that has been used on several Microsoft Services Dynamics 365 projects.

# Design Pattern
Several design pattern are included in this solution :
1.	Definition of Services to access CRM Data from Plugins or external code (Webservices, console apps, ...)
2.	Automatic Plugin Step registration from source code (use of attributes to describe plugin and plugin steps)
3.	Advanced plugin traces (service calls are logged)
4.  Metadata Definition extraction tool (no more plain string attribute references)

# Quick start

## Set configuration informations
Add a `connectionStrings.config` file as follows in the `Config\` folder near `App.config`
```xml

  <connectionStrings>
    <!-- You must specify here the connection string to your environment
         You can specify several connection strings, you will select the one corresponding to the deployment
         environnement in the xrmFramework selectedConnection attribute below    
    -->
    <add name="Xrm" connectionString="AuthType=Office365; Url=https://yourorg.crm4.dynamics.com; Username=****@***.**; Password=*****"/>
  </connectionStrings>

```

Edit the `App.config` file in the ``Config`` solution folder to configure the connection to Dynamics 365 and the configuration of project deployments.

Define the connection string to Dynamics 365

You can specify several connection strings and pick the one to use in deployment tools by modifying the attribute `selectedConnection` 
```xml
<xrmFramework selectedConnection="Xrm">
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
    <add name="Plugins" targetSolution="PluginsSolutionUniqueName" type="PluginsWorkflows" />
    <add name="Workflows" targetSolution="WorkflowsSolutionUniqueName" type="PluginsWorkflows" />
    <add name="Webresources" targetSolution="WebResourcesSolutionUniqueName" type="WebResources" />
</projects>

```

## Generate model definitions
Launch the executable `DefinitionManager.exe` in the Deploy solution folder.

<img src="docs/images/definitionManager1.png" width="100" alt="Start of DefinitionManager" />

## Create your first plugin

Implement a plugin using the `Plugin` base class.

```csharp
using Model;
using Plugins;

public class SamplePlugin : Plugin
{
    /// <summary>
    /// AddSteps allows to generate SdkMessageProcessing items.
    /// </summary>
    protected override void AddSteps()
    {
        AddStep(Stages.PreValidation, Messages.Create, Modes.Synchronous, AccountDefinition.EntityName, nameof(AssignContactOwnerToAccount));

        AddStep(Stages.PostOperation, Messages.Update, Modes.Synchronous, AccountDefinition.EntityName, nameof(UpdateSubContactInfos));
    }

    ...
}



```

# Contribute
The code is currently on production on several big project but is not at all finished. If you have time and motivation to contribute to it you are welcome to make pull requests. Il will study them and include the changes in this repo. 
