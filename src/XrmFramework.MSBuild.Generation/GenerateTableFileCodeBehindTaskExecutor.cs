using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoDi;
using Microsoft.Build.Framework;
using XrmFramework.CommonModels;

namespace XrmFramework.MSBuild.Generation
{
    public class GenerateTableFileCodeBehindTaskExecutor : IGenerateTableFileCodeBehindTaskExecutor
    {
        private readonly IProcessInfoDumper _processInfoDumper;
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;
        private readonly IXrmFrameworkProjectProvider _xrmFrameworkProjectProvider;
        private readonly XrmFrameworkProjectInfo _xrmFrameworkProjectInfo;
        private readonly WrappedGeneratorContainerBuilder _wrappedGeneratorContainerBuilder;
        private readonly IObjectContainer _rootObjectContainer;
        private readonly IMSBuildTaskAnalyticsTransmitter _msbuildTaskAnalyticsTransmitter;
        private readonly IExceptionTaskLogger _exceptionTaskLogger;

        public GenerateTableFileCodeBehindTaskExecutor(
            IProcessInfoDumper processInfoDumper,
            ITaskLoggingWrapper taskLoggingWrapper,
            IXrmFrameworkProjectProvider xrmFrameworkProjectProvider,
            XrmFrameworkProjectInfo xrmFrameworkProjectInfo,
            WrappedGeneratorContainerBuilder wrappedGeneratorContainerBuilder,
            IObjectContainer rootObjectContainer,
            IMSBuildTaskAnalyticsTransmitter msbuildTaskAnalyticsTransmitter,
            IExceptionTaskLogger exceptionTaskLogger)
        {
            _processInfoDumper = processInfoDumper;
            _taskLoggingWrapper = taskLoggingWrapper;
            _xrmFrameworkProjectProvider = xrmFrameworkProjectProvider;
            _xrmFrameworkProjectInfo = xrmFrameworkProjectInfo;
            _wrappedGeneratorContainerBuilder = wrappedGeneratorContainerBuilder;
            _rootObjectContainer = rootObjectContainer;
            _msbuildTaskAnalyticsTransmitter = msbuildTaskAnalyticsTransmitter;
            _exceptionTaskLogger = exceptionTaskLogger;
        }

        public IResult<IReadOnlyCollection<ITaskItem>> Execute()
        {
            _processInfoDumper.DumpProcessInfo();
            _taskLoggingWrapper.LogMessage("Starting GenerateFeatureFileCodeBehind");

            var xrmFrameworkProject = _xrmFrameworkProjectProvider.GetXrmFrameworkProject();

            using (var generatorContainer = _wrappedGeneratorContainerBuilder.BuildGeneratorContainer(
                xrmFrameworkProject.ProjectSettings.ConfigurationHolder,
                xrmFrameworkProject.ProjectSettings,
                _xrmFrameworkProjectInfo.GeneratorPlugins,
                _rootObjectContainer))
            {
                var projectCodeBehindGenerator = generatorContainer.Resolve<IProjectCodeBehindGenerator>();

                try
                {
                    _ = Task.Run(_msbuildTaskAnalyticsTransmitter.TryTransmitProjectCompilingEventAsync);

                    var returnValue = projectCodeBehindGenerator.GenerateCodeBehindFilesForProject();

                    if (_taskLoggingWrapper.HasLoggedErrors())
                    {
                        return Result<IReadOnlyCollection<ITaskItem>>.Failure("Feature file code-behind generation has failed with errors.");
                    }

                    return Result.Success(returnValue);
                }
                catch (Exception e)
                {
                    _exceptionTaskLogger.LogException(e);
                    return Result<IReadOnlyCollection<ITaskItem>>.Failure(e);
                }
            }
        }
    }
}