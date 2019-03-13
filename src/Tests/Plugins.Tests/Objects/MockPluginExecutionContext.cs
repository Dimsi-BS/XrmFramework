// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Plugins.Tests.Objects
{
    [ExcludeFromCodeCoverage]
    public class MockPluginExecutionContext : Mock<IPluginExecutionContext>, IPluginExecutionContext
    {


        public MockPluginExecutionContext()
        {
            var mockOrganizationServiceFactory = new Mock<IOrganizationServiceFactory>();

            OrganizationServiceFactory = mockOrganizationServiceFactory;
        }

        public Mock<IOrganizationServiceFactory> OrganizationServiceFactory { get; }

        public int Mode
        {
            get => Object.Mode;
            set => SetupGet(s => s.Mode).Returns(value);
        }

        public int IsolationMode
        {
            get => Object.IsolationMode;
            set => SetupGet(s => s.IsolationMode).Returns(value);
        }

        public int Depth
        {
            get => Object.Depth;
            set => SetupGet(s => s.Depth).Returns(value);
        }

        public string MessageName
        {
            get => Object.MessageName;
            set => SetupGet(s => s.MessageName).Returns(value);
        }

        public string PrimaryEntityName
        {
            get => Object.PrimaryEntityName;
            set => SetupGet(s => s.PrimaryEntityName).Returns(value);
        }

        public Guid? RequestId
        {
            get => Object.RequestId;
            set => SetupGet(s => s.RequestId).Returns(value);
        }

        public string SecondaryEntityName
        {
            get => Object.SecondaryEntityName;
            set => SetupGet(s => s.SecondaryEntityName).Returns(value);
        }

        public ParameterCollection InputParameters { get; } = new ParameterCollection();

        public ParameterCollection OutputParameters { get; } = new ParameterCollection();

        public ParameterCollection SharedVariables { get; } = new ParameterCollection();

        public Guid UserId
        {
            get => Object.UserId;
            set => SetupGet(s => s.UserId).Returns(value);
        }

        public Guid InitiatingUserId
        {
            get => Object.InitiatingUserId;
            set => SetupGet(s => s.InitiatingUserId).Returns(value);
        }

        public Guid BusinessUnitId
        {
            get => Object.BusinessUnitId;
            set => SetupGet(s => s.BusinessUnitId).Returns(value);
        }

        public Guid OrganizationId
        {
            get => Object.OrganizationId;
            set => SetupGet(s => s.OrganizationId).Returns(value);
        }

        public string OrganizationName
        {
            get => Object.OrganizationName;
            set => SetupGet(s => s.OrganizationName).Returns(value);
        }

        public Guid PrimaryEntityId
        {
            get => Object.PrimaryEntityId;
            set => SetupGet(s => s.PrimaryEntityId).Returns(value);
        }

        public EntityImageCollection PreEntityImages { get; } = new EntityImageCollection();

        public EntityImageCollection PostEntityImages { get; } = new EntityImageCollection();

        public EntityReference OwningExtension
        {
            get => Object.OwningExtension;
            set => SetupGet(s => s.OwningExtension).Returns(value);
        }

        public Guid CorrelationId
        {
            get => Object.CorrelationId;
            set => SetupGet(s => s.CorrelationId).Returns(value);
        }

        public bool IsExecutingOffline
        {
            get => Object.IsExecutingOffline;
            set => SetupGet(s => s.IsExecutingOffline).Returns(value);
        }

        public bool IsOfflinePlayback
        {
            get => Object.IsOfflinePlayback;
            set => SetupGet(s => s.IsOfflinePlayback).Returns(value);
        }

        public bool IsInTransaction
        {
            get => Object.IsInTransaction;
            set => SetupGet(s => s.IsInTransaction).Returns(value);
        }

        public Guid OperationId
        {
            get => Object.OperationId;
            set => SetupGet(s => s.OperationId).Returns(value);
        }

        public DateTime OperationCreatedOn
        {
            get => Object.OperationCreatedOn;
            set => SetupGet(s => s.OperationCreatedOn).Returns(value);
        }

        public int Stage
        {
            get => Object.Stage;
            set => SetupGet(s => s.Stage).Returns(value);
        }

        public IPluginExecutionContext ParentContext
        {
            get => Object.ParentContext;
            set => SetupGet(s => s.ParentContext).Returns(value);
        }
    }
}
