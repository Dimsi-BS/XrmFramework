// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Activities;
using System.Configuration;
using Microsoft.Xrm.Sdk.Workflow;
using XrmFramework;
using XrmFramework.Workflow;
using XrmFramework.Workflow.EnableRuleProvider;

namespace XrmFramework.PluginInventory.Fixtures
{
    /// <summary>
    ///     Plugin whose steps are registered the way real ones are: a loop, attributes on the step
    ///     methods for images, filtering and order. The inventory has to report what the constructor
    ///     actually produced, not what a static reading of the source might guess.
    /// </summary>
    public class AccountPlugin : Plugin
    {
        public AccountPlugin(string unsecuredConfig, string securedConfig)
            : base(unsecuredConfig, securedConfig)
        {
        }

        protected override void AddSteps()
        {
            AddStep(Stages.PreOperation, Messages.Create, Modes.Synchronous, "account", nameof(OnCreate));
            AddStep(Stages.PostOperation, Messages.Update, Modes.Asynchronous, "account", nameof(OnUpdate));

            // Registered in a loop: the whole reason the inventory executes the code instead of
            // reading the source. A static analyser sees one AddStep; the manifest must hold three.
            foreach (var entityName in new[] { "contact", "lead", "opportunity" })
            {
                AddStep(Stages.PreValidation, Messages.Delete, Modes.Synchronous, entityName, nameof(OnDelete));
            }
        }

        [PreImage("name", "accountnumber")]
        [PostImage(true)]
        public void OnCreate()
        {
        }

        [FilteringAttributes("name", "telephone1")]
        [ExecutionOrder(42)]
        public void OnUpdate()
        {
        }

        public void OnDelete()
        {
        }
    }

    /// <summary>
    ///     Custom Workflow Activity deriving from the REAL base class, with a display name computed in
    ///     the constructor. Nothing but running that constructor can produce the value, which is why
    ///     the inventory instantiates activities rather than reading their metadata - and why the host
    ///     has to provide a System.Activities the activity can load against.
    /// </summary>
    public sealed class ComputeScoreActivity : CustomWorkflowActivity
    {
        public ComputeScoreActivity()
        {
            SetAction(nameof(ComputeScore));
            SetDisplayName("Compute " + Target + " score");
        }

        private const string Target = "account";

        [RequiredArgument]
        [Input("Account name")]
        public InArgument<string> AccountName { get; set; }

        [Output("Score")]
        public OutArgument<int> Score { get; set; }

        public void ComputeScore()
        {
        }
    }

    /// <summary>
    ///     Activity that never sets a display name: expected to be reported under its type name, the
    ///     same value .NET Framework's System.Activities would resolve on its own.
    /// </summary>
    public sealed class NamelessActivity : CustomWorkflowActivity
    {
        public NamelessActivity() => SetAction(nameof(DoNothing));

        public void DoNothing()
        {
        }
    }

    /// <summary>
    ///     Activity overriding <c>CacheMetadata</c>, so the stand-in's <c>CodeActivityMetadata</c> is
    ///     exercised: the parameter type is baked into this method's signature, so the type has to
    ///     load for the activity to load at all.
    /// </summary>
    public sealed class CachingActivity : CustomWorkflowActivity
    {
        public CachingActivity()
        {
            SetAction(nameof(Run));
            SetDisplayName("Caching activity");
        }

        public void Run()
        {
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
        }
    }

    /// <summary>Custom API with typed in/out arguments, including an enum.</summary>
    [CustomApi(CustomApiBindingType.Entity,
        DisplayName = "Recompute Score",
        Description = "Recomputes the score of an account",
        BoundEntityLogicalName = "account",
        AllowedCustomProcessing = AllowedCustomProcessingStep.AsyncOnly,
        IsFunction = false)]
    public sealed class RecomputeScore : CustomApi
    {
        public RecomputeScore() : base(nameof(Execute))
        {
        }

        public void Execute()
        {
        }

        [CustomApiInput(DisplayName = "The reason", IsOptional = true)]
        public CustomApiInArgument<string> Reason { get; set; }

        [CustomApiInput]
        public CustomApiInArgument<ScoreKind> Kind { get; set; }

        [CustomApiOutput]
        public CustomApiOutArgument<int> Score { get; set; }
    }

    /// <summary>Enum argument type, reported with <c>isEnum</c> in the manifest.</summary>
    public enum ScoreKind
    {
        Simple = 0,
        Weighted = 1,
    }

    /// <summary>
    ///     Configuration type compiled into a plugin assembly. It forwards to
    ///     System.Configuration.ConfigurationManager, which a .NET-modern host only has because the
    ///     inventory host references the package: without it this type fails to load, GetTypes()
    ///     throws, and the inventory now reports that as an error. Nothing reads it - its presence is
    ///     the test.
    /// </summary>
    public sealed class ScoringSection : ConfigurationSection
    {
        [ConfigurationProperty("threshold", DefaultValue = 10)]
        public int Threshold => (int)this["threshold"];
    }

    /// <summary>
    ///     Forces <c>EnableRulesActivityBase</c> to load, and with it the framework's own
    ///     <c>InArgument&lt;string&gt;</c> / <c>OutArgument&lt;string&gt;</c> auto-properties.
    /// </summary>
    /// <remarks>
    ///     Worth its own type because <c>XrmFramework.Workflows</c> ships as SOURCE, not as a
    ///     library: in a real plugin project <c>EnableRulesActivityBase</c> is compiled straight into
    ///     the plugin assembly, so the generic argument types have to be loadable even for a plugin
    ///     whose own activities declare no arguments. Here the base lives in the referenced assembly,
    ///     so deriving from it is what makes the inventory's GetTypes() pull it in.
    /// </remarks>
    public sealed class EnableRulesFixture : EnableRulesActivityBase
    {
    }

    /// <summary>Plain type: must be ignored by the inventory.</summary>
    public class NotAPlugin
    {
    }
}
