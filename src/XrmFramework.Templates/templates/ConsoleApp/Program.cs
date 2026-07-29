using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using $safeprojectname$;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appSettings.json")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var services = new ServiceCollection();
services.AddXrmFramework(opt => opt.UseConnectionString(configuration.GetConnectionString("Xrm")));
services.AddTransient<ConsoleApplication>();

await using var serviceProvider = services.BuildServiceProvider(true);
using var scope = serviceProvider.CreateScope();

await scope.ServiceProvider.GetRequiredService<ConsoleApplication>().RunAsync();
