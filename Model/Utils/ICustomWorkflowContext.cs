using System;
using Microsoft.Xrm.Sdk;
using System.Activities;

namespace Plugins
{
    public interface ICustomWorkflowContext
    {
        EntityReference ObjectRef { get; }

        Guid UserId { get; }

        Guid CorrelationId { get; }

        T GetArgumentValue<T>(InArgument<T> argument);

        void SetArgumentValue<T>(OutArgument<T> argument, T value);

        T GetArgumentValue<T>(InOutArgument<T> argument);

        void SetArgumentValue<T>(InOutArgument<T> argument, T value);

        WorkflowModes WorkflowMode { get; }

        void Log(string message, params object[] paramsObject);
    }
}
