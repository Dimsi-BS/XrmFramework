using System.Linq;
using System.Reflection;
using Plugins;
using XrmFramework.Utils;

namespace XrmFramework
{
    public abstract class DefaultServiceWithSettings<T> : DefaultService, IServiceWithSettings where T : CrmSettings, new()
    {
        protected T Settings { get; private set; }

        public void InitSettings()
        {
            var propertySettings = typeof(T).GetProperties()
                .Where(p => p.GetCustomAttribute<SettingNameAttribute>() != null)
                .Select(p => new {Name = p.GetCustomAttribute<SettingNameAttribute>().Name, Property = p}).ToList();

            var returnSettings = new T();

            foreach (var propertySetting in propertySettings)
            {
                var settingValue = GetEnvironmentVariable(propertySetting.Property.PropertyType, propertySetting.Name);

                propertySetting.Property.SetValue(returnSettings, settingValue);
            }

            Settings = returnSettings;
        }

        protected DefaultServiceWithSettings(IServiceContext context) : base(context)
        {
        }
    }
}