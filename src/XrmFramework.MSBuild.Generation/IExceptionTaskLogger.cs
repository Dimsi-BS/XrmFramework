using System;

namespace XrmFramework.MSBuild.Generation
{
    public interface IExceptionTaskLogger
    {
        void LogException(Exception exception);
    }
}
