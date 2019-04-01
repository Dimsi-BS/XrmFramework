using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelPropertyConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public ModelPropertyConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
