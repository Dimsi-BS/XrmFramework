using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
    public partial interface ICustomApiContext
    {
        EntityReference ObjectRef { get; }

        Guid UserId { get; }

        Guid CorrelationId { get; }

        T GetArgumentValue<T>(CustomApiInArgument<T> argument);

        void SetArgumentValue<T>(CustomApiOutArgument<T> argument, T value);

        bool HasArgument<T>(CustomApiInArgument<T> argument);
        bool HasArgument<T>(CustomApiOutArgument<T> argument);

        void Log(string message, params object[] paramsObject);
    }
}