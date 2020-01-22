using System;
using System.Collections.Generic;
using System.Text;
using Plugins;

namespace XrmFramework.Utils
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
