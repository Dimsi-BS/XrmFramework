using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Plugins.Tests.Objects
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LocalContextTests : TestClass
    {
        private MockServiceProvider ServiceProvider { get; set; }

        private MockPluginExecutionContext PluginExecutionContext { get; set; }

        private LocalContext LocalContext { get; set; }
        protected override void InitializeTest(IServiceContext context)
        {
            ServiceProvider = new MockServiceProvider();
            PluginExecutionContext = ServiceProvider.PluginExecutionContext;

            LocalContext = new LocalContext(ServiceProvider.Object);
        }

        [TestMethod]
        public void ConstructorTestKo()
        {
            AssertException<ArgumentException>(() => {
                IServiceProvider s = null;
                new LocalContext(s); 
            });
        }

        [TestMethod]
        public void ModeTests()
        {
            PluginExecutionContext.SetupGet(s => s.Mode).Returns(45);

            AssertException<InvalidPluginExecutionException>(() =>
            {
                var mode = LocalContext.Mode;
            });

            PluginExecutionContext.SetupGet(s => s.Mode).Returns(0);

            var modeOk = LocalContext.Mode;

        }

        [TestMethod]
        public void MessageTests()
        {
            PluginExecutionContext.SetupGet(s => s.MessageName).Returns("Machin");

            AssertException<InvalidPluginExecutionException>(() =>
            {
                var message = LocalContext.MessageName;
            });

            PluginExecutionContext.SetupGet(s => s.MessageName).Returns("Update");

            var messageOk = LocalContext.MessageName;
        }

        [TestMethod]
        public void InputParametersTests()
        {
            var entity = new EntityHelper("machin"
                , new At("column1", new OptionSetValue(1))
                , new At("column2", GetRef("name"))
                , new At("column3", 12)
                , new At("column4", null)
                ).ToTrueEntity();
            
            PluginExecutionContext.InputParameters.Add("Target", entity);

            PluginExecutionContext.MessageName = "Create";
            LocalContext.DumpInputParameters();

            Assert.AreSame(entity, LocalContext.GetInputParameter<Entity>(InputParameters.Target));

            PluginExecutionContext.InputParameters.Clear();
            PluginExecutionContext.InputParameters.Add("EntityMoniker", new EntityReference());
            PluginExecutionContext.InputParameters.Add("State", new OptionSetValue());
            PluginExecutionContext.InputParameters.Add("Status", new OptionSetValue());
            PluginExecutionContext.MessageName = "SetState";
            LocalContext.DumpInputParameters();

            PluginExecutionContext.InputParameters.Clear();
            PluginExecutionContext.InputParameters.Add("Target", new EntityReference());
            PluginExecutionContext.InputParameters.Add("Assignee", new EntityReference());
            PluginExecutionContext.MessageName = "Assign";
            LocalContext.DumpInputParameters();
        }

        [TestMethod]
        public void SharedVariablesTests()
        {
            var entity = new EntityHelper("machin"
                , new At("column1", new OptionSetValue(1))
                , new At("column2", GetRef("name"))
                , new At("column3", 12)
                , new At("column4", null)
                ).ToTrueEntity();
            PluginExecutionContext.SharedVariables.Add("Target", entity);
            
            LocalContext.DumpSharedVariables();

            Assert.AreSame(entity, LocalContext.GetSharedVariable<Entity>("Target"));

            Assert.IsTrue(LocalContext.HasSharedVariable("Target"));

            PluginExecutionContext.SharedVariables.Clear();

            LocalContext.SetSharedVariable<Entity>("Target", entity);

            Assert.AreSame(entity, LocalContext.GetSharedVariable<Entity>("Target"));
        }

        [TestMethod]
        public void OutputParametersTests()
        {
            var entity = new EntityHelper("machin"
                , new At("column1", new OptionSetValue(1))
                , new At("column2", GetRef("name"))
                , new At("column3", 12)
                , new At("column4", null)
                ).ToTrueEntity();

            PluginExecutionContext.OutputParameters.Add("BusinessEntityCollection", entity);

            Assert.AreSame(entity, LocalContext.GetOutputParameter<Entity>(OutputParameters.BusinessEntityCollection));

            PluginExecutionContext.OutputParameters.Clear();

            LocalContext.SetOutputParameter<Entity>(OutputParameters.BusinessEntityCollection, entity);

            Assert.AreSame(entity, LocalContext.GetOutputParameter<Entity>(OutputParameters.BusinessEntityCollection));
        }

        [TestMethod]
        public void ImagesTests()
        {
            var entity = new EntityHelper("machin"
                , new At("column1", new OptionSetValue(1))
                , new At("column2", GetRef("name"))
                , new At("column3", 12)
                , new At("column4", null)
                ).ToTrueEntity();

            PluginExecutionContext.PreEntityImages.Add("PreImage", entity);
            
            Assert.AreSame(entity, LocalContext.GetPreImage("PreImage"));

            Assert.IsTrue(LocalContext.HasPreImage("PreImage"));

            PluginExecutionContext.PreEntityImages.Clear();

            AssertException<ArgumentNullException>(() =>
            {
                LocalContext.GetPreImage("PreImage");
            });
            
            PluginExecutionContext.PostEntityImages.Add("PostImage", entity);

            Assert.AreSame(entity, LocalContext.GetPostImage("PostImage"));

            Assert.IsTrue(LocalContext.HasPostImage("PostImage"));

            PluginExecutionContext.PostEntityImages.Clear();

            AssertException<ArgumentNullException>(() =>
            {
                LocalContext.GetPostImage("PostImage");
            });
        }

        //[TestMethod]
        public void TestThrowLocalizedError()
        {
            int languageCode = 1033;
            int userLanguageCode = 1036;
            //string languageSuffix = "en_US";
            //string userLanguageSuffix = "fr_FR";
            var userId = Guid.NewGuid();

            var xml = "<root><data name='messageName'><value>LocalizedLabel</value></data></root>";

            CheckRetrieveMultiple(OrganizationService, (query) =>
            {
                Assert.AreEqual("organization", query.EntityName);
                query.ColumnSet.AssertColumns("languagecode");
            }).Add("organization", new At("languagecode", languageCode));

            CheckRetrieveMultiple(OrganizationService, (query) =>
            {
                Assert.AreEqual("usersettings", query.EntityName);
                query.ColumnSet.AssertColumns("uilanguageid", "systemuserid");
                query.AssertCondition("systemuserid", userId);
            }).Add("usersettings", new At("uilanguageid", userLanguageCode));

            CheckRetrieveMultiple(OrganizationService, (query) =>
            {
                Assert.AreEqual("webresource", query.EntityName);
                query.ColumnSet.AssertColumns("content");
                query.AssertCondition("name", string.Format("mcs_/xml/PluginMessages.{0}.xml", userLanguageCode));
            }).Add("webresource", new At("content", Convert.ToBase64String(Encoding.UTF8.GetBytes(xml))));

            AssertException<InvalidPluginExecutionException>(() =>
            {
                LocalContext.ThrowInvalidPluginException("messageName");
            }, (e) => { Assert.AreEqual("LocalizedLabel", e.Message); });
        }
    }
}
