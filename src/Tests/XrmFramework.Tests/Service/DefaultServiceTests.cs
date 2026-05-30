// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace XrmFramework.Tests.Service
{
    [TestFixture]
    public class DefaultServiceTests
    {
        // ─────────────────────────────────────────────────────────────
        //  Test helpers — typed enums for GetOptionSetNameFromValue<T>
        // ─────────────────────────────────────────────────────────────

        private enum EnumWithoutDefinitionAttribute { A = 1, B = 2 }

        [OptionSetDefinition("contact", "statuscode")]
        private enum StatusCodeEnum { Active = 1, Inactive = 2 }

        // ─────────────────────────────────────────────────────────────
        //  Fixture state
        // ─────────────────────────────────────────────────────────────

        private Mock<IServiceContext> _contextMock;
        private Mock<IOrganizationService> _orgServiceMock;
        private Mock<IOrganizationService> _adminOrgServiceMock;
        private Guid _initiatingUserId;
        private DefaultService _sut;

        [SetUp]
        public void SetUp()
        {
            _initiatingUserId = Guid.NewGuid();

            _contextMock = new Mock<IServiceContext>();
            _orgServiceMock = new Mock<IOrganizationService>();
            _adminOrgServiceMock = new Mock<IOrganizationService>();

            _contextMock.Setup(c => c.OrganizationService).Returns(_orgServiceMock.Object);
            _contextMock.Setup(c => c.AdminOrganizationService).Returns(_adminOrgServiceMock.Object);
            _contextMock.Setup(c => c.LogServiceMethod).Returns((string _, string __, object[] ___) => { });
            _contextMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.InitiatingUserId).Returns(_initiatingUserId);
            _contextMock.Setup(c => c.CorrelationId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.OrganizationName).Returns("testorg");

            _sut = new DefaultService(_contextMock.Object);
        }

        // ─────────────────────────────────────────────────────────────
        //  Create — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Create_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(null));
        }

        [Test]
        public void Create_NullEntityWithCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Create(null, Guid.NewGuid()));
        }

        // ─────────────────────────────────────────────────────────────
        //  Create — service routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Create_UseAdminFalse_ExecutesOnOrganizationService()
        {
            var expectedId = Guid.NewGuid();
            SetupExecuteReturnsId(_orgServiceMock, expectedId);

            var result = _sut.Create(new Entity("contact"), useAdmin: false);

            Assert.AreEqual(expectedId, result);
            _orgServiceMock.Verify(s => s.Execute(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Test]
        public void Create_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            var expectedId = Guid.NewGuid();
            SetupExecuteReturnsId(_adminOrgServiceMock, expectedId);

            var result = _sut.Create(new Entity("contact"), useAdmin: true);

            Assert.AreEqual(expectedId, result);
            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Test]
        public void Create_WithCallerIdGuidEmpty_ExecutesOnAdminOrganizationService()
        {
            SetupExecuteReturnsId(_adminOrgServiceMock, Guid.NewGuid());

            _sut.Create(new Entity("contact"), Guid.Empty);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Test]
        public void Create_WithCallerId_CallsGetOrganizationServiceWithCallerId()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            SetupExecuteReturnsId(callerService, Guid.NewGuid());
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);

            _sut.Create(new Entity("contact"), callerId);

            _contextMock.Verify(c => c.GetOrganizationService(callerId), Times.Once);
        }

        [Test]
        public void Create_BypassCustomPluginExecution_SetsRequestFlag()
        {
            CreateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<CreateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (CreateRequest)r)
                .Returns(BuildCreateResponse(Guid.NewGuid()));

            _sut.Create(new Entity("contact"), useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void Create_BypassFalse_DoesNotSetRequestFlag()
        {
            CreateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<CreateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (CreateRequest)r)
                .Returns(BuildCreateResponse(Guid.NewGuid()));

            _sut.Create(new Entity("contact"), useAdmin: false, bypassCustomPluginExecution: false);

            Assert.IsFalse(capturedRequest.Parameters.Contains("BypassCustomPluginExecution"));
        }

        // ─────────────────────────────────────────────────────────────
        //  Upsert(Entity) — guard clauses + routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Upsert_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert((Entity)null));
        }

        [Test]
        public void Upsert_NullEntityWithCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Upsert((Entity)null, Guid.NewGuid()));
        }

        [Test]
        public void Upsert_UseAdminFalse_ExecutesOnOrganizationService()
        {
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<UpsertRequest>())).Returns(new UpsertResponse());

            _sut.Upsert(new Entity("contact"), useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<UpsertRequest>()), Times.Once);
        }

        [Test]
        public void Upsert_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<UpsertRequest>())).Returns(new UpsertResponse());

            _sut.Upsert(new Entity("contact"), useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<UpsertRequest>()), Times.Once);
        }

        [Test]
        public void Upsert_WithCallerId_CallsGetOrganizationServiceWithCallerId()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            callerService.Setup(s => s.Execute(It.IsAny<UpsertRequest>())).Returns(new UpsertResponse());
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);

            _sut.Upsert(new Entity("contact"), callerId);

            _contextMock.Verify(c => c.GetOrganizationService(callerId), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────
        //  Update — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Update_NullEntity_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update(null));
        }

        [Test]
        public void Update_EntityWithEmptyId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update(new Entity("contact") { Id = Guid.Empty }));
        }

        [Test]
        public void Update_NullEntityWithCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update(null, Guid.NewGuid()));
        }

        [Test]
        public void Update_EntityWithEmptyIdAndCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Update(new Entity("contact"), Guid.NewGuid()));
        }

        // ─────────────────────────────────────────────────────────────
        //  Update — service routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Update_UseAdminFalse_ExecutesOnOrganizationService()
        {
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<UpdateRequest>()), Times.Once);
        }

        [Test]
        public void Update_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<UpdateRequest>()), Times.Once);
        }

        [Test]
        public void Update_WithCallerId_CallsGetOrganizationServiceWithCallerId()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            callerService.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, callerId);

            _contextMock.Verify(c => c.GetOrganizationService(callerId), Times.Once);
        }

        [Test]
        public void Update_WithCallerIdGuidEmpty_ExecutesOnAdminOrganizationService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, Guid.Empty);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<UpdateRequest>()), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────
        //  Delete(string, Guid, ...) — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Delete_StringGuid_NullLogicalName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((string)null, Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuid_EmptyLogicalName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete(string.Empty, Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuid_EmptyId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("contact", Guid.Empty));
        }

        // ─────────────────────────────────────────────────────────────
        //  Delete(string, Guid, Guid) — guard clauses + routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Delete_StringGuidCallerId_NullLogicalName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((string)null, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuidCallerId_EmptyLogicalName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete(string.Empty, Guid.NewGuid(), Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuidCallerId_EmptyId_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete("contact", Guid.Empty, Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuidCallerId_WithCallerId_CallsGetOrganizationService()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            callerService.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);

            _sut.Delete("contact", Guid.NewGuid(), callerId);

            _contextMock.Verify(c => c.GetOrganizationService(callerId), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────
        //  Delete(EntityReference, ...) — guard clauses + routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Delete_EntityRef_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((EntityReference)null));
        }

        [Test]
        public void Delete_EntityRef_NullWithCallerId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Delete((EntityReference)null, Guid.NewGuid()));
        }

        [Test]
        public void Delete_StringGuid_UseAdminFalse_ExecutesDeleteOnOrgService()
        {
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete("contact", Guid.NewGuid(), useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void Delete_EntityRef_UseAdminTrue_ExecutesDeleteOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete(new EntityReference("contact", Guid.NewGuid()), useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void Delete_EntityRefCallerId_WithCallerId_CallsGetOrganizationService()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            callerService.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);

            _sut.Delete(new EntityReference("contact", Guid.NewGuid()), callerId);

            _contextMock.Verify(c => c.GetOrganizationService(callerId), Times.Once);
        }

        // ─────────────────────────────────────────────────────────────
        //  Retrieve(string, Guid, ...) — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_StringGuidCols_NullEntityName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(null, Guid.NewGuid(), "name"));
        }

        [Test]
        public void Retrieve_StringGuidCols_EmptyEntityName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(string.Empty, Guid.NewGuid(), "name"));
        }

        [Test]
        public void Retrieve_StringGuidCols_EmptyId_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("contact", Guid.Empty, "name"));
        }

        [Test]
        public void Retrieve_StringGuidCols_NullColumns_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("contact", Guid.NewGuid(), (string[])null));
        }

        [Test]
        public void Retrieve_StringGuidCols_EmptyColumns_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("contact", Guid.NewGuid()));
        }

        [Test]
        public void Retrieve_StringGuidAllColumns_NullEntityName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(null, Guid.NewGuid(), true));
        }

        [Test]
        public void Retrieve_StringGuidAllColumns_EmptyId_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("contact", Guid.Empty, true));
        }

        [Test]
        public void Retrieve_StringGuidAllColumns_AllColumnsFalse_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve("contact", Guid.NewGuid(), false));
        }

        // ─────────────────────────────────────────────────────────────
        //  Retrieve(EntityReference, ...) — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_RefCols_NullRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((EntityReference)null, "name"));
        }

        [Test]
        public void Retrieve_RefCols_NullColumns_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(new EntityReference("contact", Guid.NewGuid()), (string[])null));
        }

        [Test]
        public void Retrieve_RefCols_EmptyColumns_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(new EntityReference("contact", Guid.NewGuid())));
        }

        [Test]
        public void Retrieve_RefAllColumns_NullRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve((EntityReference)null, true));
        }

        [Test]
        public void Retrieve_RefAllColumns_AllColumnsFalse_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Retrieve(new EntityReference("contact", Guid.NewGuid()), false));
        }

        // ─────────────────────────────────────────────────────────────
        //  Retrieve — happy paths (always uses AdminOrganizationService)
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_StringGuidCols_ExecutesRetrieveOnAdminService()
        {
            var expected = new Entity("contact");
            SetupRetrieve(_adminOrgServiceMock, expected);

            var result = _sut.Retrieve("contact", Guid.NewGuid(), "firstname");

            Assert.AreSame(expected, result);
            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<RetrieveRequest>()), Times.Once);
        }

        [Test]
        public void Retrieve_StringGuidAllColumns_ExecutesRetrieveOnAdminService()
        {
            var expected = new Entity("contact");
            SetupRetrieve(_adminOrgServiceMock, expected);

            var result = _sut.Retrieve("contact", Guid.NewGuid(), true);

            Assert.AreSame(expected, result);
        }

        [Test]
        public void Retrieve_RefCols_ExecutesRetrieveOnAdminService()
        {
            var expected = new Entity("contact");
            SetupRetrieve(_adminOrgServiceMock, expected);

            var result = _sut.Retrieve(new EntityReference("contact", Guid.NewGuid()), "firstname");

            Assert.AreSame(expected, result);
        }

        [Test]
        public void Retrieve_RefAllColumns_ExecutesRetrieveOnAdminService()
        {
            var expected = new Entity("contact");
            SetupRetrieve(_adminOrgServiceMock, expected);

            var result = _sut.Retrieve(new EntityReference("contact", Guid.NewGuid()), true);

            Assert.AreSame(expected, result);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue(string, int) — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetNameFromValue_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue(null, 1));
        }

        [Test]
        public void GetOptionSetNameFromValue_EmptyName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue(string.Empty, 1));
        }

        [Test]
        public void GetOptionSetNameFromValue_NegativeValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue("statuscode", -1));
        }

        // ─────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue<T> — guard clauses + invalid type
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetNameFromValue_Generic_NegativeValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.GetOptionSetNameFromValue<EnumWithoutDefinitionAttribute>(-1));
        }

        [Test]
        public void GetOptionSetNameFromValue_Generic_NonEnumType_ThrowsInvalidPluginExecutionException()
        {
            Assert.Throws<InvalidPluginExecutionException>(() => _sut.GetOptionSetNameFromValue<string>(1));
        }

        [Test]
        public void GetOptionSetNameFromValue_Generic_EnumWithoutOptionSetDefinition_ThrowsInvalidPluginExecutionException()
        {
            Assert.Throws<InvalidPluginExecutionException>(() => _sut.GetOptionSetNameFromValue<EnumWithoutDefinitionAttribute>(1));
        }

        // ─────────────────────────────────────────────────────────────
        //  AssignEntity
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AssignEntity_NullObjectReference_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.AssignEntity(null, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void AssignEntity_WithOwnerRef_ExecutesOnAdminServiceWithOwnerRef()
        {
            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var ownerRef = new EntityReference("systemuser", Guid.NewGuid());
            AssignRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AssignRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AssignRequest)r)
                .Returns(new AssignResponse());

            _sut.AssignEntity(objectRef, ownerRef);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(ownerRef.Id, capturedRequest.Assignee.Id);
            Assert.AreEqual(objectRef, capturedRequest.Target);
        }

        [Test]
        public void AssignEntity_NullOwnerRef_UsesInitiatingUserIdAsAssignee()
        {
            var objectRef = new EntityReference("contact", Guid.NewGuid());
            AssignRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AssignRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AssignRequest)r)
                .Returns(new AssignResponse());

            _sut.AssignEntity(objectRef, null);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(_initiatingUserId, capturedRequest.Assignee.Id);
        }

        // ─────────────────────────────────────────────────────────────
        //  SetState
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SetState_NullObjectRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetState(null, 1, 1));
        }

        [Test]
        public void SetState_UseAdminFalse_ExecutesOnOrganizationService()
        {
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<SetStateRequest>())).Returns(new SetStateResponse());

            _sut.SetState(new EntityReference("contact", Guid.NewGuid()), 0, 1, useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<SetStateRequest>()), Times.Once);
        }

        [Test]
        public void SetState_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<SetStateRequest>())).Returns(new SetStateResponse());

            _sut.SetState(new EntityReference("contact", Guid.NewGuid()), 0, 1, useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<SetStateRequest>()), Times.Once);
        }

        [Test]
        public void SetState_SetsStatecodeAndStatuscode()
        {
            SetStateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<SetStateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (SetStateRequest)r)
                .Returns(new SetStateResponse());

            _sut.SetState(new EntityReference("contact", Guid.NewGuid()), stateCode: 1, statusCode: 2);

            Assert.AreEqual(1, capturedRequest.State.Value);
            Assert.AreEqual(2, capturedRequest.Status.Value);
        }

        // ─────────────────────────────────────────────────────────────
        //  Share
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Share_NullObjectRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.Share(null, new EntityReference("systemuser", Guid.NewGuid()), AccessRights.ReadAccess));
        }

        [Test]
        public void Share_NullAssigneeRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.Share(new EntityReference("contact", Guid.NewGuid()), null, AccessRights.ReadAccess));
        }

        [Test]
        public void Share_ExecutesGrantAccessOnAdminService()
        {
            GrantAccessRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<GrantAccessRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (GrantAccessRequest)r)
                .Returns(new GrantAccessResponse());

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var assigneeRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.Share(objectRef, assigneeRef, AccessRights.ReadAccess);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(objectRef, capturedRequest.Target);
            Assert.AreEqual(assigneeRef, capturedRequest.PrincipalAccess.Principal);
            Assert.AreEqual(AccessRights.ReadAccess, capturedRequest.PrincipalAccess.AccessMask);
        }

        // ─────────────────────────────────────────────────────────────
        //  UnShare
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UnShare_NullObjectRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.UnShare(null, new EntityReference("systemuser", Guid.NewGuid())));
        }

        [Test]
        public void UnShare_NullRevokeeRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.UnShare(new EntityReference("contact", Guid.NewGuid()), null));
        }

        [Test]
        public void UnShare_ExecutesRevokeAccessOnAdminService()
        {
            RevokeAccessRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RevokeAccessRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RevokeAccessRequest)r)
                .Returns(new RevokeAccessResponse());

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var revokeeRef = new EntityReference("systemuser", Guid.NewGuid());

            _sut.UnShare(objectRef, revokeeRef);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(objectRef, capturedRequest.Target);
            Assert.AreEqual(revokeeRef, capturedRequest.Revokee);
        }

        // ─────────────────────────────────────────────────────────────
        //  AddUsersToTeam
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddUsersToTeam_EmptyArray_DoesNotCallExecute()
        {
            _sut.AddUsersToTeam(new EntityReference("team", Guid.NewGuid()));

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<OrganizationRequest>()), Times.Never);
        }

        [Test]
        public void AddUsersToTeam_WithUsers_ExecutesAddMembersOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<AddMembersTeamRequest>()))
                .Returns(new AddMembersTeamResponse());

            var teamRef = new EntityReference("team", Guid.NewGuid());
            var user1 = new EntityReference("systemuser", Guid.NewGuid());
            var user2 = new EntityReference("systemuser", Guid.NewGuid());

            _sut.AddUsersToTeam(teamRef, user1, user2);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<AddMembersTeamRequest>()), Times.Once);
        }

        [Test]
        public void AddUsersToTeam_WithUsers_RequestContainsCorrectMemberIds()
        {
            AddMembersTeamRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AddMembersTeamRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AddMembersTeamRequest)r)
                .Returns(new AddMembersTeamResponse());

            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            _sut.AddUsersToTeam(teamRef, new EntityReference("systemuser", userId1), new EntityReference("systemuser", userId2));

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(teamRef.Id, capturedRequest.TeamId);
            CollectionAssert.Contains(capturedRequest.MemberIds, userId1);
            CollectionAssert.Contains(capturedRequest.MemberIds, userId2);
        }

        // ─────────────────────────────────────────────────────────────
        //  RemoveUsersFromTeam
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void RemoveUsersFromTeam_ExecutesRemoveMembersOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<RemoveMembersTeamRequest>()))
                .Returns(new RemoveMembersTeamResponse());

            var teamRef = new EntityReference("team", Guid.NewGuid());
            var user = new EntityReference("systemuser", Guid.NewGuid());

            _sut.RemoveUsersFromTeam(teamRef, user);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<RemoveMembersTeamRequest>()), Times.Once);
        }

        [Test]
        public void RemoveUsersFromTeam_RequestContainsCorrectMemberIds()
        {
            RemoveMembersTeamRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RemoveMembersTeamRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RemoveMembersTeamRequest)r)
                .Returns(new RemoveMembersTeamResponse());

            var teamRef = new EntityReference("team", Guid.NewGuid());
            var userId = Guid.NewGuid();

            _sut.RemoveUsersFromTeam(teamRef, new EntityReference("systemuser", userId));

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(teamRef.Id, capturedRequest.TeamId);
            CollectionAssert.Contains(capturedRequest.MemberIds, userId);
        }

        [Test]
        public void RemoveUsersFromTeam_Bypass_SetsRequestFlag()
        {
            RemoveMembersTeamRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RemoveMembersTeamRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RemoveMembersTeamRequest)r)
                .Returns(new RemoveMembersTeamResponse());

            var user = new EntityReference("systemuser", Guid.NewGuid());
            _sut.RemoveUsersFromTeam(new EntityReference("team", Guid.NewGuid()), bypassCustomPluginExecution: true, user);

            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  AddToQueue
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddToQueue_ExecutesOnAdminService()
        {
            AddToQueueRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AddToQueueRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AddToQueueRequest)r)
                .Returns(new AddToQueueResponse());

            var queueId = Guid.NewGuid();
            var target = new EntityReference("email", Guid.NewGuid());

            _sut.AddToQueue(queueId, target);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(queueId, capturedRequest.DestinationQueueId);
            Assert.AreEqual(target, capturedRequest.Target);
        }

        // ─────────────────────────────────────────────────────────────
        //  Merge
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Merge_ExecutesOnAdminService()
        {
            MergeRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<MergeRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (MergeRequest)r)
                .Returns(new MergeResponse());

            var target = new EntityReference("contact", Guid.NewGuid());
            var subordinateId = Guid.NewGuid();
            var content = new Entity("contact");

            _sut.Merge(target, subordinateId, content);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(target, capturedRequest.Target);
            Assert.AreEqual(subordinateId, capturedRequest.SubordinateId);
            Assert.AreEqual(content, capturedRequest.UpdateContent);
            Assert.IsTrue(capturedRequest.PerformParentingChecks);
        }

        // ─────────────────────────────────────────────────────────────
        //  AssociateRecords
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AssociateRecords_ExecutesOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<AssociateRequest>()))
                .Returns(new AssociateResponse());

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var rel = new Microsoft.Xrm.Sdk.Relationship("contact_roles");
            var relatedRef = new EntityReference("role", Guid.NewGuid());

            _sut.AssociateRecords(objectRef, rel, relatedRef);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<AssociateRequest>()), Times.Once);
        }

        [Test]
        public void AssociateRecords_WithBypass_SetsRequestFlag()
        {
            AssociateRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AssociateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AssociateRequest)r)
                .Returns(new AssociateResponse());

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var rel = new Microsoft.Xrm.Sdk.Relationship("contact_roles");

            _sut.AssociateRecords(objectRef, rel, bypassCustomPluginExecution: true,
                new EntityReference("role", Guid.NewGuid()));

            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void AssociateRecords_SetsTargetAndRelatedEntities()
        {
            AssociateRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AssociateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AssociateRequest)r)
                .Returns(new AssociateResponse());

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            var rel = new Microsoft.Xrm.Sdk.Relationship("contact_roles");
            var relatedRef1 = new EntityReference("role", Guid.NewGuid());
            var relatedRef2 = new EntityReference("role", Guid.NewGuid());

            _sut.AssociateRecords(objectRef, rel, relatedRef1, relatedRef2);

            Assert.AreEqual(objectRef, capturedRequest.Target);
            Assert.AreEqual(rel, capturedRequest.Relationship);
            Assert.AreEqual(2, capturedRequest.RelatedEntities.Count);
        }

        // ─────────────────────────────────────────────────────────────
        //  UserHasOneRoleOf / UserHasRole
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UserHasOneRoleOf_GuidArray_ReturnsTrue_WhenEntitiesFound()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { new Entity("role") }));

            var result = _sut.UserHasOneRoleOf(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsTrue(result);
        }

        [Test]
        public void UserHasOneRoleOf_GuidArray_ReturnsFalse_WhenNoEntitiesFound()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection());

            var result = _sut.UserHasOneRoleOf(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsFalse(result);
        }

        [Test]
        public void UserHasOneRoleOf_StringArray_DelegatesToGuidArrayOverload()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { new Entity("role") }));

            var result = _sut.UserHasOneRoleOf(Guid.NewGuid(), Guid.NewGuid().ToString());

            Assert.IsTrue(result);
            _adminOrgServiceMock.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Once);
        }

        [Test]
        public void UserHasRole_DelegatesToUserHasOneRoleOf()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { new Entity("role") }));

            var result = _sut.UserHasRole(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsTrue(result);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetUserRoleIds
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetUserRoleIds_ReturnsRoleTemplateIds_WhenPresent()
        {
            var templateId = Guid.NewGuid();
            var roleEntity = new Entity("role");
            roleEntity[RoleDefinition.Columns.RoleTemplateId] = new EntityReference("roletemplate", templateId);

            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { roleEntity }));

            var result = _sut.GetUserRoleIds(new EntityReference("systemuser", Guid.NewGuid()));

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(templateId));
        }

        [Test]
        public void GetUserRoleIds_FallsBackToParentRootRoleId_WhenTemplateIdAbsent()
        {
            var rootRoleId = Guid.NewGuid();
            var roleEntity = new Entity("role");
            roleEntity[RoleDefinition.Columns.ParentRootRoleId] = new EntityReference("role", rootRoleId);

            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { roleEntity }));

            var result = _sut.GetUserRoleIds(new EntityReference("systemuser", Guid.NewGuid()));

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.Contains(rootRoleId));
        }

        [Test]
        public void GetUserRoleIds_EmptyResult_ReturnsEmptyCollection()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection());

            var result = _sut.GetUserRoleIds(new EntityReference("systemuser", Guid.NewGuid()));

            Assert.IsEmpty(result);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetDefaultCurrencyRef
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetDefaultCurrencyRef_WhenCurrencyFound_ReturnsEntityRef()
        {
            var currencyId = Guid.NewGuid();
            var currency = new Entity("transactioncurrency") { Id = currencyId };
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { currency }));

            var result = _sut.GetDefaultCurrencyRef();

            Assert.IsNotNull(result);
            Assert.AreEqual(currencyId, result.Id);
            Assert.AreEqual("transactioncurrency", result.LogicalName);
        }

        [Test]
        public void GetDefaultCurrencyRef_WhenNoCurrencyFound_ReturnsNull()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection());

            var result = _sut.GetDefaultCurrencyRef();

            Assert.IsNull(result);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetTeamMemberRefs
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetTeamMemberRefs_ReturnsMemberEntityRefs()
        {
            var memberId = Guid.NewGuid();
            var memberEntity = new Entity("systemuser") { Id = memberId };
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { memberEntity }));

            var result = _sut.GetTeamMemberRefs(new EntityReference("team", Guid.NewGuid()));

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(memberId, result.First().Id);
            Assert.AreEqual("systemuser", result.First().LogicalName);
        }

        [Test]
        public void GetTeamMemberRefs_WhenNoMembers_ReturnsEmptyCollection()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection());

            var result = _sut.GetTeamMemberRefs(new EntityReference("team", Guid.NewGuid()));

            Assert.IsEmpty(result);
        }

        // ─────────────────────────────────────────────────────────────
        //  Upsert(Entity) — bypass flag + callerId empty routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Upsert_WithCallerIdGuidEmpty_ExecutesOnAdminOrganizationService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<UpsertRequest>())).Returns(new UpsertResponse());

            _sut.Upsert(new Entity("contact"), Guid.Empty);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<UpsertRequest>()), Times.Once);
        }

        [Test]
        public void Upsert_UseAdmin_BypassCustomPluginExecution_SetsRequestFlag()
        {
            UpsertRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<UpsertRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (UpsertRequest)r)
                .Returns(new UpsertResponse());

            _sut.Upsert(new Entity("contact"), useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void Upsert_UseAdmin_BypassFalse_DoesNotSetRequestFlag()
        {
            UpsertRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<UpsertRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (UpsertRequest)r)
                .Returns(new UpsertResponse());

            _sut.Upsert(new Entity("contact"), useAdmin: false, bypassCustomPluginExecution: false);

            Assert.IsFalse(capturedRequest.Parameters.Contains("BypassCustomPluginExecution"));
        }

        // ─────────────────────────────────────────────────────────────
        //  Update — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Update_BypassCustomPluginExecution_SetsRequestFlag()
        {
            UpdateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<UpdateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (UpdateRequest)r)
                .Returns(new UpdateResponse());

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void Update_BypassFalse_DoesNotSetRequestFlag()
        {
            UpdateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<UpdateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (UpdateRequest)r)
                .Returns(new UpdateResponse());

            _sut.Update(new Entity("contact") { Id = Guid.NewGuid() }, useAdmin: false, bypassCustomPluginExecution: false);

            Assert.IsFalse(capturedRequest.Parameters.Contains("BypassCustomPluginExecution"));
        }

        // ─────────────────────────────────────────────────────────────
        //  Delete — routing + bypass gaps
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Delete_StringGuid_UseAdminTrue_ExecutesDeleteOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete("contact", Guid.NewGuid(), useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void Delete_EntityRef_UseAdminFalse_ExecutesDeleteOnOrgService()
        {
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete(new EntityReference("contact", Guid.NewGuid()), useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void Delete_EntityRefCallerId_GuidEmpty_ExecutesOnAdminService()
        {
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete(new EntityReference("contact", Guid.NewGuid()), Guid.Empty);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
        }

        [Test]
        public void Delete_EntityRef_Bypass_SetsRequestFlag()
        {
            DeleteRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<DeleteRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (DeleteRequest)r)
                .Returns(new DeleteResponse());

            _sut.Delete(new EntityReference("contact", Guid.NewGuid()), useAdmin: true, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void Delete_StringGuid_Bypass_SetsRequestFlag()
        {
            DeleteRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<DeleteRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (DeleteRequest)r)
                .Returns(new DeleteResponse());

            _sut.Delete("contact", Guid.NewGuid(), useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  AssignEntity — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AssignEntity_Bypass_SetsRequestFlag()
        {
            AssignRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AssignRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AssignRequest)r)
                .Returns(new AssignResponse());

            _sut.AssignEntity(new EntityReference("contact", Guid.NewGuid()), null, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  SetState — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SetState_Bypass_SetsRequestFlag()
        {
            SetStateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<SetStateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (SetStateRequest)r)
                .Returns(new SetStateResponse());

            _sut.SetState(new EntityReference("contact", Guid.NewGuid()), 0, 1, useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  Share — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Share_Bypass_SetsRequestFlag()
        {
            GrantAccessRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<GrantAccessRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (GrantAccessRequest)r)
                .Returns(new GrantAccessResponse());

            _sut.Share(new EntityReference("contact", Guid.NewGuid()), new EntityReference("systemuser", Guid.NewGuid()),
                AccessRights.ReadAccess, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  UnShare — bypass flag + callerRef behavior
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UnShare_Bypass_SetsRequestFlag()
        {
            RevokeAccessRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RevokeAccessRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RevokeAccessRequest)r)
                .Returns(new RevokeAccessResponse());

            _sut.UnShare(new EntityReference("contact", Guid.NewGuid()),
                new EntityReference("systemuser", Guid.NewGuid()), bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void UnShare_WithCallerRef_AlwaysExecutesOnAdminService()
        {
            var callerId = Guid.NewGuid();
            var callerService = new Mock<IOrganizationService>();
            _contextMock.Setup(c => c.GetOrganizationService(callerId)).Returns(callerService.Object);
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<RevokeAccessRequest>())).Returns(new RevokeAccessResponse());

            _sut.UnShare(new EntityReference("contact", Guid.NewGuid()),
                new EntityReference("systemuser", Guid.NewGuid()),
                callerRef: new EntityReference("systemuser", callerId));

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<RevokeAccessRequest>()), Times.Once);
            callerService.Verify(s => s.Execute(It.IsAny<OrganizationRequest>()), Times.Never);
        }

        // ─────────────────────────────────────────────────────────────
        //  AddUsersToTeam — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddUsersToTeam_WithBypass_SetsRequestFlag()
        {
            AddMembersTeamRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AddMembersTeamRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AddMembersTeamRequest)r)
                .Returns(new AddMembersTeamResponse());

            var user = new EntityReference("systemuser", Guid.NewGuid());
            _sut.AddUsersToTeam(new EntityReference("team", Guid.NewGuid()), bypassCustomPluginExecution: true, user);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  AddToQueue — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddToQueue_Bypass_SetsRequestFlag()
        {
            AddToQueueRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<AddToQueueRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (AddToQueueRequest)r)
                .Returns(new AddToQueueResponse());

            _sut.AddToQueue(Guid.NewGuid(), new EntityReference("email", Guid.NewGuid()), bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  Merge — bypass flag
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Merge_Bypass_SetsRequestFlag()
        {
            MergeRequest capturedRequest = null;
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<MergeRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (MergeRequest)r)
                .Returns(new MergeResponse());

            _sut.Merge(new EntityReference("contact", Guid.NewGuid()), Guid.NewGuid(), new Entity("contact"), bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        // ─────────────────────────────────────────────────────────────
        //  Retrieve — ColumnSet verification
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Retrieve_StringGuidCols_RequestContainsSpecifiedColumns()
        {
            RetrieveRequest capturedRequest = null;
            var response = new RetrieveResponse();
            response.Results["Entity"] = new Entity("contact");
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RetrieveRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RetrieveRequest)r)
                .Returns(response);

            _sut.Retrieve("contact", Guid.NewGuid(), "firstname", "lastname");

            Assert.IsNotNull(capturedRequest);
            CollectionAssert.Contains(capturedRequest.ColumnSet.Columns, "firstname");
            CollectionAssert.Contains(capturedRequest.ColumnSet.Columns, "lastname");
            Assert.IsFalse(capturedRequest.ColumnSet.AllColumns);
        }

        [Test]
        public void Retrieve_StringGuidAllColumns_RequestUsesAllColumns()
        {
            RetrieveRequest capturedRequest = null;
            var response = new RetrieveResponse();
            response.Results["Entity"] = new Entity("contact");
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RetrieveRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RetrieveRequest)r)
                .Returns(response);

            _sut.Retrieve("contact", Guid.NewGuid(), true);

            Assert.IsNotNull(capturedRequest);
            Assert.IsTrue(capturedRequest.ColumnSet.AllColumns);
        }

        [Test]
        public void Retrieve_EntityRefCols_RequestContainsSpecifiedColumns()
        {
            RetrieveRequest capturedRequest = null;
            var response = new RetrieveResponse();
            response.Results["Entity"] = new Entity("contact");
            _adminOrgServiceMock
                .Setup(s => s.Execute(It.IsAny<RetrieveRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RetrieveRequest)r)
                .Returns(response);

            var objectRef = new EntityReference("contact", Guid.NewGuid());
            _sut.Retrieve(objectRef, "emailaddress1");

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(objectRef, capturedRequest.Target);
            CollectionAssert.Contains(capturedRequest.ColumnSet.Columns, "emailaddress1");
        }

        // ─────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue — happy paths
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetOptionSetNameFromValue_MatchingValue_ReturnsLabel()
        {
            var metadata = new OptionSetMetadata();
            metadata.Options.Add(BuildOption("Active", 1033, 1));
            metadata.Options.Add(BuildOption("Inactive", 1033, 2));
            var response = new RetrieveOptionSetResponse();
            response.Results["OptionSetMetadata"] = metadata;
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<RetrieveOptionSetRequest>())).Returns(response);

            var result = _sut.GetOptionSetNameFromValue("statuscode", 1);

            Assert.AreEqual("Active", result);
        }

        [Test]
        public void GetOptionSetNameFromValue_NoMatchingValue_ReturnsEmptyString()
        {
            var metadata = new OptionSetMetadata();
            metadata.Options.Add(BuildOption("Active", 1033, 1));
            var response = new RetrieveOptionSetResponse();
            response.Results["OptionSetMetadata"] = metadata;
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<RetrieveOptionSetRequest>())).Returns(response);

            var result = _sut.GetOptionSetNameFromValue("statuscode", 99);

            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetOptionSetNameFromValue_PassesOptionsetNameToRequest()
        {
            RetrieveOptionSetRequest capturedRequest = null;
            var metadata = new OptionSetMetadata();
            metadata.Options.Add(BuildOption("Active", 1033, 0));
            var response = new RetrieveOptionSetResponse();
            response.Results["OptionSetMetadata"] = metadata;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<RetrieveOptionSetRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (RetrieveOptionSetRequest)r)
                .Returns(response);

            _sut.GetOptionSetNameFromValue("statuscode", 0);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual("statuscode", capturedRequest.Name);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetTeamMemberRefs — multiple members
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void GetTeamMemberRefs_WithMultipleMembers_ReturnsAllMemberRefs()
        {
            var member1Id = Guid.NewGuid();
            var member2Id = Guid.NewGuid();
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity>
                {
                    new Entity("systemuser") { Id = member1Id },
                    new Entity("systemuser") { Id = member2Id }
                }));

            var result = _sut.GetTeamMemberRefs(new EntityReference("team", Guid.NewGuid()));

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(r => r.Id == member1Id));
            Assert.IsTrue(result.Any(r => r.Id == member2Id));
        }

        // ─────────────────────────────────────────────────────────────
        //  UserHasOneRoleOf — multiple roles
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void UserHasOneRoleOf_MultipleRoleIds_QueryExecutedOnce()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { new Entity("role") }));

            var result = _sut.UserHasOneRoleOf(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            Assert.IsTrue(result);
            _adminOrgServiceMock.Verify(s => s.RetrieveMultiple(It.IsAny<QueryBase>()), Times.Once);
        }

        [Test]
        public void UserHasOneRoleOf_StringArray_MultipleRoles_ReturnsTrue()
        {
            _adminOrgServiceMock
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Returns(new EntityCollection(new List<Entity> { new Entity("role") }));

            var result = _sut.UserHasOneRoleOf(Guid.NewGuid(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            Assert.IsTrue(result);
        }

        // ─────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────

        private static void SetupExecuteReturnsId(Mock<IOrganizationService> mock, Guid id)
        {
            mock.Setup(s => s.Execute(It.IsAny<CreateRequest>()))
                .Returns(BuildCreateResponse(id));
        }

        private static CreateResponse BuildCreateResponse(Guid id)
        {
            var response = new CreateResponse();
            response.Results["id"] = id;
            return response;
        }

        private static void SetupRetrieve(Mock<IOrganizationService> mock, Entity entity)
        {
            var response = new RetrieveResponse();
            response.Results["Entity"] = entity;
            mock.Setup(s => s.Execute(It.IsAny<RetrieveRequest>())).Returns(response);
        }

        private static OptionMetadata BuildOption(string labelText, int languageCode, int value)
        {
            var localizedLabel = new LocalizedLabel(labelText, languageCode);
            var label = new Label(labelText, languageCode);
            label.UserLocalizedLabel = localizedLabel;
            return new OptionMetadata(label, value);
        }
    }
}
