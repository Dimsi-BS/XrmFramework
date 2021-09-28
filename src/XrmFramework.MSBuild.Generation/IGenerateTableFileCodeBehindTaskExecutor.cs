using System.Collections.Generic;
using Microsoft.Build.Framework;
using XrmFramework.CommonModels;

namespace XrmFramework.MSBuild.Generation
{
    public interface IGenerateTableFileCodeBehindTaskExecutor
    {
        IResult<IReadOnlyCollection<ITaskItem>> Execute();
    }
}
