// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Moq;
using NUnit.Framework;

namespace XrmFramework.Tests.Service
{
    [TestFixture]
    public class DefaultServiceTests
    {
        private Mock<IServiceContext> _contextMock;
        private Mock<IOrganizationService> _orgServiceMock;
        private Mock<IOrganizationService> _adminOrgServiceMock;
        private DefaultService _sut;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<IServiceContext>();
            _orgServiceMock = new Mock<IOrganizationService>();
            _adminOrgServiceMock = new Mock<IOrganizationService>();

            _contextMock.Setup(c => c.OrganizationService).Returns(_orgServiceMock.Object);
            _contextMock.Setup(c => c.AdminOrganizationService).Returns(_adminOrgServiceMock.Object);
            _contextMock.Setup(c => c.LogServiceMethod).Returns((string _, string __, object[] ___) => { });
            _contextMock.Setup(c => c.UserId).Returns(Guid.NewGuid());
            _contextMock.Setup(c => c.InitiatingUserId).Returns(Guid.NewGuid());
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
        //  Create — happy path / service routing
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void Create_UseAdminFalse_ExecutesOnOrganizationService()
        {
            var expectedId = Guid.NewGuid();
            var entity = new Entity("contact");
            SetupExecuteReturnsId(_orgServiceMock, expectedId);

            var result = _sut.Create(entity, useAdmin: false);

            Assert.AreEqual(expectedId, result);
            _orgServiceMock.Verify(s => s.Execute(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Test]
        public void Create_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            var expectedId = Guid.NewGuid();
            var entity = new Entity("contact");
            SetupExecuteReturnsId(_adminOrgServiceMock, expectedId);

            var result = _sut.Create(entity, useAdmin: true);

            Assert.AreEqual(expectedId, result);
            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<CreateRequest>()), Times.Once);
        }

        [Test]
        public void Create_WithCallerIdGuidEmpty_ExecutesOnAdminOrganizationService()
        {
            var expectedId = Guid.NewGuid();
            var entity = new Entity("contact");
            SetupExecuteReturnsId(_adminOrgServiceMock, expectedId);

            var result = _sut.Create(entity, Guid.Empty);

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
            var entity = new Entity("contact");
            CreateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<CreateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (CreateRequest)r)
                .Returns(BuildCreateResponse(Guid.NewGuid()));

            _sut.Create(entity, useAdmin: false, bypassCustomPluginExecution: true);

            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(true, capturedRequest["BypassCustomPluginExecution"]);
        }

        [Test]
        public void Create_BypassFalse_DoesNotSetRequestFlag()
        {
            var entity = new Entity("contact");
            CreateRequest capturedRequest = null;
            _orgServiceMock
                .Setup(s => s.Execute(It.IsAny<CreateRequest>()))
                .Callback<OrganizationRequest>(r => capturedRequest = (CreateRequest)r)
                .Returns(BuildCreateResponse(Guid.NewGuid()));

            _sut.Create(entity, useAdmin: false, bypassCustomPluginExecution: false);

            Assert.IsFalse(capturedRequest.Parameters.Contains("BypassCustomPluginExecution"));
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
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            _orgServiceMock.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());

            _sut.Update(entity, useAdmin: false);

            _orgServiceMock.Verify(s => s.Execute(It.IsAny<UpdateRequest>()), Times.Once);
        }

        [Test]
        public void Update_UseAdminTrue_ExecutesOnAdminOrganizationService()
        {
            var entity = new Entity("contact") { Id = Guid.NewGuid() };
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<UpdateRequest>())).Returns(new UpdateResponse());

            _sut.Update(entity, useAdmin: true);

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
        //  Delete(EntityReference, ...) — guard clauses
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

        // ─────────────────────────────────────────────────────────────
        //  Delete — happy path
        // ─────────────────────────────────────────────────────────────

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
            var objRef = new EntityReference("contact", Guid.NewGuid());
            _adminOrgServiceMock.Setup(s => s.Execute(It.IsAny<DeleteRequest>())).Returns(new DeleteResponse());

            _sut.Delete(objRef, useAdmin: true);

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<DeleteRequest>()), Times.Once);
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
        //  Retrieve — happy path (always uses AdminOrganizationService)
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
            var objectRef = new EntityReference("contact", Guid.NewGuid());

            var result = _sut.Retrieve(objectRef, "firstname");

            Assert.AreSame(expected, result);
        }

        // ─────────────────────────────────────────────────────────────
        //  GetOptionSetNameFromValue — guard clauses
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
        //  AssignEntity — guard clauses
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AssignEntity_NullObjectReference_Throws()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.AssignEntity(null, new EntityReference("systemuser", Guid.NewGuid())));
        }

        // ─────────────────────────────────────────────────────────────
        //  Share — guard clauses
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

        // ─────────────────────────────────────────────────────────────
        //  UnShare — guard clauses
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

        // ─────────────────────────────────────────────────────────────
        //  AddUsersToTeam — empty array short-circuits
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void AddUsersToTeam_EmptyArray_DoesNotCallExecute()
        {
            _sut.AddUsersToTeam(new EntityReference("team", Guid.NewGuid()));

            _adminOrgServiceMock.Verify(s => s.Execute(It.IsAny<OrganizationRequest>()), Times.Never);
        }

        // ─────────────────────────────────────────────────────────────
        //  SetState — guard clause
        // ─────────────────────────────────────────────────────────────

        [Test]
        public void SetState_NullObjectRef_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.SetState(null, 1, 1));
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
    }
}
