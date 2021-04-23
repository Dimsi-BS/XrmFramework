using TechTalk.SpecFlow.Analytics;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateServiceFileCodeBehindTaskConfiguration
    {
        public GenerateServiceFileCodeBehindTaskConfiguration(IAnalyticsTransmitter overrideAnalyticsTransmitter, IServiceFileCodeBehindGenerator overrideFeatureFileCodeBehindGenerator)
        {
            OverrideAnalyticsTransmitter = overrideAnalyticsTransmitter;
            OverrideFeatureFileCodeBehindGenerator = overrideFeatureFileCodeBehindGenerator;
        }

        public IAnalyticsTransmitter OverrideAnalyticsTransmitter { get; }

        public IServiceFileCodeBehindGenerator OverrideFeatureFileCodeBehindGenerator { get; }
    }
}
