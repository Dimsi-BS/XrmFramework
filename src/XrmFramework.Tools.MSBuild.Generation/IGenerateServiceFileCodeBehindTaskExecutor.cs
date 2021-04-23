using System.Collections.Generic;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.CommonModels;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IGenerateServiceFileCodeBehindTaskExecutor
    {
        IResult<IReadOnlyCollection<ITaskItem>> Execute();
    }
}
