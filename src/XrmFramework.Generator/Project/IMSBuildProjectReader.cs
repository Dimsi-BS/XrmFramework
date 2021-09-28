
namespace XrmFramework.Generator.Project
{
    public interface IMSBuildProjectReader
    {
        XrmFrameworkProject LoadXrmFrameworkProjectFromMsBuild(string projectFilePath, string rootNamespace);
    }
}
