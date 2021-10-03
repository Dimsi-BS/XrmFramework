namespace XrmFramework.Generator.Project
{
    public class MSBuildProjectReader : IMSBuildProjectReader
    {
        private readonly IXrmFrameworkProjectReader _projectReader;

        public MSBuildProjectReader(IXrmFrameworkProjectReader projectReader)
        {
            _projectReader = projectReader;
        }

        public XrmFrameworkProject LoadXrmFrameworkProjectFromMsBuild(string projectFilePath, string rootNamespace)
        {
            return _projectReader.ReadXrmFrameworkProject(projectFilePath, rootNamespace);
        }
    }
}
