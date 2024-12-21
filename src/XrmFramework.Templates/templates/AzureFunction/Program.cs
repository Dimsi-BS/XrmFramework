using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.AddXrmFramework();
builder.Services.AddXrmFramework(opt =>
    opt
        .UseConnectionString(builder.Configuration.GetConnectionStringOrSetting("Xrm"))
        .UseWebApi(false));

await builder.Build().RunAsync();
