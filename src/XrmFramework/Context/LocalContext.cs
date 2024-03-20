// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Xrm.Sdk;
using XrmFramework.Definitions;

namespace XrmFramework
{
    public partial class LocalContext : IContext, IServiceContext
    {
        private IOrganizationService _adminService;

        private readonly EntityReference _businessUnitRef;

        public EntityReference UserRef => new(SystemUserDefinition.EntityName, UserId);

        public EntityReference BusinessUnitRef => _businessUnitRef;

        protected IObjectContainer ObjectContainer { get; }

        protected LocalContext()
        {
            ObjectContainer = new ObjectContainer();

            ObjectContainer.RegisterInstanceAs(this, typeof(IServiceContext));

            InternalDependencyProvider.RegisterDefaults(ObjectContainer);
        }

        protected ILogger Logger { get; }

        public Guid UserId => ExecutionContext.UserId;

        public Guid InitiatingUserId => ExecutionContext.InitiatingUserId;

        public Guid CorrelationId => ExecutionContext.CorrelationId;

        public string OrganizationName => ExecutionContext.OrganizationName;

        //public Guid CorrelationId => ExecutionContext.CorrelationId;
        
        public LocalContext(IServiceProvider serviceProvider) : this()
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            // Obtain the execution context service from the service provider.
            ExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the tracing service from the service provider.
            TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the Organization Service factory service from the service provider
            Factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            OrganizationService = Factory.CreateOrganizationService(ExecutionContext.UserId);

            _businessUnitRef = new EntityReference("businessunit", ExecutionContext.BusinessUnitId);

            Logger = LoggerFactory.GetLogger(this, TracingService.Trace);
        }

        public LogServiceMethod LogServiceMethod => Logger.LogWithMethodName;

        private IOrganizationServiceFactory Factory { get; set; }

        public IOrganizationService OrganizationService { get; protected set; }

        public IExecutionContext ExecutionContext { get; private set; }

        public ITracingService TracingService { get; protected set; }

        public IOrganizationService AdminOrganizationService => _adminService ??= Factory.CreateOrganizationService(null);

        public Messages MessageName => Messages.GetMessage(ExecutionContext.MessageName);

        public void Log(string message, params object[] formatArgs)
        {
            Logger.Log(message, formatArgs);
        }

        public void LogError(Exception e)
        {
            Logger.LogError(e, "ERROR");
        }

        public void DumpLog() => Logger.DumpLog();

        public void FlushLogs() => FlushLogsInternal();
        
        partial void FlushLogsInternal();

        #region Image Helpers
        public bool HasPreImage(string imageName)
        {
            return ExecutionContext.PreEntityImages.ContainsKey(imageName);
        }
        public virtual Entity GetPreImage(string imageName)
        {
            VerifyPreImage(imageName);
            return ExecutionContext.PreEntityImages[imageName];
        }
        public virtual Entity GetPreImageOrDefault(string imageName)
        {
            if (!ExecutionContext.PreEntityImages.ContainsKey(imageName))
            {
                return null;
            }

            return ExecutionContext.PreEntityImages[imageName];
        }
        public bool HasPostImage(string imageName)
        {
            return ExecutionContext.PostEntityImages.ContainsKey(imageName);
        }
        public virtual Entity GetPostImage(string imageName)
        {
            VerifyPostImage(imageName);
            return ExecutionContext.PostEntityImages[imageName];
        }
        protected void VerifyPreImage(string imageName)
        {
            VerifyImage(ExecutionContext.PreEntityImages, imageName, true);
        }

        protected void VerifyPostImage(string imageName)
        {
            VerifyImage(ExecutionContext.PostEntityImages, imageName, false);
        }

        private void VerifyImage(EntityImageCollection collection, string imageName, bool isPreImage)
        {
            if (!collection.Contains(imageName)
                || collection[imageName] == null)
            {
                throw new ArgumentNullException(imageName, $"{(isPreImage ? "PreImage" : "PostImage")} {imageName} does not exist in this context");
            }
        }
        #endregion

        #region Message/Stage/Mode Helpers
        public virtual bool IsCreate()
        {
            return IsMessage(Messages.Create);
        }

        public virtual bool IsUpdate()
        {
            return IsMessage(Messages.Update);
        }

        public virtual bool IsMessage(Messages message)
        {
            return ExecutionContext.MessageName == message.ToString();
        }

