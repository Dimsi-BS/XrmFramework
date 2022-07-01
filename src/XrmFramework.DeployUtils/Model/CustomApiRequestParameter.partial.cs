using Microsoft.Xrm.Sdk;

namespace Deploy
{
    partial class CustomApiRequestParameter
    {
        public static CustomApiRequestParameter FromXrmFrameworkArgument(string customApiName, dynamic argument, bool isOnPrem)
        {
            var parameter = new CustomApiRequestParameter
            {
                Description = string.IsNullOrWhiteSpace(argument.Description) ? $"{customApiName}.{argument.ArgumentName}" : argument.Description,
                Name = $"{customApiName}.{argument.ArgumentName}",
                DisplayName = string.IsNullOrWhiteSpace(argument.DisplayName) ? $"{customApiName}.{argument.ArgumentName}" : argument.DisplayName,
                IsOptional = argument.IsOptional,
                Type = new OptionSetValue((int)argument.ArgumentType),
                UniqueName = argument.ArgumentName
            };

            if (isOnPrem)
            {
                parameter.EntityLogicalNameProperty = argument.LogicalEntityName;
            }
            else
            {
                parameter.LogicalEntityName = argument.LogicalEntityName;
            }

            return parameter;
        }

        [Microsoft.Xrm.Sdk.AttributeLogicalNameAttribute("entitylogicalname")]
        public string EntityLogicalNameProperty
        {
            get
            {
                return this.GetAttributeValue<string>("entitylogicalname");
            }
            set
            {
                this.OnPropertyChanging("EntityLogicalNameProperty");
                this.SetAttributeValue("entitylogicalname", value);
                this.OnPropertyChanged("EntityLogicalNameProperty");
            }
        }
    }
}
