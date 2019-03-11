using System;

namespace Model
{
    /// <summary>
    /// Indicates that a class will be ignored by DefinitionManager
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class DefinitionManagerIgnoreAttribute : Attribute
    {
    }
}
