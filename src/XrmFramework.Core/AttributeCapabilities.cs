using System;

namespace XrmFramework.Core
{
    [Flags]
    public enum AttributeCapabilities
    {
        None = 0,
        Read = 1,
        Update = 2,
        Create = 4,
        AdvancedFind = 8
    }
}