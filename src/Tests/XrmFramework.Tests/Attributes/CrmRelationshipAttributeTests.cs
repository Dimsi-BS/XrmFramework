using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmFramework.BindingModel;

namespace XrmFramework.Tests.Attributes
{
    [TestClass]
    public class CrmRelationshipAttributeTests
    {
        [TestMethod]
        public void CrmRelationshipAttribute_ConstructorWithRelationshipNameAndRole_SetsRelationshipNameAndRole()
        {
            // Arrange
            string relationshipName = "TestRelationship";
            EntityRole role = EntityRole.Referencing;

            // Act
            CrmRelationshipAttribute attribute = new CrmRelationshipAttributeTestClass(relationshipName, role);

            // Assert
            Assert.AreEqual(relationshipName, attribute.RelationshipName);
            Assert.AreEqual(role, attribute.Role);
        }

        [TestMethod]
        public void CrmRelationshipAttribute_IsValidForUpdateProperty_CanBeSetAndGet()
        {
            // Arrange
            CrmRelationshipAttribute attribute = new CrmRelationshipAttributeTestClass("TestRelationship", EntityRole.Referencing);

            // Act
            attribute.IsValidForUpdate = false;

            // Assert
            Assert.IsFalse(attribute.IsValidForUpdate);
        }

        [TestMethod]
        public void ChildRelationshipAttribute_ConstructorWithRelationshipName_SetsRelationshipNameAndRole()
        {
            // Arrange
            string relationshipName = "TestChildRelationship";

            // Act
            ChildRelationshipAttribute attribute = new ChildRelationshipAttribute(relationshipName);

            // Assert
            Assert.AreEqual(relationshipName, attribute.RelationshipName);
            Assert.AreEqual(EntityRole.Referenced, attribute.Role);
        }

        [TestMethod]
        public void ManyToManyRelationshipAttribute_ConstructorWithRelationshipName_SetsRelationshipNameAndRole()
        {
            // Arrange
            string relationshipName = "TestManyToManyRelationship";

            // Act
            ManyToManyRelationshipAttribute attribute = new ManyToManyRelationshipAttribute(relationshipName);

            // Assert
            Assert.AreEqual(relationshipName, attribute.RelationshipName);
            Assert.AreEqual(EntityRole.Referenced, attribute.Role);
        }

        [TestMethod]
        public void ManyToManyRelationshipAttribute_UpdateStrategyProperty_CanBeSetAndGet()
        {
            // Arrange
            ManyToManyRelationshipAttribute attribute = new ManyToManyRelationshipAttribute("TestManyToManyRelationship");

            // Act
            attribute.UpdateStrategy = UpdateStrategy.Add;

            // Assert
            Assert.AreEqual(UpdateStrategy.Add, attribute.UpdateStrategy);
        }

        [TestMethod]
        public void CrmRelationshipAttribute_GetRelation()
        {
            // Arrange
            CrmRelationshipAttributeTestClass attribute = new CrmRelationshipAttributeTestClass("TestManyToManyRelationship", EntityRole.Referenced);

            // Act
            var relation = attribute.GetRelationship();

            // Assert
            Assert.AreEqual("TestManyToManyRelationship", relation.SchemaName);
            Assert.AreEqual(EntityRole.Referenced, relation.PrimaryEntityRole);
        }
    }

    // Helper class to expose protected members for testing
    public class CrmRelationshipAttributeTestClass : CrmRelationshipAttribute
    {
        public CrmRelationshipAttributeTestClass(string relationshipName, EntityRole role) : base(relationshipName, role) { }

        public string GetPrivateRelationshipName()
        {
            return RelationshipName;
        }

        public EntityRole GetPrivateRole()
        {
            return Role;
        }
    }
}
