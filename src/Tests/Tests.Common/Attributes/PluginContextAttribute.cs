// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;

namespace Plugins.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class PluginContextAttribute : Attribute
    {
        public PluginContextAttribute(Stages stage, string messageName, Modes mode, string entityName)
        {
            Message = Messages.GetMessage(messageName);
            Stage = stage;
            Mode = mode;
            EntityName = entityName;
        }

        public Messages Message { get; set; }
        public Stages Stage { get; set; }
        public Modes Mode { get; set; }
        public string EntityName { get; set; }
    }
}
