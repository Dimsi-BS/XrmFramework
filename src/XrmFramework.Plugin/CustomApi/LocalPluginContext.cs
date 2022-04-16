using Newtonsoft.Json;
using Microsoft.Xrm.Sdk;

// ReSharper disable once CheckNamespace
namespace XrmFramework
{
    partial class LocalPluginContext : ICustomApiContext
    {
        public EntityReference ObjectRef => new(PluginExecutionContext.PrimaryEntityName, PluginExecutionContext.PrimaryEntityId);
               
        public T GetArgumentValue<T>(CustomApiInArgument<T> argument)
        {
            if (!PluginExecutionContext.InputParameters.TryGetValue(argument.ArgumentName, out var argumentValue) || argumentValue == null)
            {
                return default;
            }
            
            switch (argument.ArgumentType)
            {

                case CustomApiArgumentType.String:
                    if (argument.IsSerializedArgument)
                    {
                        return JsonConvert.DeserializeObject<T>((string)argumentValue);
                    }
                    else
                    {
                        return (T)argumentValue;
                    }
                default:
                    return (T)argumentValue;
            }
        }

        public void SetArgumentValue<T>(CustomApiOutArgument<T> argument, T value)
        {
            switch (argument.ArgumentType)
            {
                case CustomApiArgumentType.String:
                    if (argument.IsSerializedArgument)
                    {
                        PluginExecutionContext.OutputParameters[argument.ArgumentName] = JsonConvert.SerializeObject(value);
                    }
                    else
                    {
                        PluginExecutionContext.OutputParameters[argument.ArgumentName] = value;
                    }
                    break;
                default:
                    PluginExecutionContext.OutputParameters[argument.ArgumentName] = value;
                    break;
            }
        }

        public bool HasArgument<T>(CustomApiInArgument<T> argument)
            => PluginExecutionContext.InputParameters.ContainsKey(argument.ArgumentName);

        public bool HasArgument<T>(CustomApiOutArgument<T> argument)
            => PluginExecutionContext.OutputParameters.ContainsKey(argument.ArgumentName);
    }
}
