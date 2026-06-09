// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using XrmFramework;
using XrmFramework.Workflow;

namespace Sample
{
    /// <summary>Plugin public : 2 steps couvrant images, filtering, ordre, impersonation, config.</summary>
    public class AccountPlugin : Plugin
    {
        public AccountPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig) { }

        protected override void AddSteps()
        {
            var create = new Step
            {
                Message = Messages.Create,
                Stage = Stages.PreOperation,
                Mode = Modes.Synchronous,
                EntityName = "account",
                Method = GetType().GetMethod(nameof(OnCreate)),
                Order = 1,
                PostImageAllAttributes = true,
            };
            create.MethodNames.Add(nameof(OnCreate));
            create.PreImageAttributes.Add("name");
            create.PreImageAttributes.Add("accountnumber");
            AddStep(create);

            var update = new Step
            {
                Message = Messages.Update,
                Stage = Stages.PostOperation,
                Mode = Modes.Asynchronous,
                EntityName = "account",
                Method = GetType().GetMethod(nameof(OnUpdate)),
                Order = 5,
                ImpersonationUsername = "admin",
                UnsecureConfig = "{\"bannedMethods\":[\"Foo\"]}",
            };
            update.MethodNames.Add(nameof(OnUpdate));
            update.FilteringAttributes.Add("name");
            AddStep(update);
        }

        public void OnCreate() { }
        public void OnUpdate() { }
    }

    /// <summary>Plugin internal : doit tout de même être inventorié (cas réel des samples).</summary>
    internal class HiddenPlugin : Plugin
    {
        public HiddenPlugin(string unsecuredConfig, string securedConfig) : base(unsecuredConfig, securedConfig) { }

        protected override void AddSteps()
        {
            var step = new Step
            {
                Message = Messages.Delete,
                Stage = Stages.PreValidation,
                Mode = Modes.Synchronous,
                EntityName = "contact",
                Method = GetType().GetMethod(nameof(OnDelete)),
            };
            step.MethodNames.Add(nameof(OnDelete));
            AddStep(step);
        }

        public void OnDelete() { }
    }

    /// <summary>Custom API : binding, traitement autorisé, arguments in/out (dont entier).</summary>
    [CustomApi(CustomApiBindingType.Entity,
        DisplayName = "Do The Thing",
        Description = "Fait la chose",
        BoundEntityLogicalName = "account",
        AllowedCustomProcessing = AllowedCustomProcessingStep.AsyncOnly,
        IsFunction = false)]
    public sealed class DoTheThing : CustomApi
    {
        [CustomApiInput(DisplayName = "Le nom", IsOptional = true)]
        public CustomApiInArgument<string> Name { get; set; }

        [CustomApiOutput]
        public CustomApiOutArgument<int> Count { get; set; }
    }

    /// <summary>Workflow avec DisplayName explicite.</summary>
    public class GreetingWorkflow : CustomWorkflowActivity
    {
        public GreetingWorkflow() => DisplayName = "Say Hello";
    }

    /// <summary>Workflow sans DisplayName : repli attendu sur le nom du type.</summary>
    public class NamelessWorkflow : CustomWorkflowActivity
    {
    }

    /// <summary>Type non-XrmFramework : doit être ignoré par l'inventaire.</summary>
    public class NotAPlugin
    {
    }
}
