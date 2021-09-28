namespace XrmFramework.Generator.Project
{
    public interface IXrmFrameworkProjectReader
    {
        XrmFrameworkProject ReadXrmFrameworkProject(string projectFilePath, string rootNamespace);
    }
}