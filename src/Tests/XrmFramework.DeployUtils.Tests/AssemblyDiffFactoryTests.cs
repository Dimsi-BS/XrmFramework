using AutoMapper;
using Moq;
using System;
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

            var targetId = Guid.NewGuid();
            var targetParentId = Guid.NewGuid();

            from.SetupProperty(m => m.RegistrationState);
            target.SetupProperty(m => m.RegistrationState);

            target.SetupProperty(m => m.Id, targetId);
            target.SetupProperty(m => m.ParentId, targetParentId);

            from.SetupProperty(m => m.Id, Guid.NewGuid());
            from.SetupProperty(m => m.ParentId, Guid.NewGuid());

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

            Assert.AreEqual(targetId, result.Id);
            Assert.AreEqual(targetParentId, result.ParentId);

            Assert.AreEqual(targetId, target.Object.Id);
            Assert.AreEqual(targetParentId, target.Object.ParentId);
        }

        [TestMethod]
        public void DiffTest_TwoCustomApis()
        {
            // Arrange
            var from = new Mock<IAssemblyContext>();
            var target = new Mock<IAssemblyContext>();

            var targetId = Guid.NewGuid();
            var targetParentId = Guid.NewGuid();
            var targetAssemblyId = Guid.NewGuid();

            var fromCustomApi = new CustomApi()
            {
                Id = Guid.NewGuid(),
                ParentId = Guid.NewGuid(),
                AssemblyId = Guid.NewGuid(),
            };

            var targetCustomApi = new CustomApi()
            {
                Id = targetId,
                ParentId = targetParentId,
                AssemblyId = targetAssemblyId,
            };

            _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
                .Returns(from.Object);

            from.Setup(m => m.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { fromCustomApi });

            target.Setup(m => m.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { targetCustomApi });

            _mockComparer.Setup(m =>
                    m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
                .Returns(targetCustomApi);

            _mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
                .Returns(true);

            // Act
            _diffFactory.ComputeDiffPatch(from.Object, target.Object);

            // Assert
            Assert.AreEqual(RegistrationState.ToUpdate, fromCustomApi.RegistrationState);
            Assert.AreEqual(RegistrationState.Computed, targetCustomApi.RegistrationState);

            Assert.AreEqual(targetId, fromCustomApi.Id);
            Assert.AreEqual(targetParentId, fromCustomApi.ParentId);

            Assert.AreEqual(targetId, targetCustomApi.Id);
            Assert.AreEqual(targetParentId, targetCustomApi.ParentId);
        }

        [TestMethod]
        public void DiffTest_CustomApiToDelete()
        {
            // Arrange
            var from = new Mock<IAssemblyContext>();
            var target = new Mock<IAssemblyContext>();

            var targetId = Guid.NewGuid();
            var targetParentId = Guid.NewGuid();
            var targetAssemblyId = Guid.NewGuid();

            var targetCustomApi = new CustomApi()
            {
                Id = targetId,
                ParentId = targetParentId,
                AssemblyId = targetAssemblyId,
                RegistrationState = RegistrationState.NotComputed
            };

            _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
                .Returns(from.Object);

            from.Setup(m => m.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { from.Object });

            from.SetupProperty(f => f.RegistrationState,
                RegistrationState.NotComputed);

            from.SetupProperty(f => f.Id,
                Guid.NewGuid());

            from.SetupProperty(f => f.ParentId,
                Guid.NewGuid());

            target.Setup(m => m.ComponentsOrderedPool)
                .Returns(new List<ICrmComponent>() { target.Object, targetCustomApi });


            target.SetupProperty(f => f.RegistrationState,
                RegistrationState.NotComputed);

            target.SetupProperty(f => f.Id,
                targetAssemblyId);

            target.SetupProperty(f => f.ParentId,
                Guid.NewGuid());

            _mockComparer.Setup(m =>
                    m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
                .Returns(target.Object);

            _mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
                .Returns(true);

            // Act
            _diffFactory.ComputeDiffPatch(from.Object, target.Object);

            // Assert
            Assert.AreEqual(RegistrationState.ToDelete, targetCustomApi.RegistrationState);

            Assert.AreEqual(targetId, targetCustomApi.Id);
            Assert.AreEqual(targetParentId, targetCustomApi.ParentId);

            from.VerifySet(m => m.RegistrationState = RegistrationState.ToUpdate, Times.Once);
            from.VerifySet(m => m.Id = targetAssemblyId, Times.Once);

            from.Verify(m => m.AddChild(It.IsAny<CustomApi>()), Times.Once);

        }
    }
}
