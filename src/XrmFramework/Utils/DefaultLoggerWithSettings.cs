using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace XrmFramework
{
	public abstract class DefaultLoggerWithSettings<TSettings> : DefaultLogHelper where TSettings : CrmSettings, new()
	{
		protected CrmSettingsFactory<TSettings> SettingsFactory { get; }
		protected TSettings Settings => SettingsFactory.Settings;

		/// <summary>
		/// A way to override how the <see cref="CrmSettingsFactory{TSettings}"/> will retrieve the settings values.
		/// Not overriding it will cause the <see cref="CrmSettingsFactory{TSettings}"/> to use the default implementation.
		/// </summary>
		/// <param name="settingDefinitions"></param>
		/// <returns></returns>

		protected virtual IEnumerable<(string settingName, object settingValue)> InitSettings(IEnumerable<(string settingName, Type settingType)> settingDefinitions)
			=> Enumerable.Empty<(string settingName, object settingValue)>();

		protected DefaultLoggerWithSettings(IOrganizationService service, ILoggerContext context, LogMethod logMethod) : base(service, context, logMethod)
		{
			SettingsFactory = new CrmSettingsFactory<TSettings>(service, InitSettings);
		}
	}
}
