// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Moq;
using NUnit.Framework;
using SdkRelationship = Microsoft.Xrm.Sdk.Relationship;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.Service
{
    [TestFixture]
    public class LoggedIServiceTests
    {
        // ─────────────────────────────────────────────────────────────────────
        //  Minimal IBindingModel implementation for generic method tests
        // ─────────────────────────────────────────────────────────────────────
        private class TestModel : IBindingModel
        {
            public Guid Id { get; set; }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Test state
        // ─────────────────────────────────────────────────────────────────────
        private Mock<IServiceContext> _contextMock;
        private Mock<IService> _innerServiceMock;
        private LoggedIService _sut;
        private int _logCallCount;

        [SetUp]
        public void SetUp()
        {
            _logCallCount = 0;
            LogServiceMethod logDelegate = (_, __, ___) => _logCallCount++;

            _contextMock = new Mock<IServiceContext>();
            _contextMock.Setup(c => c.LogServiceMethod).Returns(logDelegate);
            _contextMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.InitiatingUserId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.CorrelationId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.OrganizationName).Returns("testorg");

            _innerServiceMock = new Mock<IService>();

            _sut = new LoggedIService(_contextMock.Object, _innerServiceMock.Object);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Class structure
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void LoggedIService_InheritsFromLoggedServiceBase()
        {
            Assert.IsInstanceOf<LoggedServiceBase>(_sut);
        }

        [Test]
        public void LoggedIService_ImplementsIService()
        {
            Assert.IsInstanceOf<IService>(_sut);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Create(Entity, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Create_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(null, false, false));
        }

        [Test]
        public void Create_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");

            _sut.Create(entity, false, false);

            _innerServiceMock.Verify(s => s.Create(entity, false, false), Times.Once);
        }

        [Test]
        public void Create_ValidArgs_CallsLogTwice()
        {
            _innerServiceMock.Setup(s => s.Create(It.IsAny<Entity>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Guid.NewGuid());

            _sut.Create(new Entity("account"), false, false);

            Assert.That(_logCallCount, Is.EqualTo(2));
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Upsert(Entity, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Upsert_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert((Entity)null, false, false));
        }

        [Test]
        public void Upsert_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");
            _innerServiceMock.Setup(s => s.Upsert(entity, false, false)).Returns(new UpsertResponse());

            _sut.Upsert(entity, false, false);

            _innerServiceMock.Verify(s => s.Upsert(entity, false, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Update(Entity, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Update_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update((Entity)null, false, false));
        }

        [Test]
        public void Update_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");

            _sut.Update(entity, false, false);

            _innerServiceMock.Verify(s => s.Update(entity, false, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete(string, Guid, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Delete_NullLogicalName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((string)null, Guid.NewGuid(), false, false));
        }

        [Test]
        public void Delete_EmptyLogicalName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("  ", Guid.NewGuid(), false, false));
        }

        [Test]
        public void Delete_DefaultId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("account", Guid.Empty, false, false));
        }

        [Test]
        public void Delete_ValidArgs_DelegatesToInnerService()
        {
            var id = Guid.NewGuid();

            _sut.Delete("account", id, false, false);

            _innerServiceMock.Verify(s => s.Delete("account", id, false, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Create(Entity, Guid callerId, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Create_WithCallerId_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(null, Guid.NewGuid(), false));
        }

        [Test]
        public void Create_WithCallerId_DefaultCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(new Entity("account"), Guid.Empty, false));
        }

        [Test]
        public void Create_WithCallerId_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");
            var callerId = Guid.NewGuid();

            _sut.Create(entity, callerId, false);

            _innerServiceMock.Verify(s => s.Create(entity, callerId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Upsert(Entity, Guid callerId, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Upsert_WithCallerId_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert((Entity)null, Guid.NewGuid(), false));
        }

        [Test]
        public void Upsert_WithCallerId_DefaultCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert(new Entity("account"), Guid.Empty, false));
        }

        [Test]
        public void Upsert_WithCallerId_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");
            var callerId = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.Upsert(entity, callerId, false)).Returns(new UpsertResponse());

            _sut.Upsert(entity, callerId, false);

            _innerServiceMock.Verify(s => s.Upsert(entity, callerId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Update(Entity, Guid callerId, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Update_WithCallerId_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update((Entity)null, Guid.NewGuid(), false));
        }

        [Test]
        public void Update_WithCallerId_DefaultCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update(new Entity("account"), Guid.Empty, false));
        }

        [Test]
        public void Update_WithCallerId_ValidArgs_DelegatesToInnerService()
        {
            var entity = new Entity("account");
            var callerId = Guid.NewGuid();

            _sut.Update(entity, callerId, false);

            _innerServiceMock.Verify(s => s.Update(entity, callerId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete(string, Guid, Guid callerId, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Delete_WithCallerId_NullLogicalName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((string)null, Guid.NewGuid(), Guid.NewGuid(), false));
        }

        [Test]
        public void Delete_WithCallerId_DefaultId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("account", Guid.Empty, Guid.NewGuid(), false));
        }

        [Test]
        public void Delete_WithCallerId_DefaultCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("account", Guid.NewGuid(), Guid.Empty, false));
        }

        [Test]
        public void Delete_WithCallerId_ValidArgs_DelegatesToInnerService()
        {
            var id = Guid.NewGuid();
            var callerId = Guid.NewGuid();

            _sut.Delete("account", id, callerId, false);

            _innerServiceMock.Verify(s => s.Delete("account", id, callerId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete(EntityReference, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Delete_NullObjectReference_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((EntityReference)null, false, false));
        }

        [Test]
        public void Delete_ObjectReference_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());

            _sut.Delete(objRef, false, false);

            _innerServiceMock.Verify(s => s.Delete(objRef, false, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Delete(EntityReference, Guid callerId, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Delete_WithCallerId_NullObjectReference_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((EntityReference)null, Guid.NewGuid(), false));
        }

        [Test]
        public void Delete_WithCallerId_DefaultCallerIdObjectReference_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Delete(new EntityReference("account", Guid.NewGuid()), Guid.Empty, false));
        }

        [Test]
        public void Delete_WithCallerId_ObjectReference_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            var callerId = Guid.NewGuid();

            _sut.Delete(objRef, callerId, false);

            _innerServiceMock.Verify(s => s.Delete(objRef, callerId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AssignEntity
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AssignEntity_NullObjectReference_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AssignEntity(null, new EntityReference("systemuser", Guid.NewGuid()), false));
        }

        [Test]
        public void AssignEntity_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            var ownerRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.AssignEntity(objRef, ownerRef, false);

            _innerServiceMock.Verify(s => s.AssignEntity(objRef, ownerRef, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AddUsersToTeam(EntityReference, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AddUsersToTeam_NullTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddUsersToTeam(null, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void AddUsersToTeam_ValidArgs_DelegatesToInnerService()
        {
            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.AddUsersToTeam(teamRef, userRef);

            _innerServiceMock.Verify(s => s.AddUsersToTeam(teamRef, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AddUsersToTeam(EntityReference, bool, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AddUsersToTeam_WithBypass_NullTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddUsersToTeam(null, false, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void AddUsersToTeam_WithBypass_ValidArgs_DelegatesToInnerService()
        {
            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.AddUsersToTeam(teamRef, false, userRef);

            _innerServiceMock.Verify(s => s.AddUsersToTeam(teamRef, false, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  RemoveUsersFromTeam(EntityReference, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void RemoveUsersFromTeam_NullTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.RemoveUsersFromTeam(null, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void RemoveUsersFromTeam_ValidArgs_DelegatesToInnerService()
        {
            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.RemoveUsersFromTeam(teamRef, userRef);

            _innerServiceMock.Verify(s => s.RemoveUsersFromTeam(teamRef, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  RemoveUsersFromTeam(EntityReference, bool, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void RemoveUsersFromTeam_WithBypass_NullTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.RemoveUsersFromTeam(null, false, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void RemoveUsersFromTeam_WithBypass_ValidArgs_DelegatesToInnerService()
        {
            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.RemoveUsersFromTeam(teamRef, false, userRef);

            _innerServiceMock.Verify(
                s => s.RemoveUsersFromTeam(teamRef, false, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AddToQueue
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AddToQueue_DefaultQueueId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddToQueue(Guid.Empty, new EntityReference("task", Guid.NewGuid()), false));
        }

        [Test]
        public void AddToQueue_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddToQueue(Guid.NewGuid(), null, false));
        }

        [Test]
        public void AddToQueue_ValidArgs_DelegatesToInnerService()
        {
            var queueId = Guid.NewGuid();
            var target = new EntityReference("task", Guid.NewGuid());

            _sut.AddToQueue(queueId, target, false);

            _innerServiceMock.Verify(s => s.AddToQueue(queueId, target, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Merge
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Merge_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Merge(null, Guid.NewGuid(), new Entity("account"), false));
        }

        [Test]
        public void Merge_DefaultSubordonate_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Merge(new EntityReference("account", Guid.NewGuid()), Guid.Empty, new Entity("account"), false));
        }

        [Test]
        public void Merge_NullContent_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Merge(new EntityReference("account", Guid.NewGuid()), Guid.NewGuid(), null, false));
        }

        [Test]
        public void Merge_ValidArgs_DelegatesToInnerService()
        {
            var target = new EntityReference("account", Guid.NewGuid());
            var subordonate = Guid.NewGuid();
            var content = new Entity("account");

            _sut.Merge(target, subordonate, content, false);

            _innerServiceMock.Verify(s => s.Merge(target, subordonate, content, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  SetState
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void SetState_NullObjectRef_ThrowsArgumentNullException()
        {
#pragma warning disable CS0618
            Assert.Throws<ArgumentNullException>(() => _sut.SetState(null, 0, 1, false, false));
#pragma warning restore CS0618
        }

        [Test]
        public void SetState_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());

#pragma warning disable CS0618
            _sut.SetState(objRef, 0, 1, false, false);
#pragma warning restore CS0618

#pragma warning disable CS0618
            _innerServiceMock.Verify(s => s.SetState(objRef, 0, 1, false, false), Times.Once);
#pragma warning restore CS0618
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Share
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Share_NullObjectRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Share(null, new EntityReference("systemuser", Guid.NewGuid()), AccessRights.ReadAccess, false));
        }

        [Test]
        public void Share_NullAssignee_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.Share(new EntityReference("account", Guid.NewGuid()), null, AccessRights.ReadAccess, false));
        }

        [Test]
        public void Share_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            var assignee = new EntityReference("systemuser", Guid.NewGuid());

            _sut.Share(objRef, assignee, AccessRights.ReadAccess, false);

            _innerServiceMock.Verify(s => s.Share(objRef, assignee, AccessRights.ReadAccess, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  UnShare
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void UnShare_NullObjectRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.UnShare(null, new EntityReference("systemuser", Guid.NewGuid()), null, false));
        }

        [Test]
        public void UnShare_NullRevokee_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.UnShare(new EntityReference("account", Guid.NewGuid()), null, null, false));
        }

        [Test]
        public void UnShare_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            var revokee = new EntityReference("systemuser", Guid.NewGuid());

            _sut.UnShare(objRef, revokee, null, false);

            _innerServiceMock.Verify(s => s.UnShare(objRef, revokee, null, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Retrieve(string, Guid, params string[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_NullEntityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((string)null, Guid.NewGuid(), "name"));
        }

        [Test]
        public void Retrieve_EmptyEntityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("  ", Guid.NewGuid(), "name"));
        }

        [Test]
        public void Retrieve_DefaultId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("account", Guid.Empty, "name"));
        }

        [Test]
        public void Retrieve_ValidArgs_DelegatesToInnerService()
        {
            var id = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.Retrieve("account", id, It.IsAny<string[]>())).Returns(new Entity("account"));

            _sut.Retrieve("account", id, "name");

            _innerServiceMock.Verify(s => s.Retrieve("account", id, It.IsAny<string[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Retrieve(string, Guid, bool allColumns)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_WithAllColumns_NullEntityName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((string)null, Guid.NewGuid(), true));
        }

        [Test]
        public void Retrieve_WithAllColumns_DefaultId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("account", Guid.Empty, true));
        }

        [Test]
        public void Retrieve_WithAllColumns_ValidArgs_DelegatesToInnerService()
        {
            var id = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.Retrieve("account", id, true)).Returns(new Entity("account"));

            _sut.Retrieve("account", id, true);

            _innerServiceMock.Verify(s => s.Retrieve("account", id, true), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Retrieve(EntityReference, params string[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_NullObjectRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((EntityReference)null, "name"));
        }

        [Test]
        public void Retrieve_ObjectRef_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            _innerServiceMock.Setup(s => s.Retrieve(objRef, It.IsAny<string[]>())).Returns(new Entity("account"));

            _sut.Retrieve(objRef, "name");

            _innerServiceMock.Verify(s => s.Retrieve(objRef, It.IsAny<string[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Retrieve(EntityReference, bool allColumns)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_ObjectRef_WithAllColumns_NullObjectRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((EntityReference)null, true));
        }

        [Test]
        public void Retrieve_ObjectRef_WithAllColumns_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            _innerServiceMock.Setup(s => s.Retrieve(objRef, true)).Returns(new Entity("account"));

            _sut.Retrieve(objRef, true);

            _innerServiceMock.Verify(s => s.Retrieve(objRef, true), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue(string, int)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetNameFromValue_NullName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue(null, 1));
        }

        [Test]
        public void GetOptionSetNameFromValue_EmptyName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue("  ", 1));
        }

        [Test]
        public void GetOptionSetNameFromValue_ValidArgs_DelegatesToInnerService()
        {
            _innerServiceMock.Setup(s => s.GetOptionSetNameFromValue("statuscode", 1)).Returns("Active");

            _sut.GetOptionSetNameFromValue("statuscode", 1);

            _innerServiceMock.Verify(s => s.GetOptionSetNameFromValue("statuscode", 1), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue<T>(int) — no guard clause
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetNameFromValue_Generic_DelegatesToInnerService()
        {
            _innerServiceMock.Setup(s => s.GetOptionSetNameFromValue<int>(1)).Returns("Active");

            _sut.GetOptionSetNameFromValue<int>(1);

            _innerServiceMock.Verify(s => s.GetOptionSetNameFromValue<int>(1), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetById<T>(Guid)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetById_DefaultId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetById<TestModel>(Guid.Empty));
        }

        [Test]
        public void GetById_ValidId_DelegatesToInnerService()
        {
            var id = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.GetById<TestModel>(id)).Returns(new TestModel());

            _sut.GetById<TestModel>(id);

            _innerServiceMock.Verify(s => s.GetById<TestModel>(id), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetById<T>(EntityReference)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetById_NullEntityReference_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetById<TestModel>((EntityReference)null));
        }

        [Test]
        public void GetById_EntityReference_DelegatesToInnerService()
        {
            var entityRef = new EntityReference("account", Guid.NewGuid());
            _innerServiceMock.Setup(s => s.GetById<TestModel>(entityRef)).Returns(new TestModel());

            _sut.GetById<TestModel>(entityRef);

            _innerServiceMock.Verify(s => s.GetById<TestModel>(entityRef), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Upsert<T>(T model, bool, bool)
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void UpsertGeneric_NullModel_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert<TestModel>(null, false, false));
        }

        [Test]
        public void UpsertGeneric_ValidArgs_DelegatesToInnerService()
        {
            var model = new TestModel { Id = Guid.NewGuid() };
            _innerServiceMock.Setup(s => s.Upsert<TestModel>(model, false, false)).Returns(model);

            _sut.Upsert<TestModel>(model, false, false);

            _innerServiceMock.Verify(s => s.Upsert<TestModel>(model, false, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  UserHasRole
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void UserHasRole_DefaultUserId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.UserHasRole(Guid.Empty, Guid.NewGuid()));
        }

        [Test]
        public void UserHasRole_DefaultParentRoleId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.UserHasRole(Guid.NewGuid(), Guid.Empty));
        }

        [Test]
        public void UserHasRole_ValidArgs_DelegatesToInnerService()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.UserHasRole(userId, roleId)).Returns(true);

            _sut.UserHasRole(userId, roleId);

            _innerServiceMock.Verify(s => s.UserHasRole(userId, roleId), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  UserHasOneRoleOf(Guid, params Guid[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void UserHasOneRoleOf_DefaultUserId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.UserHasOneRoleOf(Guid.Empty, Guid.NewGuid()));
        }

        [Test]
        public void UserHasOneRoleOf_ValidArgs_DelegatesToInnerService()
        {
            var userId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.UserHasOneRoleOf(userId, It.IsAny<Guid[]>())).Returns(true);

            _sut.UserHasOneRoleOf(userId, roleId);

            _innerServiceMock.Verify(s => s.UserHasOneRoleOf(userId, It.IsAny<Guid[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  UserHasOneRoleOf(Guid, params string[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void UserHasOneRoleOf_String_DefaultUserId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.UserHasOneRoleOf(Guid.Empty, "roleId"));
        }

        [Test]
        public void UserHasOneRoleOf_String_ValidArgs_DelegatesToInnerService()
        {
            var userId = Guid.NewGuid();
            _innerServiceMock.Setup(s => s.UserHasOneRoleOf(userId, It.IsAny<string[]>())).Returns(true);

            _sut.UserHasOneRoleOf(userId, "roleId");

            _innerServiceMock.Verify(s => s.UserHasOneRoleOf(userId, It.IsAny<string[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetUserRoleIds
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetUserRoleIds_NullUserRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetUserRoleIds(null));
        }

        [Test]
        public void GetUserRoleIds_ValidArgs_DelegatesToInnerService()
        {
            var userRef = new EntityReference("systemuser", Guid.NewGuid());
            _innerServiceMock.Setup(s => s.GetUserRoleIds(userRef)).Returns(new List<Guid>());

            _sut.GetUserRoleIds(userRef);

            _innerServiceMock.Verify(s => s.GetUserRoleIds(userRef), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  ToEntity<T>
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void ToEntity_NullModel_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.ToEntity<TestModel>(null));
        }

        [Test]
        public void ToEntity_ValidArgs_DelegatesToInnerService()
        {
            var model = new TestModel { Id = Guid.NewGuid() };
            _innerServiceMock.Setup(s => s.ToEntity<TestModel>(model)).Returns(new Entity("account"));

            _sut.ToEntity(model);

            _innerServiceMock.Verify(s => s.ToEntity<TestModel>(model), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GetTeamMemberRefs
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void GetTeamMemberRefs_NullTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetTeamMemberRefs(null));
        }

        [Test]
        public void GetTeamMemberRefs_ValidArgs_DelegatesToInnerService()
        {
            var teamRef = new EntityReference("team", Guid.NewGuid());
            _innerServiceMock.Setup(s => s.GetTeamMemberRefs(teamRef)).Returns(new List<EntityReference>());

            _sut.GetTeamMemberRefs(teamRef);

            _innerServiceMock.Verify(s => s.GetTeamMemberRefs(teamRef), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AssociateRecords(EntityReference, Relationship, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AssociateRecords_NullObjectRef_ThrowsArgumentNullException()
        {
            SdkRelationship relation = new SdkRelationship("account_contact");
            Assert.Throws<ArgumentNullException>(
                () => _sut.AssociateRecords(null, relation, new EntityReference("contact", Guid.NewGuid())));
        }

        [Test]
        public void AssociateRecords_NullRelationName_ThrowsArgumentNullException()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(
                () => _sut.AssociateRecords(objRef, null, new EntityReference("contact", Guid.NewGuid())));
        }

        [Test]
        public void AssociateRecords_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            SdkRelationship relation = new SdkRelationship("account_contact");
            var related = new EntityReference("contact", Guid.NewGuid());

            _sut.AssociateRecords(objRef, relation, related);

            _innerServiceMock.Verify(
                s => s.AssociateRecords(objRef, relation, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AssociateRecords(EntityReference, Relationship, bool, params EntityReference[])
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AssociateRecords_WithBypass_NullObjectRef_ThrowsArgumentNullException()
        {
            SdkRelationship relation = new SdkRelationship("account_contact");
            Assert.Throws<ArgumentNullException>(
                () => _sut.AssociateRecords(null, relation, false, new EntityReference("contact", Guid.NewGuid())));
        }

        [Test]
        public void AssociateRecords_WithBypass_NullRelationName_ThrowsArgumentNullException()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            Assert.Throws<ArgumentNullException>(
                () => _sut.AssociateRecords(objRef, null, false, new EntityReference("contact", Guid.NewGuid())));
        }

        [Test]
        public void AssociateRecords_WithBypass_ValidArgs_DelegatesToInnerService()
        {
            var objRef = new EntityReference("account", Guid.NewGuid());
            SdkRelationship relation = new SdkRelationship("account_contact");
            var related = new EntityReference("contact", Guid.NewGuid());

            _sut.AssociateRecords(objRef, relation, false, related);

            _innerServiceMock.Verify(
                s => s.AssociateRecords(objRef, relation, false, It.IsAny<EntityReference[]>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  AddRoleToUserOrTeam
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AddRoleToUserOrTeam_NullUserOrTeamRef_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddRoleToUserOrTeam(null, "role-guid", false));
        }

        [Test]
        public void AddRoleToUserOrTeam_EmptyRoleId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => _sut.AddRoleToUserOrTeam(new EntityReference("systemuser", Guid.NewGuid()), "  ", false));
        }

        [Test]
        public void AddRoleToUserOrTeam_ValidArgs_DelegatesToInnerService()
        {
            var userRef = new EntityReference("systemuser", Guid.NewGuid());
            const string roleId = "00000000-0000-0000-0000-000000000001";

            _sut.AddRoleToUserOrTeam(userRef, roleId, false);

            _innerServiceMock.Verify(s => s.AddRoleToUserOrTeam(userRef, roleId, false), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Log delegation — cross-cutting concern
        // ─────────────────────────────────────────────────────────────────────

        [Test]
        public void AnyMethod_LogIsCalledBeforeAndAfterDelegation()
        {
            // Arrange — track call order: log vs. inner service
            var callOrder = new List<string>();

            LogServiceMethod logDelegate = (_, __, ___) => callOrder.Add("log");
            _contextMock.Setup(c => c.LogServiceMethod).Returns(logDelegate);

            var innerMock = new Mock<IService>();
            innerMock.Setup(s => s.Update(It.IsAny<Entity>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Callback(() => callOrder.Add("inner"));

            var sut = new LoggedIService(_contextMock.Object, innerMock.Object);

            // Act
            sut.Update(new Entity("account"), false, false);

            // Assert — "log", "inner", "log"
            Assert.That(callOrder.Count, Is.EqualTo(3));
            Assert.That(callOrder[0], Is.EqualTo("log"));
            Assert.That(callOrder[1], Is.EqualTo("inner"));
            Assert.That(callOrder[2], Is.EqualTo("log"));
        }
    }
}
