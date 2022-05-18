// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
    /// <summary>
    /// Component of a <see cref="Plugin"/> that defines on which particular input said plugin should be executed
    /// </summary>
    /// <seealso cref="XrmFramework.DeployUtils.Model.ICrmComponent" />
    public class Step : ICrmComponent
    {
        private Guid _id = Guid.NewGuid();

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

        public Guid Id
        {
            get => _id;
            set
            {
                PreImage.ParentId = value;
                PostImage.ParentId = value;
                _id = value;
            }
        }
        public Guid ParentId { get; set; }

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
        public List<string> FilteringAttributes { get; } = new List<string>();

        /// <summary>PreImage of the Step, may not be used</summary>
        public StepImage PreImage { get; set; }

        /// <summary>PostImage of the Step, may not be used</summary>
        public StepImage PostImage { get; set; }

        /// <summary>Preferred Order of execution</summary>
        public int Order { get; set; }

        /// <summary>Can execute while impersonating a specific user</summary>
        public string ImpersonationUsername { get; set; }

        /// <summary><inheritdoc cref="XrmFramework.StepConfiguration.RegisteredMethods"/></summary>
        public List<string> MethodNames => StepConfiguration.RegisteredMethods;

        /// <summary>Joined string of the <see cref="MethodNames"/></summary>
        public string MethodsDisplayName => string.Join(",", MethodNames);

        /// <summary>Serialized string of <see cref="StepConfiguration"/></summary>
        public string UnsecureConfig => JsonConvert.SerializeObject(StepConfiguration);


        /// <summary>Merge two Steps that trigger on the same event</summary>
        /// <param name="step"></param>
        public void Merge(Step step)
        {
            if (!step.FilteringAttributes.Any())
            {
                DoNotFilterAttributes = true;
            }

            FilteringAttributes.AddRange(step.FilteringAttributes);

            if (step.PreImage.AllAttributes)
            {
                PreImage.AllAttributes = true;
                PreImage.Attributes.Clear();
            }
            else
            {
                PreImage.Attributes.AddRange(step.PreImage.Attributes);
            }

            if (step.PostImage.AllAttributes)
            {
                PostImage.AllAttributes = true;
                PostImage.Attributes.Clear();
            }
            else
            {
                PostImage.Attributes.AddRange(step.PostImage.Attributes);
            }

            MethodNames.AddRange(step.MethodNames);
        }

        /// <summary>
        /// Description of the <see cref="Step"/><br/>
        /// Is built like this : <br/>
        /// <see cref="PluginTypeName"/> : <see cref="Stage"/> <see cref="Message"/> of <see cref="EntityName"/> (<see cref="MethodsDisplayName"/>)
        /// </summary>
        public string Description => $"{PluginTypeName} : {Stage} {Message} of {EntityName} ({MethodsDisplayName})";

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;
        public string EntityTypeName => SdkMessageProcessingStepDefinition.EntityName;
        public string UniqueName => $"{PluginTypeFullName}.{Stage}.{Message}.{EntityName}.{MethodsDisplayName}";
        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var res = new List<ICrmComponent>();
                if (PreImage.ShouldAppearAsChild) res.Add(PreImage);
                if (PostImage.ShouldAppearAsChild) res.Add(PostImage);
                return res;
            }
        }
        public void AddChild(ICrmComponent child)
        {
            if (child is not StepImage stepChild) throw new ArgumentException("Step doesn't take this type of children");
            if (stepChild.IsPreImage)
            {
                PreImage.Id = _id;
                PreImage = stepChild;
            }
            else
            {
                PostImage.Id = _id;
                PostImage = stepChild;
            }
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            var childrenWithStateSafe = Children
                .Where(c => c.RegistrationState == state)
                .ToList();
            foreach (var child in childrenWithStateSafe)
            {

                child.RegistrationState = RegistrationState.Computed;

            }
        }

        public int Rank => 2;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => false;
    }

}
