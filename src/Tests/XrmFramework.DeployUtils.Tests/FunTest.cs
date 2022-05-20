using Moq;
using System;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Tests.Mocks;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Tests
{
    /// <summary>
    /// Moq tests to get used to its behavior
    /// </summary>
    [TestClass]
    public class FunTest
    {
        public AssemblyDiffFactory AssemblyDiff;


        [TestInitialize]
        public void InitTests()
        {

        }

        [TestMethod]
        public void MockTest1()
        {
            var component = new CrmComponentMock().Mock.Object;
            Assert.AreEqual(RegistrationState.NotComputed, component.RegistrationState);

            component.RegistrationState = RegistrationState.Computed;

            Assert.AreEqual(RegistrationState.Computed, component.RegistrationState);
        }

        [TestMethod]
        public void MockTest2()
        {
            var component = new CrmComponentMock().Mock.Object;
            Assert.AreEqual(RegistrationState.NotComputed, component.RegistrationState);

            component.RegistrationState = RegistrationState.ToCreate;

            Assert.AreNotEqual(RegistrationState.Computed, component.RegistrationState);
        }

        [TestMethod]
        public void MockTypeTest()
        {
            var component1 = new Mock<ICrmComponent>().Object;

            var component2 = new Mock<ICrmComponent>().Object;

            Assert.IsTrue(component2.GetType() == component1.GetType());
        }

        [TestMethod]
        public void AssemblyNameTests()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var AssemblyName = assembly.FullName;

            var anyType = assembly.GetTypes().First();

            var anyTypeAssemblyQualifiedName = anyType.AssemblyQualifiedName;

            var anyTypeAssemblyName = anyType.Assembly.FullName;

            var sameAssembly = Assembly.Load(AssemblyName);

            var sameType = sameAssembly.GetType(anyType.FullName);

            var sameTypeQuestionMark = Type.GetType(anyType.FullName);

            Assert.AreEqual(AssemblyName, anyTypeAssemblyQualifiedName);
        }
    }
}
