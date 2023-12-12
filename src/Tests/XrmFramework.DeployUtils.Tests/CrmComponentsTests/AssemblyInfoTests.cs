using System;
using System.Linq;
using System.Text;
using Moq;
using XrmFramework;
using XrmFramework.DeployUtils.Model;

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
	private const ModeDIsolation IsolationMode = ModeDIsolation.BacASableSandbox;
	private const TypeDeSource SourceType = TypeDeSource.BaseDeDonnees;
	private readonly AssemblyInfo _component;
	private readonly byte[] Content = Encoding.UTF8.GetBytes("content");

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
			Content = Content
		};
	}

	[TestMethod]
	public void UniquePropertiesTests()
	{
		Assert.AreEqual(_component.Name, Name);
		Assert.AreEqual(_component.Version, Version);
		Assert.AreEqual(_component.SourceType, SourceType);
		Assert.AreEqual(_component.IsolationMode, IsolationMode);
		Assert.AreEqual(_component.Culture, Culture);
		Assert.AreEqual(_component.PublicKeyToken, PublicKeyToken);
		Assert.AreEqual(_component.Description, Description);
		Assert.AreEqual(_component.Content, Content);
	}

	[TestMethod]
	public void CrmPropertiesTests()
	{
		Assert.AreEqual(_component.EntityTypeName, EntityTypeName);
		Assert.AreEqual(_component.UniqueName, Name);
		Assert.AreEqual(_component.Rank, 0);
		Assert.AreEqual(_component.DoAddToSolution, true);
		Assert.AreEqual(_component.DoFetchTypeCode, false);
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
		Action<ICrmComponent> addChild = _component.AddChild;
		Assert.ThrowsException<ArgumentException>(() => _component.AddChild(anyComponent),
			"AssemblyInfo doesn't take children");
	}
}