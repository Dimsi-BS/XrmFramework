using Microsoft.Xrm.Sdk;

namespace Deploy
{
    partial class CustomApiRequestParameter : ICustomApiComponent
    {
        public static CustomApiRequestParameter FromXrmFrameworkArgument(string customApiName, dynamic argument)
        {
            return new CustomApiRequestParameter
            {
                Description = string.IsNullOrWhiteSpace(argument.Description) ? $"{customApiName}.{argument.ArgumentName}" : argument.Description,
                UniqueName = $"{customApiName}.{argument.ArgumentName}",
                DisplayName = string.IsNullOrWhiteSpace(argument.DisplayName) ? $"{customApiName}.{argument.ArgumentName}" : argument.DisplayName,
                LogicalEntityName = argument.LogicalEntityName,
                IsOptional = argument.IsOptional,
                Type = new OptionSetValue((int)argument.ArgumentType),
                Name = argument.ArgumentName
            };
        }

        public Entity ToEntity()
        {
            return this;
        }
    }
}
