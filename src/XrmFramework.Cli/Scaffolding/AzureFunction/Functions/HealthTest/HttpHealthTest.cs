using System.Net;
using Azure.Functions.Worker.Extensions.HttpApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace $safeprojectname$.Functions.HealthTest;

public class HttpHealthTest(
    IMediator mediator,
    IHttpContextAccessor httpContextAccessor)
    : HttpFunctionBase(httpContextAccessor)
{
    [Function(nameof(HttpHealthTest))]
    [OpenApiOperation(nameof(HttpHealthTest))]

    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/HealthTest")]
        HttpRequest httpRequest,
        [FromBody] HealthTestCommand request)
    {
        await mediator.Send(request);

        return Ok();
    }
}
