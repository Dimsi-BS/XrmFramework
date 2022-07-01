using Microsoft.Xrm.Sdk;

namespace Deploy
{
    partial class CustomApiResponseProperty
    {
        public static CustomApiResponseProperty FromXrmFrameworkArgument(string customApiName, dynamic argument, bool isOnPrem)
        {
            var response = new CustomApiResponseProperty
            {
                Description = string.IsNullOrWhiteSpace(argument.Description) ? $"{customApiName}.{argument.ArgumentName}" : argument.Description,
                Name = $"{customApiName}.{argument.ArgumentName}",
                DisplayName = string.IsNullOrWhiteSpace(argument.DisplayName) ? $"{customApiName}.{argument.ArgumentName}" : argument.DisplayName,
                Type = new OptionSetValue((int)argument.ArgumentType),
                UniqueName = argument.ArgumentName
            };

            if (isOnPrem)
            {
                response.EntityLogicalNameProperty = argument.LogicalEntityName;
            }
            else
            {
                response.LogicalEntityName = argument.LogicalEntityName;
            }

            return response;
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
