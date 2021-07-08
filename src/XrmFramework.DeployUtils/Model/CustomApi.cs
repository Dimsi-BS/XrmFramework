using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xrm.Sdk;

namespace Deploy
{
    partial class CustomApi
    {
        public List<CustomApiRequestParameter> InArguments { get; } = new List<CustomApiRequestParameter>();

        public List<CustomApiResponseProperty> OutArguments { get; } = new List<CustomApiResponseProperty>();


        public static CustomApi FromXrmFrameworkCustomApi(dynamic record, string prefix)
        {
            var type = (Type)record.GetType();

            dynamic customApiAttribute = type.GetCustomAttributes().FirstOrDefault(a => a.GetType().FullName == "XrmFramework.CustomApiAttribute");

            if (customApiAttribute == null)
            {
                throw new Exception($"The custom api type {type.FullName} must have a CustomApiAttribute defined");
            }

            var customApi = new CustomApi
            {
                DisplayName = customApiAttribute.DisplayName,
                Name = type.Name,
                AllowedCustomProcessingStepType = new OptionSetValue((int)customApiAttribute.AllowedCustomProcessing),
                BindingType = new OptionSetValue((int)customApiAttribute.BindingType),
                BoundEntityLogicalName = customApiAttribute.BoundEntityLogicalName,
                Description = customApiAttribute.Description,
                ExecutePrivilegeName = customApiAttribute.ExecutePrivilegeName,
                IsFunction = customApiAttribute.IsFunction,
                IsPrivate = customApiAttribute.IsPrivate,
                UniqueName = $"{prefix}_{type.Name}",
                WorkflowSdkStepEnabled = customApiAttribute.WorkflowSdkStepEnabled,
                FullName = type.FullName
            };

            foreach (var argument in record.Arguments)
            {
                if (argument.IsInArgument)
                {
                    customApi.InArguments.Add(CustomApiRequestParameter.FromXrmFrameworkArgument(customApi.Name, argument));
                }
                else
                {
                    customApi.OutArguments.Add(CustomApiResponseProperty.FromXrmFrameworkArgument(customApi.Name, argument));
                }
            }

            return customApi;
        }

        public string FullName
        {
            get;
            set;
        }

        public string Prefix
        {
            set => UniqueName = $"{value}{Name}";
        }
    }
}
