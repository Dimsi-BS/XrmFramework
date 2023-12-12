using Microsoft.Xrm.Sdk;
using XrmFramework;

namespace Deploy
{
    partial class SdkMessageProcessingStep
    {
        /// <summary>EntityName linked to the SdkStep</summary>
        public string EntityName => this.GetAttributeValue<AliasedValue>($"filter.{SdkMessageFilterDefinition.PrimaryObjectTypeCode}")?.Value as string;
    }
}
