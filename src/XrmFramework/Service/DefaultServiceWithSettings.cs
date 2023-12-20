using System;
using System.Collections.Generic;
using System.Linq;

namespace XrmFramework
{
    public abstract class DefaultServiceWithSettings<T> : DefaultService where T : CrmSettings, new()
    {
        private CrmSettingsFactory<T> SettingsFactory { get; }
        protected T Settings => SettingsFactory.Settings;

        /// <summary>
        /// A way to override how the <see cref="CrmSettingsFactory{TSettings}"/> will retrieve the settings values.
        /// Not overriding it will cause the <see cref="CrmSettingsFactory{TSettings}"/> to use the default implementation.
        /// </summary>
        /// <param name="settingDefinitions"></param>
        /// <returns></returns>
        protected virtual IEnumerable<(string settingName, object settingValue)> InitSettings(IEnumerable<(string settingName, Type settingType)> settingDefinitions)
            => Enumerable.Empty<(string settingName, object settingValue)>();
        
        protected DefaultServiceWithSettings(IServiceContext context) : base(context)
        {
            SettingsFactory = new CrmSettingsFactory<T>(context.AdminOrganizationService, InitSettings);
        }
    }
}