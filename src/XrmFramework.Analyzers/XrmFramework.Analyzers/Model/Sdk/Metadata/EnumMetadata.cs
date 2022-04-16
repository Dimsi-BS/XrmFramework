using System.Runtime.Serialization;

namespace Model.Sdk.Metadata
{
    [DataContract]
    public class EnumMetadata
    {
        public string UniqueName => string.IsNullOrEmpty(FieldName) ? Name : $"{EntityName}|{FieldName}";

        [DataMember(Name = "n")]
        public string Name { get; set; }

        [DataMember(Name = "en")]
        public string EnumName { get; set; }

        [DataMember(Name = "s")]
        public bool IsSelected { get; set; }

        [DataMember(Name = "e")]
        public string EntityName { get; set; }

        [DataMember(Name = "f")]
        public string FieldName { get; set; }

        public bool IsGlobal => string.IsNullOrEmpty(FieldName);

        [DataMember(Name = "o")]
        public IList<OptionMetadata> Options { get; set; } = new List<OptionMetadata>();
    }

    [DataContract]
    public class OptionMetadata
    {
        [DataMember(Name = "n")]
        public string Name { get; set; }

        [DataMember(Name = "v")]
        public int Value { get; set; }

        [DataMember(Name = "d")]
        public string DisplayName { get; set; }
    }
}
