using BoDi;
using Microsoft.Build.Utilities;
using XrmFramework.Analytics;
using XrmFramework.Analytics.AppInsights;
using XrmFramework.Analytics.UserId;
using XrmFramework.Configuration;
using XrmFramework.EnvironmentAccess;
using XrmFramework.Generator.Configuration;
using XrmFramework.Generator.Project;

namespace XrmFramework.MSBuild.Generation
{
    public class GenerateTableFileCodeBehindTaskContainerBuilder
    {
        public IObjectContainer BuildRootContainer(
            TaskLoggingHelper taskLoggingHelper,
            XrmFrameworkProjectInfo xrmFrameworkProjectInfo,
            IMSBuildInformationProvider msbuildInformationProvider,
            GenerateTableFileCodeBehindTaskConfiguration generateFeatureFileCodeBehindTaskConfiguration)
        {
            var objectContainer = new ObjectContainer();

            // singletons
            objectContainer.RegisterInstanceAs(taskLoggingHelper);
            objectContainer.RegisterInstanceAs(xrmFrameworkProjectInfo);
            objectContainer.RegisterInstanceAs(msbuildInformationProvider);
            objectContainer.RegisterInstanceAs(generateFeatureFileCodeBehindTaskConfiguration);

            // types
            objectContainer.RegisterTypeAs<TaskLoggingHelperWithNameTagWrapper, ITaskLoggingWrapper>();
            objectContainer.RegisterTypeAs<XrmFrameworkProjectProvider, IXrmFrameworkProjectProvider>();
            objectContainer.RegisterTypeAs<MSBuildProjectReader, IMSBuildProjectReader>();
            objectContainer.RegisterTypeAs<ProcessInfoDumper, IProcessInfoDumper>();
            objectContainer.RegisterTypeAs<AssemblyResolveLoggerFactory, IAssemblyResolveLoggerFactory>();
            objectContainer.RegisterTypeAs<GenerateTableFileCodeBehindTaskExecutor, IGenerateTableFileCodeBehindTaskExecutor>();
            objectContainer.RegisterTypeAs<MSBuildTaskAnalyticsTransmitter, IMSBuildTaskAnalyticsTransmitter>();
            objectContainer.RegisterTypeAs<ExceptionTaskLogger, IExceptionTaskLogger>();

            objectContainer.RegisterTypeAs<FileUserIdStore, IUserUniqueIdStore>();
            objectContainer.RegisterTypeAs<FileService, IFileService>();
            objectContainer.RegisterTypeAs<DirectoryService, IDirectoryService>();
            objectContainer.RegisterTypeAs<EnvironmentWrapper, IEnvironmentWrapper>();

            objectContainer.RegisterTypeAs<EnvironmentXrmFrameworkTelemetryChecker, IEnvironmentXrmFrameworkTelemetryChecker>();
            objectContainer.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            objectContainer.RegisterTypeAs<HttpClientAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();
            objectContainer.RegisterTypeAs<AppInsightsEventSerializer, IAppInsightsEventSerializer>();
            objectContainer.RegisterTypeAs<HttpClientWrapper, HttpClientWrapper>();
            objectContainer.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();
            objectContainer.RegisterTypeAs<ConfigurationLoader, IConfigurationLoader>();
            objectContainer.RegisterTypeAs<GeneratorConfigurationProvider, IGeneratorConfigurationProvider>();
            objectContainer.RegisterTypeAs<ProjectReader, IXrmFrameworkProjectReader>();
            objectContainer.RegisterTypeAs<XrmFrameworkJsonLocator, IXrmFrameworkJsonLocator>();

            if (generateFeatureFileCodeBehindTaskConfiguration.OverrideAnalyticsTransmitter is null)
            {
                objectContainer.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            }
            else
            {
                objectContainer.RegisterInstanceAs(generateFeatureFileCodeBehindTaskConfiguration.OverrideAnalyticsTransmitter);
            }

            return objectContainer;
        }
    }
}
