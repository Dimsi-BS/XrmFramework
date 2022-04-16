using System;

namespace XrmFramework
{
    public class SettingNameAttribute : Attribute
    {
        public string Name { get; }

        public SettingNameAttribute(string name)
        {
            Name = name;
        }
    }
}