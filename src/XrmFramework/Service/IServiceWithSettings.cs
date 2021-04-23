using System;

namespace XrmFramework
{
    public interface IServiceWithSettings
    {
        void InitSettings();
    }

    public class CrmSettings
    {

    }

    public class SettingNameAttribute : Attribute
    {
        public string Name { get; }

        public SettingNameAttribute(string name)
        {
            Name = name;
        }
    }
}
