using System;

namespace XrmFramework.DeployUtils.Configuration;

[AttributeUsage(AttributeTargets.Assembly)]
public class DeployFolderAttribute : Attribute
{
	public DeployFolderAttribute(string path)
	{
		Path = path;
	}

	public string Path { get; }
}