namespace XrmFramework.DeployUtils.Model.Interfaces;

public interface IAssemblyComponent : ICrmComponent
{
	string HumanName { get; }
	string AssemblyName { get; }
}