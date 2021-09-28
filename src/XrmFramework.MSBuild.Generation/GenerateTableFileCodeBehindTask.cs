

using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace XrmFramework.MSBuild.Generation
{
    public class GenerateTableFileCodeBehindTask : Task
    {
        public ITableFileCodeBehindGenerator CodeBehindGenerator { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] TableFiles { get; set; }

        public ITaskItem[] GeneratorPlugins { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }

        public string MSBuildVersion { get; set; }
        public string AssemblyName { get; set; }
        public string TargetFrameworks { get; set; }
        public string TargetFramework { get; set; }
        public string ProjectGuid { get; set; }

        public override bool Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}
