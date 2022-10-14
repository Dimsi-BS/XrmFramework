using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XrmFramework
{
    public abstract class CustomApi : Plugin
    {
        public ICollection<CustomApiArgument> Arguments { get; } = new List<CustomApiArgument>();

        private string _customApiMethodName;
        private string _customApiEntityName;

        protected CustomApi(string methodName) : base(null, null)
        {
            //Sets methodInfo and _isCustomApi
            SetCustomApiInfos(methodName);

            //Loop through every property
            foreach (var property in GetType().GetProperties())
            {
                var inArgumentAttribute = property.GetCustomAttribute<CustomApiInputAttribute>();
                var outArgumentAttribute = property.GetCustomAttribute<CustomApiOutputAttribute>();
                //The property has to have an input or output attribute (c'est quoi un attribute)
                var argumentAttribute = (CustomApiArgumentAttribute)inArgumentAttribute ?? outArgumentAttribute;

                // If it is neither then skip to next property
                if (argumentAttribute == null)
                {
                    continue;
                }


                // Get the name if the attribute
                var argumentName = argumentAttribute.Name ?? property.Name;
                // Get property generic type ?
                var objectType = property.PropertyType.GenericTypeArguments.Single();

                var isSerialized = !CustomApiArgumentTypeMapper.TryMap(objectType, out var argumentType);

                var propertyValue = (CustomApiArgument)Activator.CreateInstance(property.PropertyType,
                    new object[] {
                        argumentName,
                        argumentType,
                        isSerialized,
                        argumentAttribute.Description,
                        argumentAttribute.DisplayName,
                        argumentAttribute.LogicalEntityName,
                        argumentAttribute.IsOptional
                        });
                // Add the newly created argument to the list
                Arguments.Add(propertyValue);
                // Set the property valye to the newly created argument ??? Pourquoi ?
                property.SetValue(this, propertyValue);
            }

        }

        protected void SetCustomApiInfos(string methodName)
        {
            _customApiMethodName = methodName;

            var customApiAttribute = GetType().GetCustomAttribute<CustomApiAttribute>();

            _customApiEntityName = string.IsNullOrWhiteSpace(customApiAttribute.BoundEntityLogicalName) ? string.Empty : customApiAttribute.BoundEntityLogicalName;
        }

        internal override IEnumerable<Step> InitStepsToExecute(LocalPluginContext localContext)
        {
            return new List<Step>
            {
                new (this, localContext.MessageName, Stages.PostOperation, Modes.Synchronous, _customApiEntityName, _customApiMethodName)
            };
        }

        protected override void AddSteps()
        {
        }
    }

}
