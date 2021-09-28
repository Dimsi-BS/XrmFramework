using XrmFramework.Analytics;
using XrmFramework.MSBuild.Generation;

namespace XrmFramework.MSBuild.Generation
{
    public class GenerateTableFileCodeBehindTaskConfiguration
    {
        public GenerateTableFileCodeBehindTaskConfiguration(IAnalyticsTransmitter overrideAnalyticsTransmitter, ITableFileCodeBehindGenerator overrideTableFileCodeBehindGenerator)
        {
            OverrideAnalyticsTransmitter = overrideAnalyticsTransmitter;
            OverrideTableFileCodeBehindGenerator = overrideTableFileCodeBehindGenerator;
        }

        public IAnalyticsTransmitter OverrideAnalyticsTransmitter { get; }

        public ITableFileCodeBehindGenerator OverrideTableFileCodeBehindGenerator { get; }
    }
}
