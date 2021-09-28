using System;
using System.IO;

namespace XrmFramework.Generator.Project
{
    public class ProjectReader : IXrmFrameworkProjectReader
    {
        private readonly ProjectLanguageReader _languageReader;

        public ProjectReader(ProjectLanguageReader languageReader)
        {
            _languageReader = languageReader;
        }

        public XrmFrameworkProject ReadXrmFrameworkProject(string projectFilePath, string rootNamespace)
        {
            try
            {
                var projectFolder = Path.GetDirectoryName(projectFilePath);

                var xrmFrameworkProject = new XrmFrameworkProject();
                xrmFrameworkProject.ProjectSettings.ProjectFolder = projectFolder;
                xrmFrameworkProject.ProjectSettings.ProjectName = Path.GetFileNameWithoutExtension(projectFilePath);
                xrmFrameworkProject.ProjectSettings.DefaultNamespace = rootNamespace;
                xrmFrameworkProject.ProjectSettings.ProjectPlatformSettings.Language = _languageReader.GetLanguage(projectFilePath);

                return xrmFrameworkProject;
            }
            catch (Exception e)
            {
                throw new Exception("Error when reading project file.", e);
            }
        }
        
    }
}