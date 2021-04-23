using System;

namespace XrmFramework
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CrmModelImplementationAttribute : Attribute
    {
        public CrmModelImplementationAttribute(Type implementationType)
        {
            ImplementationType = implementationType;
        }

        public Type ImplementationType { get; }
    }
}
