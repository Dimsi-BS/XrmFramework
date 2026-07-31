// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using XrmFramework;
using XrmFramework.Workflow;

namespace Sample
{
    /// <summary>Public plugin: 2 steps covering images, filtering, order, impersonation, config.</summary>
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

    /// <summary>Internal plugin: must still be inventoried (real-world case from the samples).</summary>
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

    /// <summary>Custom API: binding, allowed processing, in/out arguments (including an integer).</summary>
    [CustomApi(CustomApiBindingType.Entity,
        DisplayName = "Do The Thing",
        Description = "Does the thing",
        BoundEntityLogicalName = "account",
        AllowedCustomProcessing = AllowedCustomProcessingStep.AsyncOnly,
        IsFunction = false)]
    public sealed class DoTheThing : CustomApi
    {
        [CustomApiInput(DisplayName = "The name", IsOptional = true)]
        public CustomApiInArgument<string> Name { get; set; }

        [CustomApiOutput]
        public CustomApiOutArgument<int> Count { get; set; }
    }

    /// <summary>Workflow with an explicit DisplayName.</summary>
    public class GreetingWorkflow : CustomWorkflowActivity
    {
        public GreetingWorkflow() => DisplayName = "Say Hello";
    }

    /// <summary>Workflow without a DisplayName: expected to fall back to the type name.</summary>
    public class NamelessWorkflow : CustomWorkflowActivity
    {
    }

    /// <summary>Non-XrmFramework type: must be ignored by the inventory.</summary>
    public class NotAPlugin
    {
    }
}
