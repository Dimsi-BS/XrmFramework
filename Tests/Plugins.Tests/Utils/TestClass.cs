
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Reflection;
using FakeXrmEasy;
using FakeXrmEasy.FakeMessageExecutors;
using Microsoft.Xrm.Sdk.Deployment;
using Moq;

namespace Plugins.Tests
{
    [TestClass]
    public abstract class TestClass
    {
        protected Mock<IOrganizationService> OrganizationService { get; private set; }

        protected Mock<IOrganizationService> AdminOrganizationService { get; private set; }

        protected MockServiceContext ServiceContext { get; private set; }

        private TraceLogger Log = new TraceLogger(Console.WriteLine);

        #region Test Context
        /// <summary>
        /// Obtient ou définit le contexte de test qui fournit
        /// des informations sur la série de tests active ainsi que ses fonctionnalités.
        /// </summary>
        public TestContext TestContext { get; set; }
        #endregion

        #region My Test Initialize
        /// <summary>
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            OrganizationService = new Mock<IOrganizationService>();
            AdminOrganizationService = new Mock<IOrganizationService>();

            ServiceContext = new MockServiceContext(OrganizationService.Object, AdminOrganizationService.Object, Log);

            InitializeTest(ServiceContext);

        }
        #endregion

        protected abstract void InitializeTest(IServiceContext context);

        protected void AssertException<T>(Action call, Action<T> checkException = null, string message = null) where T : Exception
        {
            var okException = false;
            try
            {
                call();
            }
            catch (T e)
            {
                checkException?.Invoke(e);
                okException = true;
            }
            catch (TargetInvocationException te)
            {
                checkException?.Invoke(te.InnerException as T);
                okException = true;
            }
            finally
            {
                if (message == null)
                {
                    message = $"Exception {typeof(T).Name} not raised";
                }
                Assert.IsTrue(okException, message);
            }
        }

        protected void AssertValue<T>(Entity e, string columnName, T value)
        {
            Assert.IsTrue(e.Contains(columnName), $"The entity does not contain the column {columnName}");

            var entityValue = e.GetAttributeValue<T>(columnName);

            Assert.AreEqual(entityValue, value, $"The values for column {columnName} is different from the specified value");
        }

        protected EntityCollectionHelper CheckRetrieveMultiple(Mock<IOrganizationService> service, Action<QueryExpression> checkQuery)
        {
            var list = new EntityCollectionHelper();

            service
                .Setup(s => s.RetrieveMultiple(It.IsAny<QueryBase>()))
                .Callback<QueryBase>(q =>
                {
                    Assert.IsInstanceOfType(q, typeof(QueryExpression));

                    var query = q as QueryExpression;

                    checkQuery(query);
                })
                .Returns(new EntityCollection(list.List));

            return list;
        }

        protected TU CheckExecute<T, TU>(Mock<IOrganizationService> service, Action<T> checkRequest)
            where T : OrganizationRequest, new()
            where TU : OrganizationResponse, new()
        {
            var response = new TU();

            service
                .Setup(s => s.Execute(It.IsAny<OrganizationRequest>()))
                .Callback<OrganizationRequest>((r) =>
                {
                    Assert.IsInstanceOfType(r, typeof(T));

                    var request = r as T;

                    checkRequest(request);
                })
                .Returns(response);

            return response;
        }

        protected class EntityCollectionHelper
        {
            List<Entity> _list = new List<Entity>();

            public List<Entity> List
            {
                get { return _list; }
            }
            public EntityCollectionHelper Add(string entityName, params At[] attributes)
            {
                _list.Add(new EntityHelper(entityName, attributes));
                return this;
            }
            public EntityCollectionHelper Add(string entityName, Guid id, params At[] attributes)
            {
                _list.Add(new EntityHelper(entityName, id, attributes));
                return this;
            }
        }

        protected class At
        {
            public At(string columnName, object value)
            {
                ColumnName = columnName;
                Value = value;
            }
            public string ColumnName { get; private set; }
            public object Value { get; private set; }
        }

        protected class EntityHelper : Entity
        {
            public EntityHelper(string entityName, params At[] attributes)
                : base(entityName)
            {
                foreach (At a in attributes)
                {
                    this[a.ColumnName] = a.Value;
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public EntityHelper(string entityName, Guid id, params At[] attributes)
                : base(entityName)
            {
                foreach (At a in attributes)
                {
                    this[a.ColumnName] = a.Value;
                }
                Id = id;
            }

            public EntityHelper(EntityReference objectRef, params At[] attributes)
                : this(objectRef.LogicalName, objectRef.Id, attributes)
            {
            }

            public EntityHelper Add(string columnName, object value)
            {
                this[columnName] = value;
                return this;
            }

            public Entity ToTrueEntity()
            {
                var entity = new Entity(this.LogicalName);

                foreach (string attributeName in Attributes.Keys)
                {
                    entity[attributeName] = this[attributeName];
                }
                return entity;
            }
        }

        protected EntityReference GetRef(string entityName)
        {
            return new EntityReference(entityName, Guid.NewGuid());
        }
    }
}
