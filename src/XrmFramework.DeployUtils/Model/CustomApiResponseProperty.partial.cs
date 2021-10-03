using Microsoft.Xrm.Sdk;

namespace Deploy
{
    partial class CustomApiResponseProperty
    {
        public static CustomApiResponseProperty FromXrmFrameworkArgument(string customApiName, dynamic argument)
        {
            return new CustomApiResponseProperty
            {
                Description = string.IsNullOrWhiteSpace(argument.Description) ? $"{customApiName}.{argument.ArgumentName}" : argument.Description,
                Name = $"{customApiName}.{argument.ArgumentName}",
                DisplayName = string.IsNullOrWhiteSpace(argument.DisplayName) ? $"{customApiName}.{argument.ArgumentName}" : argument.DisplayName,
                LogicalEntityName = argument.LogicalEntityName,
                Type = new OptionSetValue((int)argument.ArgumentType),
                UniqueName = argument.ArgumentName
            };
        }
    }
}
