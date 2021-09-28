using System.Threading.Tasks;

namespace XrmFramework.MSBuild.Generation
{
    public interface IMSBuildTaskAnalyticsTransmitter
    {
        Task TryTransmitProjectCompilingEventAsync();
    }
}
