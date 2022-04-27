using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework;

namespace XrmFramework.Plugin.tests
{
    public enum TestEnum
    {
        Option1,
        [System.ComponentModel.Description("Description for option 2")]
        Option2,
        Option3,
        [ExternalValue("option 4")]
        Option4,

    }

    [TestClass]
    public class Extensions
    {


        [TestMethod]
        public void CollectionExtension()
        {
            List<int> testCollection = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                testCollection.Add(i);
            }
            int counter = 0;
            foreach (List<int> testList in testCollection.SplitList())
            {
                Assert.AreEqual(1000, testList.Count);
                for (int i = 0; i < testList.Count; i++)
                {
                    Assert.AreEqual(counter * 1000 + i, testList[i]);
                }

                counter++;
            }

        }


        [TestMethod]
        public void DecimalExtensions()
        {


            decimal? testD = 10.2m;
            decimal? testD2 = null;



            Money testM2 = new Money(testD.Value);
            Assert.AreEqual(testD.ToMoney(), testM2);

            Assert.IsNull(testD2.ToMoney());
        }

        [TestMethod]
        public void EntityExtensions()
        {
            var testEntity = new Entity();

            OptionSetValue optionSetValue = new OptionSetValue((int)TestEnum.Option3);
            List<TestEnum> testEnumList = new List<TestEnum>();
            testEnumList.Add(TestEnum.Option1);
            testEnumList.Add(TestEnum.Option2);
            testEnumList.Add(TestEnum.Option3);

            testEntity.Attributes["a1"] = 50;
            testEntity.Attributes["a2"] = null;
            testEntity.Attributes["optionSetValue"] = optionSetValue;
            testEntity.Attributes["optionSetValues"] = testEnumList;

            Assert.IsNull(testEntity.GetAttributeValue<Object>("nonExistentValue"));
            Assert.AreEqual(50, testEntity.GetAttributeValue<int>("a1"));
            Assert.IsNull(testEntity.GetAttributeValue<Object>("a2"));
            Assert.AreEqual(TestEnum.Option3, testEntity.GetOptionSetValue<TestEnum>("optionSetValue"));

            var enumValues = testEntity.GetAttributeValue<List<TestEnum>>("optionSetValues");
            for (int i = 0; i < enumValues.Count; i++)
            {
                Assert.AreEqual(testEnumList[i], enumValues[i]);
            }
            testEnumList.Clear();
            var testEnumList2 = new List<TestEnum>();
            testEnumList2.Add(TestEnum.Option4);
            testEnumList2.Add(TestEnum.Option4);
            testEnumList2.Add(TestEnum.Option4);

            //testEnumList.Add(TestEnum.Option4);
            //testEnumList.Add(TestEnum.Option4);
            //testEnumList.Add(TestEnum.Option4);

            for (int i = 0; i < enumValues.Count; i++)
            {
                Assert.AreNotEqual((int)testEnumList2[i], (int)enumValues[i]);
            }
            testEntity.SetOptionSetValues<TestEnum>("optionSetValues", testEnumList2);
            for (int i = 0; i < enumValues.Count; i++)
            {
                Assert.AreEqual((int)testEnumList2[i], (int)enumValues[i]);
            }
            testEntity.SetOptionSetValue<TestEnum>("optionSetValue", TestEnum.Option1);
            Assert.AreEqual(TestEnum.Option1, testEntity.GetOptionSetValue<TestEnum>("optionSetValue"));


            var eList = new List<Entity>();
            eList.Add(new Entity("e1", Guid.NewGuid()));
            eList.Add(new Entity("e2", Guid.NewGuid()));
            eList.Add(new Entity("e3", Guid.NewGuid()));
            eList.Add(new Entity("e4", Guid.NewGuid()));

            var testCollection = new EntityCollection(eList);
            testCollection.EmptyIds();
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(Guid.Empty, testCollection[i].Id);
            }

            var testPreimage = new Entity();
            testEntity.CopyField(testPreimage, "a1", "a3");
            Assert.AreEqual(testPreimage.Attributes["a3"], testEntity.Attributes["a1"]);

            //testEntity.MergeWith(testPreimage);
            testPreimage.MergeWith(testEntity);

            foreach (var key in testEntity.Attributes.Keys)
            {
                Assert.AreEqual(testEntity.Attributes[key], testPreimage.Attributes[key]);
            }
            var testE1 = new Entity();
            var testE2 = new Entity();

            testE1.Attributes["a1"] = 1;
            testE1.Attributes["a2"] = 2;
            testE2.Attributes["a3"] = 3;
            testE2.Attributes["a4"] = 4;
            var testE3 = testE1.Merge(testE2);

