using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

#if DEBUG
builder.Configuration.AddJsonFile("local.settings.json", false);
#endif

builder.UseMediatR(
    options => options
        .OpenApiInfos(infos =>
        {
            infos.Title("Sample project")
                .Description("Sample project description")
                .Version(ThisAssembly.AssemblyVersion);
        })
);

builder.Services.AddXrmFramework(opt =>
    opt
        .UseConnectionString(builder.Configuration.GetConnectionStringOrSetting("Xrm"))
        .UseWebApi(false));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

await builder.Build().RunAsync();
