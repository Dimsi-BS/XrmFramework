using System;
using System.Linq;
using Moq;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.CrmComponentsTests;

[TestClass]
public class AssemblyInfoTests
{
	/*
	 * Unique Properties hard coding
	 */
	private const string Name = "thisAssembly";
	private const string Culture = "culture";
	private const string PublicKeyToken = "token";
	private const string Version = "version";
	private const string Description = "description";

	private const string EntityTypeName = "pluginassembly";
	private const ModeDIsolation IsolationMode = ModeDIsolation.Sandbox;
	private const TypeDeSource SourceType = TypeDeSource.Database;
	private readonly AssemblyInfo _component;
	private readonly byte[] _content = "content"u8.ToArray();

	public AssemblyInfoTests()
	{
		_component = new AssemblyInfo
		{
			Name = Name,
			Version = Version,
			SourceType = SourceType,
			IsolationMode = IsolationMode,
			Culture = Culture,
			PublicKeyToken = PublicKeyToken,
			Description = Description,
			Content = _content
		};
	}

	[TestMethod]
	public void UniquePropertiesTests()
	{
		Assert.AreEqual(Name, _component.Name);
		Assert.AreEqual(Version, _component.Version);
		Assert.AreEqual(SourceType, _component.SourceType);
		Assert.AreEqual(IsolationMode, _component.IsolationMode);
		Assert.AreEqual(Culture, _component.Culture);
		Assert.AreEqual(PublicKeyToken, _component.PublicKeyToken);
		Assert.AreEqual(Description, _component.Description);
		Assert.AreEqual(_content, _component.Content);
	}

	[TestMethod]
	public void CrmPropertiesTests()
	{
		Assert.AreEqual(EntityTypeName, _component.EntityTypeName);
		Assert.AreEqual(Name, _component.UniqueName);
		Assert.AreEqual(0, _component.Rank);
		Assert.AreEqual(true, _component.DoAddToSolution);
		Assert.AreEqual(false, _component.DoFetchTypeCode);
	}

	[TestMethod]
	public void ChildrenTests()
	{
		Assert.IsTrue(_component.Children != null);
		Assert.IsFalse(_component.Children.Any());
	}

	[TestMethod]
	public void AddChildTests()
	{
		var anyComponent = new Mock<ICrmComponent>().Object;
		
		Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent),
			"AssemblyInfo doesn't take children");
	}
}