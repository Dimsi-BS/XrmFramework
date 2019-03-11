using Microsoft.Xrm.Sdk;
using System;
namespace Plugins
{
    public interface IContext
    {
        EntityReference BusinessUnitRef { get; }

        T GetInputParameter<T>(InputParameters parameterName);
        void SetInputParameter<T>(InputParameters parameterName, T parameterValue);

        T GetOutputParameter<T>(OutputParameters parameterName);
        void SetOutputParameter<T>(OutputParameters parameterName, T parameterValue);

        bool HasPostImage(string imageName);
        Entity GetPostImage(string imageName);

        bool HasPreImage(string imageName);
        Entity GetPreImage(string imageName);

        bool HasSharedVariable(string variableName);
        T GetSharedVariable<T>(string variableName);
        void SetSharedVariable<T>(string variableName, T value);

        bool IsSynchronous();
        bool IsAsynchronous();
        
        bool IsCreate();
        bool IsUpdate();
        bool IsMessage(Messages message);

        void Log(string message, params object[] formatArgs);

        void LogFields(Entity entity, params string[] fieldNames);

        void ThrowInvalidPluginException(string messageName, params object[] formatArguments);
    }
}
