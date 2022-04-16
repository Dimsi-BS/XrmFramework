using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using XrmFramework;

namespace $safeprojectname$.Commands
{
    public record EveryTenMinutesTimerCommand : ICommand;

    public class EveryTenMinutesTimerCommandHandler : ICommandHandler<EveryTenMinutesTimerCommand>
    {
        private readonly IService _service;

        public EveryTenMinutesTimerCommandHandler(IService service)
        {
            _service = service;
        }


        /// <inheritdoc />
        public Task ExecuteAsync(EveryTenMinutesTimerCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}
