extern alias remote;

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using remote::XrmFramework.RemoteDebugger;

namespace XrmFramework.Tests;

[TestClass]
public class RemoteDebugContextTests
{
    // Test serialization of RemoteDebugExecutionContext
    [TestMethod]
    public void TestRemoteDebugExecutionContextSerialization()
    {
        var context = new RemoteDebugExecutionContext
        {
            Mode = 1,
            IsolationMode = 2,
            Depth = 3,
            MessageName = "MessageName",
            PrimaryEntityName = "PrimaryEntityName",
            SecondaryEntityName = "SecondaryEntityName",
            UserId = Guid.NewGuid(),
            InitiatingUserId = Guid.NewGuid(),
            BusinessUnitId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            OrganizationName = "OrganizationName",
            PrimaryEntityId = Guid.NewGuid(),
            OwningExtension = Guid.NewGuid().ToEntityReference("ojdoiazj"),
            CorrelationId = Guid.NewGuid(),
            IsExecutingOffline = true,
            IsOfflinePlayback = true,
            IsInTransaction = true,
            OperationId = Guid.NewGuid(),
            OperationCreatedOn = new(2021, 1, 1),
            IsWorkflowContext = true
        };

        context.InputParameters = new();
        context.InputParameters.Add("Target", new PicklistAttributeMetadata());
        
        var json = remote::Newtonsoft.Json.JsonConvert.SerializeObject(context, RemoteDebuggerSettings.JsonSerializerSettings);

        var deserializedContext = remote::Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteDebugExecutionContext>(json, RemoteDebuggerSettings.JsonSerializerSettings);

        Assert.AreEqual(1, deserializedContext.Mode);
        Assert.AreEqual(2, deserializedContext.IsolationMode);
        Assert.AreEqual(3, deserializedContext.Depth);
        Assert.AreEqual("MessageName", deserializedContext.MessageName);
        Assert.AreEqual("PrimaryEntityName", deserializedContext.PrimaryEntityName);
        Assert.AreEqual("SecondaryEntityName", deserializedContext.SecondaryEntityName);
        Assert.AreEqual(context.UserId, deserializedContext.UserId);
        Assert.AreEqual(context.InitiatingUserId, deserializedContext.InitiatingUserId);
        Assert.AreEqual(context.BusinessUnitId, deserializedContext.BusinessUnitId);
        Assert.AreEqual(context.OrganizationId, deserializedContext.OrganizationId);
        Assert.AreEqual("OrganizationName", deserializedContext.OrganizationName);
        Assert.AreEqual(context.PrimaryEntityId, deserializedContext.PrimaryEntityId);
        Assert.AreEqual(context.OwningExtension, deserializedContext.OwningExtension);
        Assert.AreEqual(context.CorrelationId, deserializedContext.CorrelationId);
        Assert.IsTrue(deserializedContext.IsExecutingOffline);
        Assert.IsTrue(deserializedContext.IsOfflinePlayback);
        Assert.IsTrue(deserializedContext.IsInTransaction);
        Assert.AreEqual(context.OperationId, deserializedContext.OperationId);
        Assert.AreEqual(new(2021, 1, 1), deserializedContext.OperationCreatedOn);
        Assert.IsTrue(deserializedContext.IsWorkflowContext);
    }
}
