using System.Collections.Generic;

namespace XrmFramework.MSBuild.Generation
{
    public interface ITableFileCodeBehindGenerator
    {
        IEnumerable<string> GenerateFilesForProject(IReadOnlyCollection<string> tableFiles, string projectFolder, string outputPath);
    }
}