            foreach (var key in testE1.Attributes.Keys)
            {
                Assert.IsTrue(testE3.Contains(key));
            }
            foreach (var key in testE2.Attributes.Keys)
            {
                Assert.IsTrue(testE3.Contains(key));
            }





        }

        [TestMethod]
        public void EntityReferenceExtensions()
        {
            EntityReference nullE = null;
            Assert.IsNull(nullE.ToEntity());

            var testE = new EntityReference();
            testE.LogicalName = "test";
            testE.Id = Guid.NewGuid();
            testE.KeyAttributes["1"] = "a1";
            testE.KeyAttributes["2"] = "a2";
            testE.KeyAttributes["3"] = "a3";

            var testEE = testE.ToEntity();
            Assert.AreEqual(testEE.Id, testE.Id);
            Assert.AreEqual(testEE.LogicalName, testE.LogicalName);
            foreach (var key in testE.KeyAttributes.Keys)
            {
                Assert.AreEqual(testE.KeyAttributes[key], testEE.KeyAttributes[key]);
            }
        }

        [TestMethod]
        public void EnumExtensions()
        {
            Assert.AreEqual(TestEnum.Option2, (1).ToEnum<TestEnum>());
            Assert.AreEqual("Description for option 2", TestEnum.Option2.GetDescription());
            Assert.AreEqual(TestEnum.Option2, TestEnum.Option2.GetDescription().ParseDescription<TestEnum>());
            Assert.AreEqual(TestEnum.Option3, TestEnum.Option3.ToString().ToEnum<TestEnum>());
            Assert.AreEqual("option 4", TestEnum.Option4.GetExternalValue());
            Assert.AreEqual(TestEnum.Option4, TestEnum.Option4.GetExternalValue().ParseExternalValue<TestEnum>());
            Assert.AreEqual(3, TestEnum.Option4.ToInt());
            Assert.AreEqual(new OptionSetValue(3), TestEnum.Option4.ToOptionSetValue());

            var oc = new OptionSetValueCollection();
            oc.Add(new OptionSetValue(1));
            oc.Add(new OptionSetValue(2));
            oc.Add(new OptionSetValue(3));

            var el = new List<TestEnum>();
            el.Add(TestEnum.Option2);
            el.Add(TestEnum.Option3);
            el.Add(TestEnum.Option4);

            var elo = el.ToOptionSetValueCollection<TestEnum>();
            for (int i = 0; i < elo.Count; i++)
            {
                Assert.AreEqual(oc.ElementAt(i), elo[i]);
            }
            var oce = oc.ToEnumCollection(typeof(TestEnum));
            for (int i = 0; i < oce.Count; i++)
            {
                Assert.AreEqual(el[i], oce.ElementAt(i));
            }
        }

        [TestMethod]
        public void GuidExtensions()
        {
            var testGuid = Guid.NewGuid();
            var testEntityRef = testGuid.ToEntityReference("testEntityRefName");
            Assert.AreEqual(testGuid, testEntityRef.Id);
            Assert.AreEqual("testEntityRefName", testEntityRef.LogicalName);
        }

        [TestMethod]
        public void QueryExpressionExtensions()
        {
            var qTest = new QueryExpression();
            var conditionExpression = new ConditionExpression("attribute", ConditionOperator.ChildOf);
            qTest.Criteria.Conditions.Add(conditionExpression);
            qTest.Criteria.Conditions.Add(new ConditionExpression("nfzl", ConditionOperator.ContainValues));
            Assert.AreEqual(conditionExpression, qTest.GetRootFilterExpression().Conditions.First());
            //qTest.Criteria.GetConditionValue<>("attribute");
            //Tester GetConditionValue, je sais pas encore comment faire

        }

        [TestMethod]
        public void StringExtensions()
        {
            var testString = "testString";
            var testGuid = "0f8fad5b-d9cb-469f-a165-70867728950e";
            var testStringTrimmed = testString.TrimIfTooLong(4);
            var guid = new Guid(testGuid);
            string testVoidString = "";
            String testNullString = null; 
            var testE1 = testGuid.ToEntityReference("logical name");
            var testE2 = testGuid.ToEntityReference("logical name 2", "value");
            Assert.AreEqual(guid, testGuid.ToGuid());
            Assert.ThrowsException<ArgumentNullException>(()=>testVoidString.ShouldNotBeNull());
            Assert.ThrowsException<ArgumentNullException>(() => testNullString.ShouldNotBeNull());
            Assert.AreEqual(guid, testE1.Id);
            Assert.AreEqual("logical name", testE1.LogicalName);
            Assert.AreEqual(testGuid, testE2.KeyAttributes["value"]);
            Assert.AreEqual(testString, testString.ToRelationship().SchemaName);
            Assert.AreEqual("test", testStringTrimmed);
            Assert.AreEqual(4,testStringTrimmed.Length);

        }
    }
}
