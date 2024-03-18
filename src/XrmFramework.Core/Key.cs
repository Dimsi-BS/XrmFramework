using System.Collections.Generic;
using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Key
    {
        public string LogicalName { get; set; }

        public string Name { get; set; }

        public List<string> FieldNames { get; } = new();
    }
}