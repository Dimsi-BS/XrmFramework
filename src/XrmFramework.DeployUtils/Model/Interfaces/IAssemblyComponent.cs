namespace XrmFramework.DeployUtils.Model;

public interface IAssemblyComponent : ICrmComponent
{
	string HumanName { get; }
	string AssemblyName { get; }
}