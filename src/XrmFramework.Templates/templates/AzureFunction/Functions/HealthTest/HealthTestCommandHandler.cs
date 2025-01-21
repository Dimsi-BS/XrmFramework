using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace $safeprojectname$.Functions.HealthTest;

public class HealthTestCommandHandler(
    IOrganizationServiceAsync2 service) 
    : IRequestHandler<HealthTestCommand>
{
    /// <inheritdoc />
    public async Task Handle(HealthTestCommand command, CancellationToken cancellationToken)
    {
        await service.ExecuteAsync(new WhoAmIRequest());
    }
}
