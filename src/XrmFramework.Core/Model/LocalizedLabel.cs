using Newtonsoft.Json;

namespace XrmFramework.DefinitionManager.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class LocalizedLabel
    {
        public string Label { get; set; }

        public int LangId { get; set; }
    }
}