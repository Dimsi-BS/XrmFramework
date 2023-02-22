using System;
using System.Collections.Generic;
using AutoMapper;
using Moq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Tests;

[TestClass]
public class AssemblyDiffFactoryTests
{
	private readonly AssemblyDiffFactory _diffFactory;

	private readonly Mock<ICrmComponentComparer> _mockComparer;
	private readonly Mock<IMapper> _mockMapper;


	public AssemblyDiffFactoryTests()
	{
		_mockComparer = new Mock<ICrmComponentComparer>();
		_mockMapper = new Mock<IMapper>();

		_diffFactory = new AssemblyDiffFactory(_mockComparer.Object, _mockMapper.Object);
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
		_diffFactory.ComputeDiffPatch(from.Object, null);

		// Assert
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
			.Returns(new List<ICrmComponent>() {child.Object});

		child.Setup(c => c.Children)
			.Returns(new List<ICrmComponent>());

		// Act
		_diffFactory.ComputeDiffPatch(from.Object, null);

		// Assert
		from.VerifyGet(from => from.Children, Times.Once);

		child.VerifySet(from => from.RegistrationState = RegistrationState.ToCreate, Times.Once);
		child.VerifyGet(from => from.Children, Times.Once);
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
			AssemblyId = Guid.NewGuid()
		};

		var targetCustomApi = new CustomApi()
		{
			Id = targetId,
			ParentId = targetParentId,
			AssemblyId = targetAssemblyId
		};

		_mockMapper.Setup(x => x.Map<IAssemblyContext>(It.IsAny<IAssemblyContext>()))
			.Returns(from.Object);

		from.Setup(m => m.ComponentsOrderedPool)
			.Returns(new List<ICrmComponent>() {fromCustomApi});

		target.Setup(m => m.ComponentsOrderedPool)
			.Returns(new List<ICrmComponent>() {targetCustomApi});

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

		var assemblyInfo = new AssemblyInfo()
		{
			Id = targetAssemblyId,
			RegistrationState = RegistrationState.NotComputed
		};

		var targetCustomApi = new CustomApi()
		{
			Id = targetId,
			ParentId = targetParentId,
			AssemblyId = targetAssemblyId,
			RegistrationState = RegistrationState.NotComputed
		};

		_mockMapper.Setup(x => x.Map<IAssemblyContext>(from.Object))
			.Returns(from.Object);
		
		_mockMapper.Setup(x => x.Map<IAssemblyContext>(target.Object))
			.Returns(target.Object);

		from.Setup(m => m.ComponentsOrderedPool)
			.Returns(new List<ICrmComponent>() { assemblyInfo});


		target.Setup(m => m.ComponentsOrderedPool)
			.Returns(new List<ICrmComponent>() {assemblyInfo, targetCustomApi});


		_mockComparer.Setup(m =>
				m.CorrespondingComponent(It.IsAny<ICrmComponent>(), It.IsAny<IReadOnlyCollection<ICrmComponent>>()))
			.Returns((ICrmComponent) null);

		_mockComparer.Setup(m => m.NeedsUpdate(It.IsAny<ICrmComponent>(), It.IsAny<ICrmComponent>()))
			.Returns(true);

		// Act
		_diffFactory.ComputeDiffPatch(from.Object, target.Object);

		// Assert
		Assert.AreEqual(RegistrationState.ToDelete, targetCustomApi.RegistrationState);

		Assert.AreEqual(targetId, targetCustomApi.Id);
		Assert.AreEqual(targetParentId, targetCustomApi.ParentId);

		from.Verify(m => m.AddChild(It.IsAny<CustomApi>()), Times.Once);
	}
}