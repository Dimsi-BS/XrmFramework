using System.Runtime.Serialization;
using System.Text;

namespace Model.Sdk.Metadata
{
    [DataContract]
    public class AttributeMetadata
    {
        [DataMember(Name = "n")]
        public string Name { get; set; }

        [DataMember(Name = "fn")]
        public string FieldName { get; set; }

        [DataMember(Name = "s")]
        public bool IsSelected { get; set; }

        [DataMember(Name = "dn")]
        public string DisplayName { get; set; }

        [DataMember(Name = "t")]
        public string AttributeTypeString { get; set; }

        [DataMember(Name = "db")]
        public string DateTimeBehaviourString { get; set; }

        [DataMember(Name = "r")]
        public bool Read { get; set; }

        [DataMember(Name = "w")]
        public bool Write { get; set; }

        [DataMember(Name = "c")]
        public bool Create { get; set; }

        [DataMember(Name = "af")]
        public bool AdvancedFind { get; set; }

        [DataMember(Name = "p")]
        public bool IsPrimaryIdAttribute { get; set; }

        [DataMember(Name = "pn")]
        public bool IsPrimaryNameAttribute { get; set; }

        [DataMember(Name = "pi")]
        public bool IsPrimaryImageAttribute { get; set; }

        [DataMember(Name = "e")]
        public string EnumName { get; set; }

        [DataMember(Name = "rs")]
        public IList<RelationshipMetadata> Relationships { get; set; } = new List<RelationshipMetadata>();

        #region Enum fields

        public DateTimeBehavior DateTimeBehaviour
        {
            get => (DateTimeBehavior)Enum.Parse(typeof(DateTimeBehavior), DateTimeBehaviourString);
            set => DateTimeBehaviourString = value.ToString();
        }

        public AttributeTypeCode AttributeType
        {
            get => (AttributeTypeCode)Enum.Parse(typeof(AttributeTypeCode), AttributeTypeString);
            set => AttributeTypeString = value.ToString();
        }

        #endregion

        #region Model generation

        public string ValidityString
        {
            get
            {
                var sb = new StringBuilder();
                if (Read)
                {
                    sb.Append("Read | ");
                }
                if (Write)
                {
                    sb.Append("Write | ");
                }
                if (Create)
                {
                    sb.Append("Create | ");
                }
                if (AdvancedFind)
                {
                    sb.Append("AdvancedFind | ");
                }

                if (sb.Length > 3)
                {
                    sb.Remove(sb.Length - 3, 3);
                }

                return sb.ToString();
            }
        }

        public string PrimaryFieldsString
        {
            get
            {
                if (IsPrimaryIdAttribute)
                {
                    return "Id";
                }

                if (IsPrimaryNameAttribute)
                {
                    return "Name";
                }

                if (IsPrimaryImageAttribute)
                {
                    return "Image";
                }

                return null;
            }
        }

        #endregion
    }
}
