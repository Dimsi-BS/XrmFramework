// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Xrm.Sdk;
using Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Plugins.Tests
{
    [ExcludeFromCodeCoverage]
    public class MockPluginContext : IPluginContext
    {
        private Messages _message;
        private Stages _stage;
        private Modes _mode;
        private string _entityName;
        private Guid _userId = Guid.NewGuid();
        private Guid _entityId = Guid.NewGuid();
        private EntityReference _businessUnitRef = new EntityReference("businessunit", Guid.NewGuid());
        private Guid _initiatingUserId = Guid.NewGuid();
        private readonly Guid _organizationId = Guid.NewGuid();


        private IDictionary<InputParameters, object> _inputParameters = new Dictionary<InputParameters, object>();
        private IDictionary<OutputParameters, object> _outputParameters = new Dictionary<OutputParameters, object>();

        private IDictionary<string, Entity> _preImages = new Dictionary<string, Entity>();
        private IDictionary<string, Entity> _postImages = new Dictionary<string, Entity>();

        public MockPluginContext(Messages message, Stages stage, Modes mode, string entityName)
        {
            _message = message;
            _stage = stage;
            _mode = mode;
            _entityName = entityName;
            CorrelationId = Guid.NewGuid();
        }


        #region PrimaryEntityName
        public string PrimaryEntityName
        {
            get { return _entityName; }
        }
        #endregion

        #region PrimaryEntityId
        public Guid PrimaryEntityId
        {
            get { return _entityId; }
        }
        #endregion

        #region Depth
        public int Depth
        {
            get { return 1; }
        }
        #endregion

        #region Business Unit Ref
        public EntityReference BusinessUnitRef
        {
            get { return _businessUnitRef; }
        }
        #endregion

        #region User Ids
        public Guid UserId
        {
            get { return _userId; }
        }

        public Guid InitiatingUserId
        {
            get { return _initiatingUserId; }
        }
        #endregion

        #region Stages
        public bool IsPostOperation()
        {
            return IsStage(Stages.PostOperation);
        }
        public bool IsPreOperation()
        {
            return IsStage(Stages.PreOperation);
        }

        public bool IsPreValidation()
        {
            return IsStage(Stages.PreValidation);
        }

        public bool IsStage(Stages stage)
        {
            return _stage == stage;
        }
        #endregion

        #region OrganizationId
        public Guid OrganizationId
        {
            get { return _organizationId; }
        }
        #endregion

        #region OrganizationService
        public Microsoft.Xrm.Sdk.IOrganizationService OrganizationService
        {
            get { return null; }
        }
        #endregion

        #region Parameters

        public object this[InputParameters parameterName]
        {
            get
            {
                return _inputParameters[parameterName];
            }
            set
            {
                _inputParameters[parameterName] = value;
            }
        }

        public object this[OutputParameters parameterName]
        {
            get
            {
                return _outputParameters[parameterName];
            }
            set
            {
                _outputParameters[parameterName] = value;
            }
        }

        public T GetInputParameter<T>(InputParameters parameterName)
        {
            if (_inputParameters.ContainsKey(parameterName))
            {
                return (T)_inputParameters[parameterName];
            };
            throw new ArgumentException(string.Format("No Input parameter {0} has been set", parameterName.ToString()));
        }

        public void SetInputParameter<T>(InputParameters parameterName, T parameterValue)
        {
            _inputParameters[parameterName] = parameterValue;
        }

        public T GetOutputParameter<T>(OutputParameters parameterName)
        {
            if (_outputParameters.ContainsKey(parameterName))
            {
                return (T)_outputParameters[parameterName];
            };
            throw new ArgumentException(string.Format("No Output parameter {0} has been set", parameterName.ToString()));
        }

        public void SetOutputParameter<T>(OutputParameters parameterName, T parameterValue)
        {
            _outputParameters[parameterName] = parameterValue;
        }
        #endregion

        #region Images
        public IDictionary<string, Entity> PreImages
        {
            get { return _preImages; }
        }

        public IDictionary<string, Entity> PostImages
        {
            get { return _postImages; }
        }

        public bool HasPostImage(string imageName)
        {
            return _postImages.ContainsKey(imageName);
        }

        public Microsoft.Xrm.Sdk.Entity GetPostImage(string imageName)
        {
            if (HasPostImage(imageName))
            {
                return _postImages[imageName];
            }
            throw new ArgumentException(string.Format("No Post image {0} has been set", imageName));
        }

        public bool HasPreImage(string imageName)
        {
            return _preImages.ContainsKey(imageName);
        }

        public Microsoft.Xrm.Sdk.Entity GetPreImage(string imageName)
        {
            if (HasPreImage(imageName))
            {
                return _preImages[imageName];
            }
            throw new ArgumentException(string.Format("No Pre image {0} has been set", imageName));
        }
        #endregion

        #region HasSharedVariable
        public bool HasSharedVariable(string variableName)
        {
            throw new NotImplementedException();
        }

        public T GetSharedVariable<T>(string variableName)
        {
            throw new NotImplementedException();
        }

        public void SetSharedVariable<T>(string variableName, T value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IsSynchronous
        public bool IsSynchronous()
        {
            return _mode == Modes.Synchronous;
        }

        public bool IsAsynchronous()
        {
            return _mode == Modes.Asynchronous;
        }
        #endregion

        #region Messages
        public Messages MessageName
        {
            get { return _message; }
        }

        public bool IsCreate()
        {
            return IsMessage(Messages.Create);
        }

        public bool IsUpdate()
        {
            return IsMessage(Messages.Update);
        }

        public bool IsMessage(Messages message)
        {
            return _message == message;
        }
        #endregion

        public Guid CorrelationId { get; }

        #region Log
        public void Log(string message, params object[] formatArgs)
        {

        }
        public void LogFields(Entity entity, params string[] fieldNames)
        {

        }

        public void ThrowInvalidPluginException(string messageName, params object[] formatArguments)
        {

        }

        public IPluginContext ParentContext { get; }
        public bool IsMultiplePrePostOperation { get; }

        #endregion

        #region LogFields
        #endregion
    }
}
