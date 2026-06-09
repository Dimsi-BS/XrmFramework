// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// Types « stub » net8 reproduisant la FORME des types XrmFramework lus par le moteur d'inventaire
// (réflexion par nom). On ne stube QUE ce qui est absent de DeployUtils : Stages/Modes/Messages et
// les attributs cœur proviennent du vrai XrmFramework (référencé via DeployUtils).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace XrmFramework
{
    /// <summary>Stub de <c>XrmFramework.Step</c> : porte exactement les propriétés lues par le moteur.</summary>
    public sealed class Step
    {
        public Messages Message { get; set; }
        public Stages Stage { get; set; }
        public Modes Mode { get; set; }
        public string EntityName { get; set; }
        public MethodInfo Method { get; set; }
        public List<string> MethodNames { get; } = new();
        public List<string> FilteringAttributes { get; } = new();
        public bool PreImageAllAttributes { get; set; }
        public List<string> PreImageAttributes { get; } = new();
        public bool PostImageAllAttributes { get; set; }
        public List<string> PostImageAttributes { get; } = new();
        public int Order { get; set; } = 1;
        public string ImpersonationUsername { get; set; } = "";
        public string UnsecureConfig { get; set; }
    }

    /// <summary>Stub de <c>XrmFramework.Plugin</c> : le ctor déclenche AddSteps comme le vrai.</summary>
    public abstract class Plugin
    {
        private readonly List<Step> _steps = new();

        protected Plugin(string unsecuredConfig, string securedConfig)
        {
            AddSteps();
        }

        protected abstract void AddSteps();

        public ReadOnlyCollection<Step> Steps => new(_steps);

        protected void AddStep(Step step) => _steps.Add(step);
    }

    public enum CustomApiBindingType { Global = 0, Entity = 1, EntityCollection = 2 }

    public enum AllowedCustomProcessingStep { None = 0, AsyncOnly = 1, SyncAndAsync = 2 }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomApiAttribute : Attribute
    {
        public CustomApiAttribute(CustomApiBindingType bindingType) => BindingType = bindingType;

        public CustomApiBindingType BindingType { get; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string BoundEntityLogicalName { get; set; }
        public bool IsFunction { get; set; }
        public bool IsPrivate { get; set; }
        public AllowedCustomProcessingStep AllowedCustomProcessing { get; set; }
        public string ExecutePrivilegeName { get; set; }
        public bool WorkflowSdkStepEnabled { get; set; }
    }

    public abstract class CustomApiArgumentAttribute : Attribute
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string LogicalEntityName { get; set; }
        public bool IsOptional { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CustomApiInputAttribute : CustomApiArgumentAttribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CustomApiOutputAttribute : CustomApiArgumentAttribute { }

    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiInArgument<T> { }

    // ReSharper disable once UnusedTypeParameter
    public sealed class CustomApiOutArgument<T> { }

    /// <summary>Stub de <c>XrmFramework.CustomApi</c> : le moteur ne l'instancie pas (réflexion de type seule).</summary>
    public abstract class CustomApi : Plugin
    {
        protected CustomApi() : base(null, null) { }

        protected override void AddSteps() { }
    }
}

namespace XrmFramework.Workflow
{
    /// <summary>Stub de <c>XrmFramework.Workflow.CustomWorkflowActivity</c> (DisplayName lu par le moteur).</summary>
    public abstract class CustomWorkflowActivity
    {
        public string DisplayName { get; set; }
    }
}
