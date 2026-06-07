// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace XrmFramework.PluginManifest.Generator.Tests;

/// <summary>
/// Compile un stub minimal du framework + un code plugin, exécute le générateur,
/// et retourne le manifeste JSON (lu depuis le const généré) + les diagnostics.
/// </summary>
internal static class GeneratorTestHelper
{
    /// <summary>Stub minimal des types du framework que le générateur recherche par nom.</summary>
    public const string Framework = @"
using System;
namespace XrmFramework
{
    public enum Stages { PreValidation = 10, PreOperation = 20, PostOperation = 40 }
    public enum Modes { Synchronous, Asynchronous }

    // Messages est une CLASSE à membres statiques (et non un enum), comme dans le framework réel.
    public class Messages
    {
        public string MessageName { get; }
        private Messages(string n) { MessageName = n; }
        public static Messages Create { get; } = new Messages(""Create"");
        public static Messages Update { get; } = new Messages(""Update"");
        public static Messages Delete { get; } = new Messages(""Delete"");
        public static Messages GetMessage(string messageName) => new Messages(messageName);
    }
    public enum CustomApiBindingType { Global, Entity, EntityCollection }
    public enum AllowedCustomProcessingStep { None, AsyncOnly, SyncAndAsync }

    public class Plugin
    {
        protected Plugin() {}
        protected Plugin(string a, string b) {}
        protected virtual void AddSteps() {}
        protected void AddStep(Stages stage, Messages message, Modes mode, string entityName, string actionName, params string[] columns) {}
    }

    public class CustomApiInArgument<T> { }
    public class CustomApiOutArgument<T> { }

    public abstract class CustomApi : Plugin
    {
        protected CustomApi(string methodName) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ImageAttribute : Attribute { public ImageAttribute(params string[] c){} public ImageAttribute(bool a){} }
    public class PreImageAttribute : ImageAttribute { public PreImageAttribute(params string[] c):base(c){} public PreImageAttribute(bool a):base(a){} }
    public class PostImageAttribute : ImageAttribute { public PostImageAttribute(params string[] c):base(c){} public PostImageAttribute(bool a):base(a){} }
    [AttributeUsage(AttributeTargets.Method)] public class FilteringAttributesAttribute : Attribute { public FilteringAttributesAttribute(params string[] a){} }
    [AttributeUsage(AttributeTargets.Method)] public class ExecutionOrderAttribute : Attribute { public ExecutionOrderAttribute(int order){} }
    [AttributeUsage(AttributeTargets.Method)] public class ImpersonationAttribute : Attribute { public ImpersonationAttribute(string u){} }
    [AttributeUsage(AttributeTargets.Method)] public class UnsecureConfigAttribute : Attribute { public UnsecureConfigAttribute(string c){} public UnsecureConfigAttribute(Type t, string p){} }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomApiAttribute : Attribute
    {
        public string? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public CustomApiBindingType BindingType { get; }
        public string? BoundEntityLogicalName { get; set; }
        public bool IsFunction { get; set; }
        public bool IsPrivate { get; set; }
        public AllowedCustomProcessingStep AllowedCustomProcessing { get; set; }
        public string? ExecutePrivilegeName { get; set; }
        public bool WorkflowSdkStepEnabled { get; set; }
        public CustomApiAttribute(CustomApiBindingType bindingType) { BindingType = bindingType; }
    }
    [AttributeUsage(AttributeTargets.Property)] public abstract class CustomApiArgumentAttribute : Attribute
    { public string? Name { get; set; } public string? DisplayName { get; set; } public string? Description { get; set; } public string? LogicalEntityName { get; set; } public bool IsOptional { get; set; } }
    public sealed class CustomApiInputAttribute : CustomApiArgumentAttribute {}
    public sealed class CustomApiOutputAttribute : CustomApiArgumentAttribute {}
}
namespace XrmFramework.Workflow { public abstract class CustomWorkflowActivity { protected void SetDisplayName(string displayName) {} } }
";

    public static (string Json, ImmutableArray<Diagnostic> Diagnostics) Run(string pluginsSource)
    {
        var refs = ((string)System.AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(System.IO.Path.PathSeparator)
            .Where(p => p.Length > 0)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        var compilation = CSharpCompilation.Create(
            "Sample.Plugins",
            new[]
            {
                CSharpSyntaxTree.ParseText(Framework),
                CSharpSyntaxTree.ParseText(pluginsSource),
            },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new PluginManifestGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);

        var manifestType = output.GetTypeByMetadataName("XrmFramework.Generated.PluginManifest");
        var json = manifestType?
            .GetMembers("Json")
            .OfType<IFieldSymbol>()
            .FirstOrDefault()?
            .ConstantValue as string ?? "";

        return (json, driver.GetRunResult().Diagnostics);
    }
}
