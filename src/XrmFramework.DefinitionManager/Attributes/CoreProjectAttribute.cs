using System;

namespace DefinitionManager;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class CoreProjectAttribute : Attribute
{
	public string RootFolder { get; }
	public string Name { get; }
	public CoreProjectAttribute(string rootFolder, string name)
	{
		RootFolder = rootFolder;
		Name = name;
	}
}
