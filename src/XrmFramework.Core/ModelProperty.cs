using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ModelProperty
    {
        [JsonProperty("LogN")]
        public string LogicalName;
        public string Name;
        [JsonProperty("UsePropCh")]
        public bool useOnPropertyChanged;
        
        public string jsonName = null;
        [JsonProperty("Type")]
        public string TypeFullName;

    }
}