        public virtual bool IsSynchronous()
        {
            return IsMode(Modes.Synchronous);
        }

        public virtual bool IsAsynchronous()
        {
            return IsMode(Modes.Asynchronous);
        }

        private bool IsMode(Modes mode)
        {
            return Mode == mode;
        }
        #endregion

        #region Entitys
        public virtual void DumpSharedVariables()
        {
            Logger.LogCollection(ExecutionContext.SharedVariables);
        }


        public virtual void DumpInputParameters()
        {
            Logger.LogCollection(ExecutionContext.InputParameters, false, "ExtensionData", "Parameters", "RequestId", "RequestName");
        }

        #endregion

        #region Parameters Helpers
        public virtual T GetInputParameter<T>(InputParameters parameterName)
        {
            VerifyInputParameter(parameterName);

            if (typeof(T).IsEnum)
            {
                var value = (OptionSetValue)ExecutionContext.InputParameters[parameterName.ToString()];
                return (T)Enum.ToObject(typeof(T), value.Value);
            }
            else
            {
                return (T)ExecutionContext.InputParameters[parameterName.ToString()];
            }
        }

        public void SetInputParameter<T>(InputParameters parameterName, T parameterValue)
        {
            ExecutionContext.InputParameters[parameterName.ToString()] = parameterValue;
        }

        public virtual T GetOutputParameter<T>(OutputParameters parameterName)
        {
            return (T)ExecutionContext.OutputParameters[parameterName.ToString()];
        }

        public void SetOutputParameter<T>(OutputParameters parameterName, T parameterValue)
        {
            ExecutionContext.OutputParameters[parameterName.ToString()] = parameterValue;
        }

        protected void VerifyInputParameter(InputParameters parameterName)
        {
            if (!ExecutionContext.InputParameters.Contains(parameterName.ToString())
                || ExecutionContext.InputParameters[parameterName.ToString()] == null)
            {
                throw new ArgumentNullException(nameof(parameterName), $"InputParameter {parameterName} does not exist in this context");
            }
        }

        public bool HasSharedVariable(string variableName)
        {
            return ExecutionContext.SharedVariables.ContainsKey(variableName);
        }

        public void SetSharedVariable<T>(string variableName, T value)
        {
            if (typeof(T).IsEnum)
            {
                ExecutionContext.SharedVariables[variableName] = Enum.GetName(typeof(T), value);
            }
            else
            {
                ExecutionContext.SharedVariables[variableName] = value;
            }
        }

        public virtual T GetSharedVariable<T>(string variableName)
        {
            T value = default;

            if (ExecutionContext.SharedVariables.ContainsKey(variableName))
            {
                if (typeof(T).IsEnum)
                {
                    var valueTemp = (string)ExecutionContext.SharedVariables[variableName];

                    value = (T)Enum.Parse(typeof(T), valueTemp);
                }
                else
                {
                    value = (T)ExecutionContext.SharedVariables[variableName];
                }
            }

            return value;
        }

        #endregion


        public Modes Mode
        {
            get
            {
                if (!Enum.IsDefined(typeof(Modes), ExecutionContext.Mode))
                {
                    throw new InvalidPluginExecutionException(string.Format("Mode {0} is not part of modes enum", ExecutionContext.Mode));
                }
                return (Modes)ExecutionContext.Mode;

            }
        }


        public IOrganizationService GetService(Guid userId)
        {
            return Factory.CreateOrganizationService(userId);
        }

        public void LogFields(Entity entity, params string[] fieldNames)
        {
            Logger.LogCollection(entity.Attributes, true, fieldNames);
        }

        public LocalContext ParentLocalContext { get; protected set; }

        public Guid GetInitiatingUserId()
        {
            if (ParentLocalContext != null)
            {
                return ParentLocalContext.GetInitiatingUserId();
            }

            return InitiatingUserId;
        }

        public Guid GetRootUserId()
        {
            if (ParentLocalContext != null)
            {
                return ParentLocalContext.GetRootUserId();
            }

            return UserId;
        }

        public void InvokeMethod(object obj, MethodInfo method)
        {
            var listParamValues = new List<object>();

            foreach (var param in method.GetParameters())
            {
                listParamValues.Add(ObjectContainer.Resolve(param.ParameterType));
            }

            var result = method.Invoke(obj, listParamValues.ToArray());

            if (result is Task taskResult)
            {
                Task.WaitAll(taskResult);
            }
        }
    }
}
