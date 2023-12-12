using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using XrmFramework.BindingModel;
using XrmFramework.Model;

namespace XrmFramework;

/// <summary>
/// The basic class used to populate a <typeparamref name="TSettings"/> object from the CRM.
/// </summary>
/// <typeparam name="TSettings"></typeparam>
public class CrmSettingsFactory<TSettings> where TSettings : CrmSettings, new()
{
	protected IOrganizationService Service { get; }
	private readonly Lazy<TSettings> _settings;

	public TSettings Settings => _settings.Value;

	/// <summary>
	/// Constructor for the <see cref="CrmSettingsFactory{TSettings}"/> class.
	/// </summary>
	/// <param name="service"></param>
	/// <param name="initSettings">If this delegate throws an <see cref="NotImplementedException"/>, uses the default <see cref="CrmSettingsFactory{TSettings}.DefaultInitSettings"/></param>
	public CrmSettingsFactory(IOrganizationService service, InitSettings initSettings)
	{
		Service = service;
		_settings = new Lazy<TSettings>(() =>
		{
			var propertySettings = typeof(TSettings).GetProperties()
			                                        .Where(p => p.GetCustomAttribute<SettingNameAttribute>() != null)
			                                        .Select(p => new {Name = p.GetCustomAttribute<SettingNameAttribute>().Name, Property = p})
			                                        .ToList();

			var returnSettings = new TSettings();

			
			var splitPropertySettings = propertySettings.Select(p => (p.Name, p.Property.PropertyType)).ToList();
			
			
			var	settingsValues = initSettings(splitPropertySettings);
			
			if(!settingsValues.Any())
			{
				settingsValues = DefaultInitSettings(splitPropertySettings);
			}

			foreach (var value in settingsValues)
			{
				var property = propertySettings.FirstOrDefault(p => p.Name == value.settingName);

				property?.Property.SetValue(returnSettings, value.settingValue);
			}

			return returnSettings;
		});
	}

	public delegate IEnumerable<(string settingName, object settingValue)> InitSettings(IEnumerable<(string settingName, Type settingType)> settingDefinitions);
	
	public IEnumerable<(string settingName, object settingValue)> DefaultInitSettings(IEnumerable<(string settingName, Type settingType)> settingDefinitions)
	{
		var settings = settingDefinitions.ToList();

		var results = GetEnvironmentVariables(settings.Select(s => s.settingName).ToArray());

		return settings.Join(results, p => p.settingName, p => p.SchemaName, (property, parameter) =>
		{
			var settingValue = GetEnvironmentVariableValue(property.settingType, parameter);

			(string settingName, object settingValue) p = new(parameter.SchemaName, settingValue);
			return p;
		}).ToList();
	}
	
	public ICollection<EnvironmentVariable> GetEnvironmentVariables(params string[] schemaNames)
	{
		if (schemaNames == null || schemaNames.Length == 0)
		{
			return new List<EnvironmentVariable>();
		}

		var queryVariable = BindingModelHelper.GetRetrieveAllQuery<EnvironmentVariable>();

		queryVariable.Criteria.FilterOperator = LogicalOperator.Or;

		foreach (var schemaName in schemaNames) {
			queryVariable.Criteria.AddCondition(EnvironmentVariableDefinition.Columns.SchemaName, ConditionOperator.Equal, schemaName);
		}

		var linkValue = queryVariable.AddLink(EnvironmentVariableValueDefinition.EntityName, EnvironmentVariableDefinition.Columns.Id, EnvironmentVariableValueDefinition.Columns.EnvironmentVariableDefinitionId, JoinOperator.LeftOuter);
		linkValue.EntityAlias = EnvironmentVariableValueDefinition.EntityName;
		linkValue.Columns.AddColumn(EnvironmentVariableValueDefinition.Columns.Value);

		return Service.RetrieveAll<EnvironmentVariable>(queryVariable);
	}
	
	public object GetEnvironmentVariable(Type objectType, string schemaName)
	{
		var variable = GetEnvironmentVariables(schemaName).FirstOrDefault();

		return GetEnvironmentVariableValue(objectType, variable);
	}

	public object GetEnvironmentVariableValue(Type objectType, EnvironmentVariable variable)
            {
                if (variable == null)
                {
                    return null;
                }
    
                switch (variable.Type)
                {
                    case EnvironmentVariableType.String:
                        if (objectType != typeof(string))
                        {
                            throw new ArgumentException($"The environment variable is of type String GetEnvironmentVariable must be called with a string Type argument");
                        }
    
                        return variable.Value;
                    case EnvironmentVariableType.Number:
                        if (objectType != typeof(int))
                        {
                            throw new ArgumentException($"The environment variable is of type Integer GetEnvironmentVariable must be called with a int Type argument");
                        }
    
                        return int.Parse(variable.Value);
                    case EnvironmentVariableType.Boolean:
                        if (objectType != typeof(bool))
                        {
                            throw new ArgumentException($"The environment variable is of type Boolean GetEnvironmentVariable must be called with a bool Type argument");
                        }
    
                        return bool.Parse(variable.Value);
                    case EnvironmentVariableType.JSON:
                        return JsonConvert.DeserializeObject(variable.Value, objectType);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(variable), "The Type of the environment variable is not supported");
                }
            }
}
