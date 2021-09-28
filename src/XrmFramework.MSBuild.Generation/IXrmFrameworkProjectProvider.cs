using XrmFramework.Generator.Project;

namespace XrmFramework.MSBuild.Generation
{
    public interface IXrmFrameworkProjectProvider
    {
        XrmFrameworkProject GetXrmFrameworkProject();
    }
}
