using AutoMapper;
using Moq;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Tests
{
    [TestClass]
    public class AssemblyDiffFactoryTests
    {

        private readonly AssemblyDiffFactory _diffFactory;

        private readonly Mock<ICrmComponentComparer> _mockComparer;
        private readonly Mock<IMapper> _mockMapper;


        public AssemblyDiffFactoryTests()
        {
            _mockComparer = new();
            _mockMapper = new();

            _diffFactory = new(_mockComparer.Object, _mockMapper.Object);
        }

        [TestMethod]
        public void OneComponent_Vs_Null()
        {
            // Arrange
            var from = new Mock<IAssemblyContext>();

            _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
                .Returns(from.Object);

            from.Setup(f => f.Children)
                .Returns(new List<ICrmComponent>());

            // Act
            var result = _diffFactory.ComputeDiffPatch(from.Object, null);

            // Assert
            from.VerifySet(from => from.RegistrationState = RegistrationState.ToCreate, Times.Once);
            from.VerifyGet(from => from.Children, Times.Once);
        }

        [TestMethod]
        public void TwoComponents_Vs_Null()
        {
            // Arrange
            var from = new Mock<IAssemblyContext>();

            var child = new Mock<ICrmComponent>();

            _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
                .Returns(from.Object);

            from.Setup(f => f.Children)
                .Returns(new List<ICrmComponent>() { child.Object });

            child.Setup(c => c.Children)
                .Returns(new List<ICrmComponent>());

            // Act
            var result = _diffFactory.ComputeDiffPatch(from.Object, null);

            // Assert
            from.VerifySet(from => from.RegistrationState = RegistrationState.ToCreate, Times.Once);
            from.VerifyGet(from => from.Children, Times.Once);

            child.VerifySet(from => from.RegistrationState = RegistrationState.ToCreate, Times.Once);
            child.VerifyGet(from => from.Children, Times.Once);

        }


        [TestMethod]
        public void TwoEmptyAssemblies()
        {
            // Arrange
            var from = new Mock<IAssemblyContext>();
            var target = new Mock<IAssemblyContext>();

            from.SetupProperty(m => m.RegistrationState);

            target.SetupProperty(m => m.RegistrationState);

            _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
                .Returns(from.Object);

            from.Setup(f => f.Children)
                .Returns(new List<ICrmComponent>());

            target.Setup(f => f.Children)
                .Returns(new List<ICrmComponent>());

            from.Setup(f => f.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { from.Object });

            target.Setup(f => f.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { target.Object });

            _mockComparer.Setup(m =>
                    m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
                .Returns(target.Object);

            _mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
                .Returns(false);

            // Act
            var result = _diffFactory.ComputeDiffPatch(from.Object, target.Object);

            // Assert
            from.VerifySet(f => f.RegistrationState = RegistrationState.NotComputed, Times.Once);

            target.VerifySet(t => t.RegistrationState = RegistrationState.NotComputed, Times.Once);

            from.VerifyGet(f => f.Children, Times.Once);
            target.VerifyGet(t => t.Children, Times.Once);

            from.VerifyGet(m => m.ComponentsOrderedPool, Times.Once);
            target.VerifyGet(m => m.ComponentsOrderedPool, Times.Once);

            _mockComparer.Verify(m =>
                m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()), Times.Once);

            _mockComparer.Verify(m =>
                m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()), Times.Once);

            Assert.AreEqual(RegistrationState.Ignore, from.Object.RegistrationState);
            Assert.AreEqual(RegistrationState.Computed, target.Object.RegistrationState);
        }

    }
}
