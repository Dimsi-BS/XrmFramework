using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Model.Sdk;

namespace Model.Utils
{
    public class AttributeDefinition
    {

        internal static AttributeDefinition GetDefinition(EntityDefinition parentEntity, FieldInfo field)
        {
            var fieldName = field.GetValue(null) as string;
            if (string.IsNullOrEmpty(fieldName)) return null;

            var attributeType = field.GetCustomAttribute<AttributeMetadataAttribute>()?.Type ?? AttributeTypeCode.String;

            var attribute = new AttributeDefinition
            {
                LogicalName = fieldName,
                ParentEntity = parentEntity,
                AttributeType = attributeType,
                PrimaryAttributeType = field.GetCustomAttribute<PrimaryAttributeAttribute>()?.Type,
                StringLength = field.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength,

            };


            return attribute;
        }

        public int? StringLength { get; private set; }

        public PrimaryAttributeType? PrimaryAttributeType { get; private set; }

        public string LogicalName { get; private set; }

        public AttributeTypeCode AttributeType { get; private set; }

        public EntityDefinition ParentEntity { get; private set; }
    }
}
