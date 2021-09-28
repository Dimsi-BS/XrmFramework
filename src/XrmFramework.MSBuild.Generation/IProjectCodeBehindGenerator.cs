using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace XrmFramework.MSBuild.Generation
{
    public interface IProjectCodeBehindGenerator
    {
        IReadOnlyCollection<ITaskItem> GenerateCodeBehindFilesForProject();
    }
}
