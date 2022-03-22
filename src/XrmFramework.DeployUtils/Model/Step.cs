// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.DeployUtils.Model
{
    public class Step
    {
        public Step(string pluginTypeName, string message, Stages stage, Modes mode, string entityName)
        {
            PluginTypeName = pluginTypeName;
            Message = message;
            Stage = stage;
            Mode = mode;
            EntityName = entityName;
            PreImage = new StepImage(Message, true, stage);
            PostImage = new StepImage(Message, false, stage);            
            RegistrationState = RegistrationState.NotComputed;
        }

        public Guid Id { get; set; }
        public string PluginTypeName { get; }
        public string Message { get; }
        public Stages Stage { get; }
        public Modes Mode { get; }
        public string EntityName { get; }

        public Guid PluginId { get; set; }
        public string PluginTypeFullName { get; set; }

        public Guid MessageId { get; set; }

        public bool DoNotFilterAttributes { get; set; }

        public List<string> FilteringAttributes { get; } = new List<string>();

        public StepImage PreImage  { get; set; }

        public StepImage PostImage { get; set; }

        public string UnsecureConfig { get; set; }

        public int Order { get; set; }

        public string ImpersonationUsername { get; set; }

        public List<string> MethodNames { get; } = new List<string>();
        public string MethodsDisplayName => string.Join(",", MethodNames);

        public RegistrationState RegistrationState { get; set; }


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
    }

}
