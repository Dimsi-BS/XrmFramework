using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Spectre.Console;
using XrmFramework.BindingModel;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Model.Record;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger.Client.Recorder;
using XrmFramework.RemoteDebugger.Client.Services;

namespace XrmFramework.RemoteDebugger.Client.Configuration
{
    /// <summary>
    /// Configures the necessary services and parameters of the project
    /// </summary>
    public class DebuggerServiceCollectionFactory
    {
        /// <summary>
        /// Configures the required objects used during RemoteDebug, such as :
        /// <list type="bullet">
        ///     <item><see cref="IRegistrationService"/>, the service used for communicating with the CRM</item>
        ///     <item><see cref="AutoMapper.IMapper"/>, used for conversion between <see cref="Deploy"/> and <see cref="Model"/> objects
        ///         as well as cloning</item>
        ///     <item><see cref="DeploySettings"/>, an object that contains information on the target <c>Solution</c></item>
        ///     <item>The configuration of all other implemented interfaces used by <c>Dependency Injection</c></item>
        /// </list>
        /// </summary>
        /// <returns><see cref="IServiceProvider"/> the service provider used to instantiate every object needed</returns>
        public IServiceCollection CreateServiceCollection<T>() where T: class, IRemoteDebuggerMessageManager
        {
            if (ConfigurationManager.ConnectionStrings["DebugConnectionString"] == null)
            {
                throw new Exception("The connectionString \"DebugConnectionString\" is not defined.");
            }

            var serviceCollection = new ServiceCollection()
                .InitServiceCollection()
                .AddScoped<IAssemblyExporter, DebuggerAssemblyExporter>()
                .AddScoped<IRecordedSession, RecordedSession>()
                .AddSingleton<IConsoleService, ConsoleService>()
                .AddScoped<IRemoteDebuggerMessageManager, T>()
                .AddSingleton<IDeploySettingsProvider, RemoteDebuggerDeploySettingsProvider>()
                .AddSingleton<IDebugSessionProvider, DebugSessionProvider>();

            serviceCollection.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(GetType().Assembly, typeof(RegistrationHelper).Assembly);
            });

            serviceCollection.AddScoped<IOrganizationService, CrmServiceClient>(sp =>
            {
                var deploySettingsProvider = sp.GetRequiredService<IDeploySettingsProvider>();

                var deploySettings = deploySettingsProvider.GetSelectedDeploySettings();

                var service = new CrmServiceClient(deploySettings.ConnectionString);
                return service;
            });

            return serviceCollection;
        }
    }
}
