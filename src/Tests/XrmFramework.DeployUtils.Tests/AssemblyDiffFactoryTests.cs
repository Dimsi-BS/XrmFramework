using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using XrmFramework.DeployUtils.Comparers;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests;

[TestClass]
public class AssemblyDiffFactoryTests
{
    private readonly AssemblyDiffFactory _diffFactory;

    private readonly Mock<ICrmComponentComparer> _mockComparer;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAssemblyContext> _from;

    private Mock<IAssemblyContext> _target = new Mock<IAssemblyContext>();
    
    private Guid _targetId;
    private Guid _targetParentId;
    private Guid _targetAssemblyId;

    public AssemblyDiffFactoryTests()
    {
        _mockComparer = new Mock<ICrmComponentComparer>();
        _mockMapper = new Mock<IMapper>();

        // Arrange
        _from = new Mock<IAssemblyContext>();

        _diffFactory = new AssemblyDiffFactory(_mockComparer.Object, _mockMapper.Object);
    }

    [TestInitialize]
    private void TestInitialize()
    {
        _mockComparer.Reset();
        _mockMapper.Reset();

        _targetId = Guid.NewGuid();
        _targetParentId = Guid.NewGuid();
        _targetAssemblyId = Guid.NewGuid();
    }

    [TestMethod]
    public void OneComponent_Vs_Null()
    {

        _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
          .Returns(_from.Object);

        _from.Setup(f => f.Children)
          .Returns(new List<ICrmComponent>());

        // Act
        _diffFactory.ComputeDiffPatch(_from.Object, null);

        // Assert
        _from.VerifyGet(from => from.Children, Times.Once);
    }

    [TestMethod]
    public void TwoComponents_Vs_Null()
    {

        var child = new Mock<ICrmComponent>();

        _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
          .Returns(_from.Object);

        _from.Setup(f => f.Children)
          .Returns(new List<ICrmComponent>() { child.Object });

        child.Setup(c => c.Children)
          .Returns(new List<ICrmComponent>());

        // Act
        _diffFactory.ComputeDiffPatch(_from.Object, null);

        // Assert
        _from.VerifyGet(from => from.Children, Times.Once);

        child.VerifySet(from => from.RegistrationState = RegistrationState.ToCreate, Times.Once);
        child.VerifyGet(from => from.Children, Times.Once);
    }

    [TestMethod]
    public void DiffTest_TwoCustomApis()
    {

        var fromCustomApi = new CustomApi()
        {
            Id = Guid.NewGuid(),
            ParentId = Guid.NewGuid(),
            AssemblyId = Guid.NewGuid()
        };

        var _targetCustomApi = new CustomApi()
        {
            Id = _targetId,
            ParentId = _targetParentId,
            AssemblyId = _targetAssemblyId
        };

        _mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
          .Returns(_from.Object);

        _from.Setup(m => m.ComponentsOrderedPool)
          .Returns(new List<ICrmComponent>() { fromCustomApi });

        _target.Setup(m => m.ComponentsOrderedPool)
          .Returns(new List<ICrmComponent>() { _targetCustomApi });

        _mockComparer.Setup(m =>
            m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
          .Returns(_targetCustomApi);

        _mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
          .Returns(true);

        // Act
        _diffFactory.ComputeDiffPatch(_from.Object, _target.Object);

        // Assert
        Assert.AreEqual(RegistrationState.ToUpdate, fromCustomApi.RegistrationState);
        Assert.AreEqual(RegistrationState.Computed, _targetCustomApi.RegistrationState);

        Assert.AreEqual(_targetId, fromCustomApi.Id);
        Assert.AreEqual(_targetParentId, fromCustomApi.ParentId);

        Assert.AreEqual(_targetId, _targetCustomApi.Id);
        Assert.AreEqual(_targetParentId, _targetCustomApi.ParentId);
    }

    [TestMethod]
    public void DiffTest_CustomApiToDelete()
    {
        // Arrange

        var assemblyInfo = new AssemblyInfo()
        {
            Id = _targetAssemblyId,
            RegistrationState = RegistrationState.NotComputed
        };

        var _targetCustomApi = new CustomApi()
        {
            Id = _targetId,
            ParentId = _targetParentId,
            AssemblyId = _targetAssemblyId,
            RegistrationState = RegistrationState.NotComputed
        };

        _mockMapper.Setup(x => x.Map<IAssemblyContext>(_from.Object))
          .Returns(_from.Object);

        _mockMapper.Setup(x => x.Map<IAssemblyContext>(_target.Object))
          .Returns(_target.Object);

        _from.Setup(m => m.ComponentsOrderedPool)
          .Returns(new List<ICrmComponent>() { assemblyInfo });


        _target.Setup(m => m.ComponentsOrderedPool)
          .Returns(new List<ICrmComponent>() { assemblyInfo, _targetCustomApi });

        _target.Setup(m => m.AssemblyInfo)
                .Returns(assemblyInfo);

        _mockComparer.Setup(m =>
            m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
          .Returns((ICrmComponent)null);

        _mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
          .Returns(true);

        // Act
        _diffFactory.ComputeDiffPatch(_from.Object, _target.Object);

        // Assert
        Assert.AreEqual(RegistrationState.ToDelete, _targetCustomApi.RegistrationState);

        Assert.AreEqual(_targetId, _targetCustomApi.Id);
        Assert.AreEqual(_targetParentId, _targetCustomApi.ParentId);

        _from.Verify(m => m.AddChild(It.IsAny<CustomApi>()), Times.Once);
    }
}