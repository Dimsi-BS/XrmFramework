// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using NUnit.Framework;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.Factories;

[TestFixture]
public class PluginManifestReaderTests
{
    private const string Manifest = @"
{
  ""plugins"": [
    {
      ""fullName"": ""My.Ns.MyPlugin"",
      ""steps"": [
        {
          ""message"": ""Create"", ""stage"": ""PreOperation"", ""mode"": ""Synchronous"",
          ""entityName"": ""account"", ""methodName"": ""OnCreate"", ""methodNames"": [""OnCreate""],
          ""filteringAttributes"": [], ""order"": 1, ""impersonationUsername"": """", ""unsecureConfig"": null,
          ""preImage"": { ""allAttributes"": true, ""attributes"": [] },
          ""postImage"": { ""allAttributes"": false, ""attributes"": [] }
        },
        {
          ""message"": ""Update"", ""stage"": ""PostOperation"", ""mode"": ""Asynchronous"",
          ""entityName"": ""contact"", ""methodName"": ""OnUpdate"", ""methodNames"": [""OnUpdate""],
          ""filteringAttributes"": [""firstname"", ""lastname""], ""order"": 5, ""impersonationUsername"": ""admin"", ""unsecureConfig"": null,
          ""preImage"": { ""allAttributes"": false, ""attributes"": [""firstname""] },
          ""postImage"": { ""allAttributes"": false, ""attributes"": [] }
        }
      ]
    }
  ],
  ""workflows"": [], ""customApis"": []
}";

    private static Plugin Plugin() => PluginManifestReader.ReadPlugins(Manifest).Single();
    private static Step Step(string method) => Plugin().Steps.Single(s => s.MethodNames.Contains(method));

    [Test]
    public void ReadPlugins_MapsPluginIdentity()
    {
        var plugin = Plugin();
        Assert.Multiple(() =>
        {
            Assert.That(plugin.FullName, Is.EqualTo("My.Ns.MyPlugin"));
            Assert.That(plugin.Steps, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void ReadPlugins_MapsStepCoreData()
    {
        var step = Step("OnCreate");
        Assert.Multiple(() =>
        {
            Assert.That(step.PluginTypeName, Is.EqualTo("MyPlugin"));
            Assert.That(step.PluginTypeFullName, Is.EqualTo("My.Ns.MyPlugin"));
            Assert.That(step.Message, Is.EqualTo(Messages.Create));
            Assert.That(step.Stage, Is.EqualTo(Stages.PreOperation));
            Assert.That(step.Mode, Is.EqualTo(Modes.Synchronous));
            Assert.That(step.EntityName, Is.EqualTo("account"));
            Assert.That(step.Order, Is.EqualTo(1));
            Assert.That(step.PreImage.AllAttributes, Is.True);
        });
    }

    [Test]
    public void ReadPlugins_MapsFilteringImpersonationOrderAndImages()
    {
        var step = Step("OnUpdate");
        Assert.Multiple(() =>
        {
            Assert.That(step.Message, Is.EqualTo(Messages.Update));
            Assert.That(step.Mode, Is.EqualTo(Modes.Asynchronous));
            Assert.That(step.FilteringAttributes, Is.EquivalentTo(new[] { "firstname", "lastname" }));
            Assert.That(step.Order, Is.EqualTo(5));
            Assert.That(step.ImpersonationUsername, Is.EqualTo("admin"));
            Assert.That(step.PreImage.AllAttributes, Is.False);
            Assert.That(step.PreImage.Attributes, Does.Contain("firstname"));
        });
    }

    [Test]
    public void ReadPlugins_EmptyManifest_YieldsNoPlugins()
    {
        var plugins = PluginManifestReader.ReadPlugins(@"{""plugins"":[],""workflows"":[],""customApis"":[]}");
        Assert.That(plugins, Is.Empty);
    }

    // ── Workflows / Custom APIs ──────────────────────────────────────────────

    private const string WorkflowAndApiManifest = @"
{
  ""plugins"": [],
  ""workflows"": [ { ""fullName"": ""My.Ns.MyWf"", ""displayName"": ""Mon WF"" } ],
  ""customApis"": [
    {
      ""fullName"": ""My.Ns.MyApi"", ""name"": ""my_api"", ""displayName"": ""My Api"", ""description"": ""desc"",
      ""bindingType"": ""Global"", ""boundEntityLogicalName"": """", ""isFunction"": true, ""isPrivate"": false,
      ""allowedCustomProcessing"": ""AsyncOnly"", ""executePrivilegeName"": """", ""workflowSdkStepEnabled"": false,
      ""arguments"": [
        { ""isInArgument"": true,  ""name"": ""inText"",  ""typeFullName"": ""System.String"", ""isEnum"": false, ""isOptional"": true },
        { ""isInArgument"": false, ""name"": ""OutNum"",  ""typeFullName"": ""System.Int32"",  ""isEnum"": false, ""isOptional"": false },
        { ""isInArgument"": true,  ""name"": ""InColor"", ""typeFullName"": ""My.Ns.Color"",   ""isEnum"": true,  ""isOptional"": false }
      ]
    }
  ]
}";

    [Test]
    public void ReadWorkflows_MapsWorkflowWithDisplayName()
    {
        var workflow = PluginManifestReader.ReadWorkflows(WorkflowAndApiManifest).Single();
        Assert.Multiple(() =>
        {
            Assert.That(workflow.FullName, Is.EqualTo("My.Ns.MyWf"));
            Assert.That(workflow.DisplayName, Is.EqualTo("Mon WF"));
            Assert.That(workflow.IsWorkflow, Is.True);
        });
    }

    [Test]
    public void ReadCustomApis_MapsApiHeaderAndUniqueName()
    {
        var api = PluginManifestReader.ReadCustomApis(WorkflowAndApiManifest, "new").Single();
        Assert.Multiple(() =>
        {
            Assert.That(api.Name, Is.EqualTo("my_api"));
            Assert.That(api.Prefix, Is.EqualTo("new"));
            Assert.That(api.UniqueName, Is.EqualTo("new_my_api"));
            Assert.That(api.IsFunction, Is.True);
            Assert.That(api.BindingType.Value, Is.EqualTo(0));               // Global
            Assert.That(api.AllowedCustomProcessingStepType.Value, Is.EqualTo(1)); // AsyncOnly
        });
    }

    [Test]
    public void ReadCustomApis_MapsArgumentsWithTypeAndDirection()
    {
        var api = PluginManifestReader.ReadCustomApis(WorkflowAndApiManifest, "new").Single();
        var args = api.Children.OfType<ICustomApiComponent>().ToList();

        var input = args.Single(a => a.Name == "inText");
        var output = args.Single(a => a.Name == "OutNum");
        var enumArg = args.Single(a => a.Name == "InColor");

        Assert.Multiple(() =>
        {
            Assert.That(input, Is.TypeOf<CustomApiRequestParameter>());
            Assert.That(input.Type.Value, Is.EqualTo(10));   // String
            Assert.That(input.IsOptional, Is.True);
            Assert.That(input.UniqueName, Is.EqualTo("my_api.inText"));

            Assert.That(output, Is.TypeOf<CustomApiResponseProperty>());
            Assert.That(output.Type.Value, Is.EqualTo(7));   // Integer

            Assert.That(enumArg.Type.Value, Is.EqualTo(9));  // Picklist (isEnum)
        });
    }

    // ── Lecture du const depuis un assembly réel (chaîne complète) ───────────

    [Test]
    public void ReadManifestJson_ReadsConstFromAssembly_WithoutInstantiation()
    {
        // Le type Generated.PluginManifest ci-dessous simule la sortie du générateur,
        // embarquée dans cet assembly de test.
        var assembly = typeof(global::XrmFramework.Generated.PluginManifest).Assembly;

        var json = PluginManifestReader.ReadManifestJson(assembly);
        Assert.That(json, Is.EqualTo(global::XrmFramework.Generated.PluginManifest.Json));

        var plugins = PluginManifestReader.ReadPlugins(json);
        Assert.That(plugins.Single().FullName, Is.EqualTo("Emb.MyPlugin"));
    }
}
