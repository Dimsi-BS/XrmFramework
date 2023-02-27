using System;
using System.Collections.Generic;

namespace XrmFramework
{
    public abstract class DefaultServiceWithSettings<T> : DefaultService where T : CrmSettings, new()
    {
        protected CrmSettingsFactory<T> SettingsFactory { get; }
        protected T Settings => SettingsFactory.Settings;

        /// <summary>
        /// A way to override how the <see cref="CrmSettingsFactory{TSettings}"/> will retrieve the settings values.
        /// Leaving it not implemented will cause the <see cref="CrmSettingsFactory{TSettings}"/> to use the default implementation.
        /// </summary>
        /// <param name="settingDefinitions"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected virtual IEnumerable<(string settingName, object settingValue)> InitSettings(IEnumerable<(string settingName, Type settingType)> settingDefinitions)
            => throw new NotImplementedException();
        
        protected DefaultServiceWithSettings(IServiceContext context) : base(context)
        {
            SettingsFactory = new CrmSettingsFactory<T>(context.AdminOrganizationService, InitSettings);
        }
    }
}