// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using Microsoft.Crm.Sdk.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Services.Description;
using Microsoft.Xrm.Sdk.Query;
using Moq;

namespace Plugins.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DefaultServiceTests : ServiceTestClass<DefaultService>
    {
        [TestMethod]
        [ServiceMethodName("AssignEntity")]
        public void AssignEntityOk()
        {
            var objectRef = GetRef("entityName");
            var ownerRef = GetRef("entityName");

            AdminOrganizationService.Setup(s => s.Execute(It.IsAny<AssignRequest>())).Callback<OrganizationRequest>(r =>
            {
                Assert.IsInstanceOfType(r, typeof(AssignRequest));

                var request = r as AssignRequest;

                Assert.AreEqual(request.Target, objectRef);
                Assert.AreEqual(request.Assignee, ownerRef);
            }).Returns(new AssignResponse());

            Service.AssignEntity(objectRef, ownerRef);

            AdminOrganizationService.Setup(s => s.Execute(It.IsAny<AssignRequest>())).Callback<OrganizationRequest>(r =>
            {
                Assert.IsInstanceOfType(r, typeof(AssignRequest));

                var request = r as AssignRequest;

                Assert.AreEqual(request.Assignee.Id, ServiceContext.InitiatingUserId);
            }).Returns(new AssignResponse());

            Service.AssignEntity(objectRef, null);
        }

        [TestMethod]
        [ServiceMethodName("AssignEntity")]
        public void AssignEntityKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.AssignEntity(null, null);
            });
        }

        [TestMethod]
        [ServiceMethodName("Create")]
        public void CreateOk()
        {
            var entity = new Entity();
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            OrganizationService.Setup(s => s.Create(It.IsAny<Entity>())).Callback<Entity>(e =>
            {
                Assert.AreEqual<Entity>(e, entity);
            }).Returns(id1);

            AdminOrganizationService.Setup(s => s.Create(It.IsAny<Entity>())).Callback<Entity>(e =>
            {
                Assert.AreEqual<Entity>(e, entity);
            }).Returns(id2);

            Assert.AreEqual(id1, Service.Create(entity, false));
            Assert.AreEqual(id2, Service.Create(entity, true));
        }

        [TestMethod]
        [ServiceMethodName("Create")]
        public void CreateKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.Create(null);
            });
        }

        [TestMethod]
        [ServiceMethodName("Delete")]
        public void DeleteOk()
        {
            var name = "test";
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            OrganizationService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Guid>())).Callback<string, Guid>((entityName, id) =>
            {
                Assert.AreEqual<string>(name, entityName);
                Assert.AreEqual<Guid>(id1, id);
            });
            AdminOrganizationService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Guid>())).Callback<string, Guid>((entityName, id) =>
            {
                Assert.AreEqual<string>(name, entityName);
                Assert.AreEqual<Guid>(id2, id);
            });

            Service.Delete(name, id1, false);
            Service.Delete(name, id2, true);
        }

        [TestMethod]
        [ServiceMethodName("Delete")]
        public void DeleteKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.Delete(null, Guid.Empty);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Delete("test", Guid.Empty);
            });
        }

        [TestMethod]
        [ServiceMethodName("GetOptionSetNameFromValue")]
        public void GetOptionSetNameFromValueOk()
        {
            var optionSetName = "name";
            var value = 12;
            var name = "value";

            OrganizationService.Setup(s => s.Execute(It.IsAny<OrganizationRequest>())).Callback<OrganizationRequest>(request =>
            {
                Assert.IsInstanceOfType(request, typeof(RetrieveOptionSetRequest));
                var r = (RetrieveOptionSetRequest)request;

                Assert.AreEqual(r.Name, optionSetName);


            }).Returns(() =>
            {
                var options = new OptionMetadataCollection();
                options.Add(new OptionMetadata(new Label(new LocalizedLabel("autre", 1033), null), 18));
                options.Add(new OptionMetadata(new Label(new LocalizedLabel(name, 1033), null), value));

                var response = new RetrieveOptionSetResponse();
                response.Results["OptionSetMetadata"] = new OptionSetMetadata(options);
                return response;
            });

            var result = Service.GetOptionSetNameFromValue(optionSetName, value);

            Assert.AreEqual(result, name);
        }

        [TestMethod]
        [ServiceMethodName("GetOptionSetNameFromValue")]
        public void GetOptionSetNameFromValueKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                string value = null;
                Service.GetOptionSetNameFromValue(value, 12);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.GetOptionSetNameFromValue("test", -1);
            });
        }

        [TestMethod]
        [ServiceMethodName("Retrieve")]
        public void RetrieveOk()
        {
            var entityRef = GetRef("name");
            var entity = new Entity();
            var columns = new string[] { "column1", "column2" };

            AdminOrganizationService.Setup(s => s.Execute(It.IsAny<OrganizationRequest>())).Callback<OrganizationRequest>(r =>
            {
                var request = r as RetrieveRequest;
                if (request == null)
                {
                    Assert.Fail("Should be a RetrieveRequest");
                }

                Assert.AreEqual<EntityReference>(request.Target, entityRef);

                for (var i = 0; i < columns.Length; i++)
                {
                    Assert.AreEqual<string>(request.ColumnSet.Columns[i], columns[i]);
                }
            }).Returns(new RetrieveResponse() { ["Entity"] = entity });

            var result = Service.Retrieve(entityRef, columns);

            Assert.AreEqual(result, entity);

            AdminOrganizationService.Setup(s => s.Execute(It.IsAny<OrganizationRequest>())).Callback<OrganizationRequest>(r =>
            {
                var request = r as RetrieveRequest;
                if (request == null)
                {
                    Assert.Fail("Should be a RetrieveRequest");
                }

                Assert.AreEqual<EntityReference>(request.Target, entityRef);

                Assert.AreEqual<bool>(request.ColumnSet.AllColumns, true);

            }).Returns(new RetrieveResponse() { ["Entity"] = entity });

            result = Service.Retrieve(entityRef, true);

            Assert.AreEqual(result, entity);
        }

        [TestMethod]
        [ServiceMethodName("Retrieve")]
        public void RetrieveKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.Retrieve(null, true);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Retrieve(null, new string[] { "column1", "column2" });
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Retrieve(GetRef("test"), new string[] { });
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Retrieve(GetRef("test"), null);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Retrieve(GetRef("test"), false);
            });
        }

        [TestMethod]
        public void TestAssertException()
        {
            AssertException<ArgumentNullException>(() =>
            {
                throw new ArgumentNullException("param");
            }, (e) => { Assert.AreEqual(e.ParamName, "param"); });
        }

        [TestMethod]
        [ServiceMethodName("SetState")]
        public void SetStateOk()
        {
            var entityRef = GetRef("name");
            var stateCode = 12;
            var statusCode = 59;

            CheckExecute<SetStateRequest, SetStateResponse>(OrganizationService, (r) =>
            {
                Assert.AreEqual(r.EntityMoniker, entityRef);
                Assert.AreEqual(r.State.Value, stateCode);
                Assert.AreEqual(r.Status.Value, statusCode);
            });

            Service.SetState(entityRef, stateCode, statusCode, false);

            CheckExecute<SetStateRequest, SetStateResponse>(AdminOrganizationService, (r) =>
            {
                Assert.AreEqual(r.EntityMoniker, entityRef);
                Assert.AreEqual(r.State.Value, stateCode);
                Assert.AreEqual(r.Status.Value, statusCode);
            });

            Service.SetState(entityRef, stateCode, statusCode, true);
        }

        [TestMethod]
        [ServiceMethodName("SetState")]
        public void SetStateKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.SetState(null, 11, 12);
            });
        }

        [TestMethod]
        [ServiceMethodName("Update")]
        public void UpdateOk()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();

            OrganizationService.Setup(s => s.Update(It.IsAny<Entity>())).Callback<Entity>(e =>
            {
                Assert.ReferenceEquals(e, entity);
            });

            AdminOrganizationService.Setup(s => s.Update(It.IsAny<Entity>())).Callback<Entity>(e =>
            {
                Assert.ReferenceEquals(e, entity);
            });

            Service.Update(entity);

            OrganizationService.Verify(s => s.Update(It.IsAny<Entity>()));

            Service.Update(entity, true);

            AdminOrganizationService.Verify(s => s.Update(It.IsAny<Entity>()));
        }

        [TestMethod]
        [ServiceMethodName("Update")]
        public void UpdateKo()
        {
            AssertException<ArgumentNullException>(() =>
            {
                Service.Update(null);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Update(new Entity());
            });
        }

        [TestMethod]
        [ServiceMethodName("Share")]
        public void ShareEntityOk()
        {
            var objectRef = GetRef("entityName");
            var userRef = GetRef("entityName");
            var accessRights = AccessRights.CreateAccess | AccessRights.WriteAccess;

            CheckExecute<GrantAccessRequest, GrantAccessResponse>(AdminOrganizationService, (req) =>
            {
                Assert.AreEqual(req.Target, objectRef);
                Assert.AreEqual(req.PrincipalAccess.Principal, userRef);
                Assert.AreEqual(req.PrincipalAccess.AccessMask, accessRights);
            });

            Service.Share(objectRef, userRef, accessRights);
        }

        [TestMethod]
        [ServiceMethodName("Share")]
        public void ShareEntityKo()
        {
            var entityReference = GetRef("entityName");

            AssertException<ArgumentNullException>(() =>
            {
                Service.Share(entityReference, null, AccessRights.ReadAccess);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.Share(null, null, AccessRights.ReadAccess);
            });
        }

        [TestMethod]
        [ServiceMethodName("UnShare")]
        public void UnShareEntityOk()
        {
            var objectRef = GetRef("entityName");
            var userRef = GetRef("entityName");

            CheckExecute<RevokeAccessRequest, RevokeAccessResponse>(AdminOrganizationService, (req) =>
            {
                Assert.AreEqual(req.Target, objectRef);
                Assert.AreEqual(req.Revokee, userRef);
            });

            Service.UnShare(objectRef, userRef);
        }

        [TestMethod]
        [ServiceMethodName("UnShare")]
        public void UnShareEntityKo()
        {
            var entityReference = GetRef("entityName");

            AssertException<ArgumentNullException>(() =>
            {
                Service.UnShare(entityReference, null);
            });
            AssertException<ArgumentNullException>(() =>
            {
                Service.UnShare(null, null);
            });
        }
    }
}
