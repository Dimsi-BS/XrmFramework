using System;

namespace Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelPropertyConverterAttribute : Attribute
    {
        public Type ConverterType { get; }
        public object[] ConstructorParameters { get; }

        public ModelPropertyConverterAttribute(Type converterType, params object[] constructorParameters)
        {
            ConverterType = converterType;
            ConstructorParameters = constructorParameters;
        }
    }
}
