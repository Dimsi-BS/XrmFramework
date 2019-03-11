using Model.Sdk;
using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AttributeMetadataAttribute : Attribute
    {
        public AttributeMetadataAttribute(AttributeTypeCode type)
        {
            Type = type;
        }

        public AttributeTypeCode Type { get; private set; }
    }
}
