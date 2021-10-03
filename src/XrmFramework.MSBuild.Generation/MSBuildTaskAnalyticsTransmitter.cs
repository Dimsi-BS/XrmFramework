using System;
using System.Threading.Tasks;
using XrmFramework.Analytics;
using XrmFramework.CommonModels;

namespace XrmFramework.MSBuild.Generation
{
    public class MSBuildTaskAnalyticsTransmitter : IMSBuildTaskAnalyticsTransmitter
    {
        private readonly IAnalyticsEventProvider _analyticsEventProvider;
        private readonly IMSBuildInformationProvider _msBuildInformationProvider;
        private readonly XrmFrameworkProjectInfo _xrmFrameworkProjectInfo;
        private readonly IAnalyticsTransmitter _analyticsTransmitter;
        private readonly ITaskLoggingWrapper _taskLoggingWrapper;

        public MSBuildTaskAnalyticsTransmitter(
            IAnalyticsEventProvider analyticsEventProvider,
            IMSBuildInformationProvider msBuildInformationProvider,
            XrmFrameworkProjectInfo xrmFrameworkProjectInfo,
            IAnalyticsTransmitter analyticsTransmitter,
            ITaskLoggingWrapper taskLoggingWrapper)
        {
            _analyticsEventProvider = analyticsEventProvider;
            _msBuildInformationProvider = msBuildInformationProvider;
            _xrmFrameworkProjectInfo = xrmFrameworkProjectInfo;
            _analyticsTransmitter = analyticsTransmitter;
            _taskLoggingWrapper = taskLoggingWrapper;
        }

        public async Task TryTransmitProjectCompilingEventAsync()
        {
            try
            {
                var transmissionResult = await TransmitProjectCompilingEventAsync();

                if (transmissionResult is IFailure failure)
                {
                    _taskLoggingWrapper.LogMessageWithLowImportance($"Could not transmit analytics: {failure}");
                }
            }
            catch (Exception exc)
            {
                // catch all exceptions since we do not want to break the build simply because event creation failed
                // but still return an error containing the exception to at least log it
                _taskLoggingWrapper.LogMessageWithLowImportance($"Could not transmit analytics: {exc}");
            }
        }

        public async Task<IResult> TransmitProjectCompilingEventAsync()
        {
            var projectCompilingEvent = _analyticsEventProvider.CreateProjectCompilingEvent(
                _msBuildInformationProvider.GetMSBuildVersion(),
                _xrmFrameworkProjectInfo.ProjectAssemblyName,
                _xrmFrameworkProjectInfo.TargetFrameworks,
                _xrmFrameworkProjectInfo.CurrentTargetFramework,
                _xrmFrameworkProjectInfo.ProjectGuid);
            return await _analyticsTransmitter.TransmitXrmFrameworkProjectCompilingEvent(projectCompilingEvent);
        }
    }
}
