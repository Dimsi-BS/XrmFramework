using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace $safeprojectname$
{
    class Program
{
    private static IServiceProvider _serviceProvider;

    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        RegisterServices(configuration);
        var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ConsoleApplication>().RunAsync();
        DisposeServices();
    }

    private static void RegisterServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddXrmFramework(opt => opt.UseConnectionString(configuration.GetConnectionString("Xrm")));

        services.AddTransient<ConsoleApplication>();
        _serviceProvider = services.BuildServiceProvider(true);
    }

    private static void DisposeServices()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
}