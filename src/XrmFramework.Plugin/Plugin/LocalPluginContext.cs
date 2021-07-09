// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace XrmFramework
{
    internal partial class LocalPluginContext : LocalContext, IPluginContext, ICustomApiContext
    {
        public LocalPluginContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentPluginContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }
        }

        private LocalPluginContext(LocalPluginContext context, IPluginExecutionContext parentContext)
            : base(context, parentContext)
        {
            if (PluginExecutionContext.ParentContext != null)
            {
                ParentPluginContext = new LocalPluginContext(this, PluginExecutionContext.ParentContext);
            }
        }

        private IPluginExecutionContext PluginExecutionContext => (IPluginExecutionContext)ExecutionContext;

        public bool IsPreOperation()
        {
            return IsStage(Stages.PreOperation);
        }

        public bool IsPreValidation()
        {
            return IsStage(Stages.PreValidation);
        }

        public bool IsPostOperation()
        {
            return IsStage(Stages.PostOperation);
        }

        public bool IsStage(Stages stage)
        {
            return PluginExecutionContext.Stage == (int)stage;
        }

        public string PrimaryEntityName => PluginExecutionContext.PrimaryEntityName;

        public Guid PrimaryEntityId => PluginExecutionContext.PrimaryEntityId;

        public int Stage => PluginExecutionContext.Stage;

        public Guid OrganizationId => PluginExecutionContext.OrganizationId;

        public override int Depth => PluginExecutionContext.Depth;

        public ParameterCollection SharedVariables => PluginExecutionContext.SharedVariables;

        public IPluginContext ParentContext => ParentPluginContext;

        public LocalPluginContext ParentPluginContext { get; }

        public bool IsMultiplePrePostOperation
        {
            get
            {
                if (ParentContext != null && (IsCreate() || IsUpdate()))
                {
                    var currentTarget = GetInputParameter<Entity>(InputParameters.Target);
                    var parentTarget = ParentContext.GetInputParameter<Entity>(InputParameters.Target);

                    return parentTarget.Attributes.Count != currentTarget.Attributes.Count;
                }
                return false;
            }
        }

        public EntityReference ObjectRef => throw new NotImplementedException();
        
        public T GetArgumentValue<T>(CustomApiInArgument<T> argument)
        {
            if (!PluginExecutionContext.InputParameters.TryGetValue(argument.ArgumentName, out var argumentValue) || argumentValue == null)
            {
                return default;
            }
            
            switch (argument.ArgumentType)
            {

                case CustomApiArgumentType.String:
                    if (argument.IsSerializedArgument)
                    {
                        return JsonConvert.DeserializeObject<T>((string)argumentValue);
                    }
                    else
                    {
                        return (T)argumentValue;
                    }
                default:
                    return (T)argumentValue;
            }
        }

        public void SetArgumentValue<T>(CustomApiOutArgument<T> argument, T value)
        {
            switch (argument.ArgumentType)
            {
                case CustomApiArgumentType.String:
                    if (argument.IsSerializedArgument)
                    {
                        PluginExecutionContext.OutputParameters[argument.ArgumentName] = JsonConvert.SerializeObject(value);
                    }
                    else
                    {
                        PluginExecutionContext.OutputParameters[argument.ArgumentName] = value;
                    }
                    break;
                default:
                    PluginExecutionContext.OutputParameters[argument.ArgumentName] = value;
                    break;
            }
        }

        public bool HasArgument<T>(CustomApiInArgument<T> argument)
            => PluginExecutionContext.InputParameters.ContainsKey(argument.ArgumentName);

        public bool HasArgument<T>(CustomApiOutArgument<T> argument)
            => PluginExecutionContext.OutputParameters.ContainsKey(argument.ArgumentName);

    }

}
