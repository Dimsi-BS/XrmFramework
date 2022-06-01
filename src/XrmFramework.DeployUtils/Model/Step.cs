// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Component of a <see cref="Plugin"/> that defines on which particular input said plugin should be executed
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class Step : BaseCrmComponent
    {
        public Step(string pluginTypeName, Messages message, Stages stage, Modes mode, string entityName)
        {
            PluginTypeName = pluginTypeName;
            Message = message;
            Stage = stage;
            Mode = mode;
            EntityName = entityName;
            PreImage = new StepImage(Message, true, stage)
            {
                FatherStep = this
            };
            PostImage = new StepImage(Message, false, stage)
            {
                FatherStep = this
            };

            if (string.IsNullOrWhiteSpace(EntityName) ||
                (message != Messages.Associate && message != Messages.Disassociate)) return;
            EntityName = string.Empty;
            StepConfiguration.RelationshipName = entityName;
        }

        /// <summary>Full Name of the <see cref="Plugin"/></summary>
        /// <example>Assembly.NameSpace.Plugin</example>
        public string PluginTypeFullName { get; set; }

        /// <summary>Simplified Name of the <see cref="Plugin"/></summary>
        /// <example>Plugin</example>
        public string PluginTypeName { get; set; }

        /// <summary>Message on which to fire</summary>
        public Messages Message { get; }

        /// <summary>Stage on which to fire</summary>
        public Stages Stage { get; }

        /// <summary>Mode of the <see cref="Step"/></summary>
        public Modes Mode { get; }

        /// <summary>Entity the step is bound to</summary>
        public string EntityName { get; }

        /// <summary><inheritdoc cref="XrmFramework.StepConfiguration"/></summary>
        public StepConfiguration StepConfiguration { get; set; } = new();

        public Guid MessageId { get; set; }

        /// <summary>Indicates if there are Filtering Attributes</summary>
        public bool DoNotFilterAttributes { get; set; }

        /// <summary>List of Attributes to Filter on trigger</summary>
        public HashSet<string> FilteringAttributes { get; } = new HashSet<string>();

        /// <summary>PreImage of the Step, may not be used</summary>
        public StepImage PreImage { get; set; }

        /// <summary>PostImage of the Step, may not be used</summary>
        public StepImage PostImage { get; set; }

        /// <summary>Preferred Order of execution</summary>
        public int Order { get; set; }

        /// <summary>Can execute while impersonating a specific user</summary>
        public string ImpersonationUsername { get; set; }

        /// <summary><inheritdoc cref="XrmFramework.StepConfiguration.RegisteredMethods"/></summary>
        public HashSet<string> MethodNames => StepConfiguration.RegisteredMethods;

        /// <summary>Joined string of the <see cref="MethodNames"/></summary>
        public string MethodsDisplayName => string.Join(",", MethodNames);

        /// <summary>Serialized string of <see cref="StepConfiguration"/></summary>
        public string UnsecureConfig => JsonConvert.SerializeObject(StepConfiguration);

        /// <summary>Merge two Steps that trigger on the same event</summary>
        /// <param name="step"></param>
        public void Merge(Step step)
        {
            DoNotFilterAttributes |= step.DoNotFilterAttributes;

            if (DoNotFilterAttributes)
            {
                FilteringAttributes.Clear();
            }
            else
            {
                FilteringAttributes.UnionWith(step.FilteringAttributes);
            }

            PreImage.Merge(step.PreImage);

            PostImage.Merge(step.PostImage);

            MethodNames.UnionWith(step.MethodNames);
        }

        /// <summary>
        /// Description of the <see cref="Step"/><br/>
        /// Is built like this : <br/>
        /// <see cref="PluginTypeName"/> : <see cref="Stage"/> <see cref="Message"/> of <see cref="EntityName"/> (<see cref="MethodsDisplayName"/>)
        /// </summary>
        public string Description => $"{PluginTypeName} : {Stage} {Message} of {EntityName} ({MethodsDisplayName})";

        #region BaseCrmComponent overrides
        public override string UniqueName
        {
            get => $"{PluginTypeFullName}.{Stage}.{Message}.{EntityName}.{MethodsDisplayName}";
            set => _ = value;
        }

        public override IEnumerable<ICrmComponent> Children
        {
            get
            {
                var res = new List<ICrmComponent>();
                if (PreImage.ShouldAppearAsChild) res.Add(PreImage);
                if (PostImage.ShouldAppearAsChild) res.Add(PostImage);
                return res;
            }
        }
        public override void AddChild(ICrmComponent child)
        {
            if (child is not StepImage stepChild) throw new ArgumentException("Step doesn't take this type of children");
            if (stepChild.IsPreImage)
            {
                PreImage = stepChild;
            }
            else
            {
                PostImage = stepChild;
            }
            base.AddChild(child);
        }

        protected override void RemoveChild(ICrmComponent child) { }
        public override string EntityTypeName => SdkMessageProcessingStepDefinition.EntityName;
        public override int Rank => 2;
        public override bool DoAddToSolution => true;
        public override bool DoFetchTypeCode => false;
        #endregion
    }
}
