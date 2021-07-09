using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xrm.Sdk;


namespace XrmFramework
{


    public abstract class CustomApi : Plugin
    {
        public readonly List<CustomApiArgument> Arguments = new List<CustomApiArgument>();

        protected CustomApi(string methodName) : base(null, null)
        {
            SetCustomApiInfos(methodName);

            foreach (var property in GetType().GetProperties())
            {
                var inArgumentAttribute = property.GetCustomAttribute<CustomApiInputAttribute>();
                var outArgumentAttribute = property.GetCustomAttribute<CustomApiOutputAttribute>();

                var argumentAttribute = (CustomApiArgumentAttribute)inArgumentAttribute ?? outArgumentAttribute;

                if (argumentAttribute == null)
                {
                    continue;
                }

                var argumentName = argumentAttribute.Name ?? property.Name;

                var objectType = property.PropertyType.GenericTypeArguments.Single();

                CustomApiArgumentType argumentType;
                var isSerialized = false;

                if (objectType == typeof(bool))
                {
                    argumentType = CustomApiArgumentType.Boolean;
                }
                else if (objectType == typeof(DateTime))
                {
                    argumentType = CustomApiArgumentType.DateTime;
                }
                else if (objectType == typeof(decimal))
                {
                    argumentType = CustomApiArgumentType.Decimal;
                }
                else if (objectType == typeof(Entity))
                {
                    argumentType = CustomApiArgumentType.Entity;
                }
                else if (objectType == typeof(EntityCollection))
                {
                    argumentType = CustomApiArgumentType.EntityCollection;
                }
                else if (objectType == typeof(EntityReference))
                {
                    argumentType = CustomApiArgumentType.EntityReference;
                }
                else if (objectType == typeof(float))
                {
                    argumentType = CustomApiArgumentType.Float;
                }
                else if (objectType == typeof(int))
                {
                    argumentType = CustomApiArgumentType.Integer;
                }
                else if (objectType == typeof(Money))
                {
                    argumentType = CustomApiArgumentType.Money;
                }
                else if (objectType == typeof(OptionSetValue) || objectType.IsEnum)
                {
                    argumentType = CustomApiArgumentType.Picklist;
                }
                else if (objectType == typeof(string))
                {
                    argumentType = CustomApiArgumentType.String;
                }
                else if (objectType == typeof(string[]))
                {
                    argumentType = CustomApiArgumentType.StringArray;
                }
                else if (objectType == typeof(Guid))
                {
                    argumentType = CustomApiArgumentType.Guid;
                }
                else
                {
                    argumentType = CustomApiArgumentType.String;
                    isSerialized = true;
                }

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

                Arguments.Add(propertyValue);

                property.SetValue(this, propertyValue);
            }

        }

        protected override void AddSteps()
        {
        }
    }

}
