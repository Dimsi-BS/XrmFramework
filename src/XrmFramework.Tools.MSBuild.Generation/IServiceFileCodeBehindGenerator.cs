using System.Collections.Generic;
using BoDi;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IServiceFileCodeBehindGenerator
    {
        IEnumerable<string> GenerateFilesForProject(IReadOnlyCollection<string> featureFiles, string projectFolder, string outputPath);

    }
}
