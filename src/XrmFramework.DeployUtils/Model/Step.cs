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
        }

        public string PluginTypeName { get; }
        public string Message { get; }
        public Stages Stage { get; }
        public Modes Mode { get; }
        public string EntityName { get; }

        public string PluginTypeFullName { get; set; }

        public Guid MessageId { get; set; }

        public bool DoNotFilterAttributes { get; set; }

        public List<string> FilteringAttributes { get; } = new List<string>();

        public bool PreImageUsed => Message != "Create" && Message != "Book" && (PreImageAllAttributes || PreImageAttributes.Any());
        public bool PreImageAllAttributes { get; set; }
        public List<string> PreImageAttributes { get; } = new List<string>();

        public string JoinedPreImageAttributes => string.Join(",", PreImageAttributes);

        public bool PostImageUsed => Stage == Stages.PostOperation && (PostImageAllAttributes || PostImageAttributes.Any());
        public bool PostImageAllAttributes { get; set; }
        public List<string> PostImageAttributes { get; } = new List<string>();

        public string JoinedPostImageAttributes => string.Join(",", PostImageAttributes);

        public string UnsecureConfig { get; set; }

        public int Order { get; set; }

        public string ImpersonationUsername { get; set; }

        public List<string> MethodNames { get; } = new List<string>();
        public string MethodsDisplayName => string.Join(",", MethodNames);



        public void Merge(Step step)
        {
            if (!step.FilteringAttributes.Any())
            {
                DoNotFilterAttributes = true;
            }

            FilteringAttributes.AddRange(step.FilteringAttributes);

            if (step.PreImageAllAttributes)
            {
                PreImageAllAttributes = true;
                PreImageAttributes.Clear();
            }
            else
            {
                PreImageAttributes.AddRange(step.PreImageAttributes);
            }

            if (step.PostImageAllAttributes)
            {
                PostImageAllAttributes = true;
                PostImageAttributes.Clear();
            }
            else
            {
                PostImageAttributes.AddRange(step.PostImageAttributes);
            }

            MethodNames.AddRange(step.MethodNames);
        }
    }

}
