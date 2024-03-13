using MediatR;
using Spectre.Console;
using XrmFramework.RemoteDebugger.Client.Recorder;

namespace XrmFramework.RemoteDebugger.Client.Notifications
{
    internal class StartMainScreenNotification : INotification
    {
    }

    internal class SlowStartMainScreenNotificationHandler : INotificationHandler<StartMainScreenNotification>
    {
        private readonly IConsoleService _consoleService;

        public SlowStartMainScreenNotificationHandler(IConsoleService consoleService)
        {
            _consoleService = consoleService;
        }

        public async Task Handle(StartMainScreenNotification notification, CancellationToken cancellationToken)
        {
            await _consoleService.StartMainScreenAsync();
        }
    }
}
