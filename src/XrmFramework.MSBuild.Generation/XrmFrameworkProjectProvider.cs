using System.IO;

namespace XrmFramework.MSBuild.Generation
{
    public class XrmFrameworkProjectProvider : IXrmFrameworkProjectProvider
    {
        private readonly IMSBuildProjectReader _msbuildProjectReader;
        private readonly XrmFrameworkProjectInfo _xrmFrameworkProjectInfo;

        public XrmFrameworkProjectProvider(IMSBuildProjectReader msbuildProjectReader, XrmFrameworkProjectInfo xrmFrameworkProjectInfo)
        {
            _msbuildProjectReader = msbuildProjectReader;
            _xrmFrameworkProjectInfo = xrmFrameworkProjectInfo;
        }

        public XrmFrameworkProject GetXrmFrameworkProject()
        {
            var xrmFrameworkProject = _msbuildProjectReader.LoadXrmFrameworkProjectFromMsBuild(Path.GetFullPath(_xrmFrameworkProjectInfo.ProjectPath), _xrmFrameworkProjectInfo.RootNamespace);
            return xrmFrameworkProject;
        }
    }
}
