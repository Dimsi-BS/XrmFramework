// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Deploy
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

        public string PluginTypeName { get; private set; }

        public string Message { get; private set; }
        public Stages Stage { get; private set; }
        public Modes Mode { get; private set; }
        public string EntityName { get; private set; }

        public Guid MessageId { get; set; }

        public string FilteredAttributes { get; set; }
        public bool PreImageUsed { get; set; }
        public bool PreImageAllAttributes { get; set; }
        public string PreImageAttributes { get; set; }

        public bool PostImageUsed { get; set; }
        public bool PostImageAllAttributes { get; set; }
        public string PostImageAttributes { get; set; }

        public string UnsecureConfig { get; set; }

        public int Order { get; set; }

        public string ImpersonationUsername { get; set; }
    }

    public class StepComparer : IEqualityComparer<Step>
    {
        public bool Equals(Step x, Step y)
        {
            return x.PluginTypeName == y.PluginTypeName && x.EntityName == y.EntityName && x.Message == y.Message && x.Stage == y.Stage && x.Mode == y.Mode && x.UnsecureConfig == y.UnsecureConfig;
        }

        public int GetHashCode(Step obj)
        {
            return obj.PluginTypeName.GetHashCode() + obj.EntityName.GetHashCode() + obj.Message.GetHashCode() + obj.Stage.GetHashCode() + obj.Mode.GetHashCode();
        }
    }
}
