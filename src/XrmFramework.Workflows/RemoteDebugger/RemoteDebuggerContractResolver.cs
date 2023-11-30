using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Converters;

namespace XrmFramework.RemoteDebugger;

partial class RemoteDebuggerContractResolver
{
    partial void AddArgumentConverter(ICollection<JsonConverter> converters)
    {
        if (converters.Any(c => c is ArgumentsCollectionConverter))
        {
            return;
        }
        
        converters.Add(new ArgumentsCollectionConverter());
    }
}