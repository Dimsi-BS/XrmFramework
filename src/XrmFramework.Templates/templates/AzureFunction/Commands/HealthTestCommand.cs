using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using System.Threading.Tasks;

namespace $safeprojectname$.Commands
{
    public record HealthTestCommand : ICommand;

    public class HealthTestCommandHandler : ICommandHandler<HealthTestCommand>
    {
        private readonly IOrganizationServiceAsync2 _service;

        public HealthTestCommandHandler(IOrganizationServiceAsync2 service)
        {
            _service = service;
        }


        /// <inheritdoc />
        public async Task ExecuteAsync(HealthTestCommand command)
        {
            await _service.ExecuteAsync(new WhoAmIRequest());
        }
    }
}
