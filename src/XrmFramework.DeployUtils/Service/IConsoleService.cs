using Spectre.Console;
using XrmFramework.DeployUtils.Model.Record;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public interface IConsoleService : IDisposable
{
    Task StartAsync();

    Task StartMainScreenAsync();

    void Stop();

    Task SetStatus(string status, Color? color = null);

    void AddCorrelation(IRecordedCorrelation recordedCorrelation);

    void Refresh();

    Task IsRunning();
}
