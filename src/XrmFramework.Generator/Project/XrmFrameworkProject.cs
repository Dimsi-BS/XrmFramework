
using XrmFramework.Generator.Interfaces;

namespace XrmFramework.Generator.Project
{
    public class XrmFrameworkProject
    {
        public ProjectSettings ProjectSettings { get; set; }

        public XrmFrameworkProject()
        {
            ProjectSettings = new ProjectSettings();
        }
    }
}