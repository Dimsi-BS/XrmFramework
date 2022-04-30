// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Definitions;

namespace XrmFramework.DeployUtils.Model
{
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


            if (!string.IsNullOrWhiteSpace(EntityName) &&
                (message == Messages.Associate || message == Messages.Disassociate))
            {
                EntityName = string.Empty;

                StepConfiguration.RelationshipName = entityName;
            }
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
        public string PluginTypeName { get; set; }
        public Messages Message { get; }
        public Stages Stage { get; }
        public Modes Mode { get; }
        public string EntityName { get; }

        public Guid ParentId { get; set; }
        public string PluginTypeFullName { get; set; }

        public Guid MessageId { get; set; }

        public bool DoNotFilterAttributes { get; set; }

        public List<string> FilteringAttributes { get; } = new List<string>();

        public StepImage PreImage { get; set; }

        public StepImage PostImage { get; set; }

        public string UnsecureConfig => JsonConvert.SerializeObject(StepConfiguration);

        public int Order { get; set; }

        public string ImpersonationUsername { get; set; }

        public List<string> MethodNames => StepConfiguration.RegisteredMethods;
        public string MethodsDisplayName => string.Join(",", MethodNames);

        public StepConfiguration StepConfiguration { get; set; } = new();

        public RegistrationState RegistrationState { get; set; } = RegistrationState.NotComputed;

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

        public string Description => $"{PluginTypeName} : {Stage} {Message} of {EntityName} ({MethodsDisplayName})";

        public string EntityTypeName => SdkMessageProcessingStepDefinition.EntityName;
        public string UniqueName => $"{PluginTypeFullName}.{Stage}.{Message}.{EntityName}.{MethodsDisplayName}";
        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var res = new List<ICrmComponent>();
                if ((PreImage.IsUsed && PreImage.RegistrationState != RegistrationState.Computed) || PreImage.RegistrationState == RegistrationState.ToDelete) res.Add(PreImage);
                if ((PostImage.IsUsed && PostImage.RegistrationState != RegistrationState.Computed) || PostImage.RegistrationState == RegistrationState.ToDelete) res.Add(PostImage);
                return res;
            }
        }
        public void AddChild(ICrmComponent child)
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
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            if (RegistrationState != state) return;
            var childrenSafe = Children.ToList();
            foreach (var child in childrenSafe)

            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any())
                {
                    child.RegistrationState = RegistrationState.Computed;
                }
            }
        }

        public int Rank => 2;
        public bool DoAddToSolution => true;
        public bool DoFetchTypeCode => false;
    }

}
