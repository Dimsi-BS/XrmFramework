// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace XrmFramework
{
    [DebuggerNonUserCode]
    public class Step
    {
        private List<string> _filteringAttributes = new List<string>();

        private bool _doNotFilterAttributes;

        private List<string> _preImageAttributes = new List<string>();

        private bool _preImageAllAttributes = false;

        private List<string> _postImageAttributes = new List<string>();

        private bool _postImageAllAttributes = false;

        public Step(Plugin plugin, Messages message, Stages stage, Modes mode, string entityName)
        {
            Plugin = plugin;
            Message = message;
            Stage = stage;
            Mode = mode;
            EntityName = entityName;
            Order = 1;
        }

        public Step(Plugin plugin, Messages message, Stages stage, Modes mode, string entityName, string methodName, params string[] columns)
            : this(plugin, message, stage, mode, entityName)
        {
            Method = plugin.GetType().GetMethod(methodName);

            if (Method != null)
            {
                MethodNames.Add(Method.Name);

                var filteringAttributes = Method.GetCustomAttribute<FilteringAttributesAttribute>();
                if (filteringAttributes != null)
                {
                    _filteringAttributes.AddRange(filteringAttributes.Attributes);
                }
                else if (columns != null)
                {
                    _filteringAttributes.AddRange(columns);
                }

                var preImageAttribute = Method.GetCustomAttribute<PreImageAttribute>();
                if (preImageAttribute != null)
                {
                    _preImageAllAttributes = preImageAttribute.AllColumns;
                    if (!_preImageAllAttributes)
                    {
                        _preImageAttributes.AddRange(preImageAttribute.Columns);
                    }
                }
                else if (columns != null)
                {
                    _preImageAttributes.AddRange(columns);
                }

                var postImageAttribute = Method.GetCustomAttribute<PostImageAttribute>();
                if (postImageAttribute != null)
                {
                    _postImageAllAttributes = postImageAttribute.AllColumns;
                    if (!_postImageAllAttributes)
                    {
                        _postImageAttributes.AddRange(postImageAttribute.Columns);
                    }
                }
                else if (columns != null)
                {
                    _postImageAttributes.AddRange(columns);
                }

                var executionOrderAttribute = Method.GetCustomAttribute<ExecutionOrderAttribute>();
                if (executionOrderAttribute != null)
                {
                    Order = executionOrderAttribute.Order;
                }

                var impersonationUserAttribute = Method.GetCustomAttribute<ImpersonationAttribute>();
                if (impersonationUserAttribute != null)
                {
                    ImpersonationUsername = impersonationUserAttribute.ImpersonationUsername;
                }

                var unsecureConfigAttribute = Method.GetCustomAttribute<UnsecureConfigAttribute>();
                if (unsecureConfigAttribute != null)
                {
                    if (!string.IsNullOrEmpty(unsecureConfigAttribute.UnsecureConfig))
                    {
                        UnsecureConfig = unsecureConfigAttribute.UnsecureConfig;
                    }
                    else
                    {
                        var resourceType = unsecureConfigAttribute.ResourceType;

                        UnsecureConfig = resourceType.GetProperty(unsecureConfigAttribute.PropertyName).GetValue(null) as string;
                    }
                }
            }
        }

        public Plugin Plugin { get; }
        public Messages Message { get; }
        public Stages Stage { get; }
        public Modes Mode { get; }
        public string EntityName { get; }

        public MethodInfo Method { get; }

        public List<string> MethodNames { get; } = new List<string>();

        public string UnsecureConfig { get; }

        public Guid MessageId { get; set; }


        public IReadOnlyCollection<string> FilteringAttributes => _filteringAttributes;

        public bool PreImageAllAttributes => _preImageAllAttributes;
        public IReadOnlyCollection<string> PreImageAttributes => _preImageAttributes;


        public bool PostImageAllAttributes => _postImageAllAttributes;
        public IReadOnlyCollection<string> PostImageAttributes => _postImageAttributes;

        public int Order { get; }

        public string ImpersonationUsername { get; }
    }

    public class StepComparer : IEqualityComparer<Step>
    {
        public bool Equals(Step x, Step y)
        {
            return x.Plugin == y.Plugin && x.EntityName == y.EntityName && x.Message == y.Message && x.Stage == y.Stage && x.Mode == y.Mode;
        }

        public int GetHashCode(Step obj)
        {
            return obj.Plugin.GetHashCode() + obj.EntityName.GetHashCode() + obj.Message.GetHashCode() + obj.Stage.GetHashCode() + obj.Mode.GetHashCode();
        }
    }
}
