using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System.Linq;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.DeployUtils.Tests
{
    [TestClass]
    public class RegistrationServiceTests
    {
        private readonly RegistrationService _service;

        private readonly Mock<IOrganizationService> _mockClient = new();

        public RegistrationServiceTests()
        {
            _service = new RegistrationService(_mockClient.Object);
        }

        [TestMethod]
        public void RetrieveAll_ShouldBe_Empty()
        {
            // Arrange
            _mockClient.Setup(x => x.RetrieveMultiple(It.IsAny<QueryExpression>()))
                .Returns(new EntityCollection());

            var query = new QueryExpression("");

            // Act
            var result = _service.RetrieveAll(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());

            _mockClient.Verify(x => x.RetrieveMultiple(query), Times.Once);
        }

        [TestMethod]
        public void RetrieveAll_ShouldCall_Twice()
        {
            // Arrange
            var collection = new EntityCollection()
            {
                MoreRecords = true
            };
            _mockClient.SetupSequence(x => x.RetrieveMultiple(It.IsAny<QueryExpression>()))
                    .Returns(collection)
                    .Returns(new EntityCollection());

            var query = new QueryExpression("");

            // Act
            var result = _service.RetrieveAll(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());

            _mockClient.Verify(x => x.RetrieveMultiple(query), Times.Exactly(2));
        }

        [TestMethod]
        public void RetrieveAll_ShouldHave_OneComponent()
        {
            // Arrange
            var collection = new EntityCollection()
            {
                Entities = { new Entity() }
            };
            _mockClient.Setup(x => x.RetrieveMultiple(It.IsAny<QueryExpression>()))
                .Returns(collection);

            var query = new QueryExpression("");

            // Act
            var result = _service.RetrieveAll(query);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);

            _mockClient.Verify(x => x.RetrieveMultiple(query), Times.Once);
        }
    }
}
