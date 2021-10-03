using Newtonsoft.Json;

namespace XrmFramework.Core
{
    [JsonObject(MemberSerialization.OptOut)]
    public class LocalizedLabel
    {
        public string Label { get; set; }

        public int LangId { get; set; }
    }
}