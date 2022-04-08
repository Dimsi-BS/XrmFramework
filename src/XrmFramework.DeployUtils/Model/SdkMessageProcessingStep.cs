using Microsoft.Xrm.Sdk;
using XrmFramework.Definitions;

namespace Deploy
{
    partial class SdkMessageProcessingStep
    {
        public string EntityName => this.GetAttributeValue<AliasedValue>($"filter.{SdkMessageFilterDefinition.Columns.PrimaryObjectTypeCode}")?.Value as string;
    }
}
