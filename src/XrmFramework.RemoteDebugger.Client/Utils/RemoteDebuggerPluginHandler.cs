using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework.DeployUtils;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public class RemoteDebuggerPluginHandler : RegistrationHelper
    {
        protected new IRemoteDebuggerAssemblyHandler _assemblyFactory;

        public RemoteDebuggerPluginHandler(IRegistrationService service,
                                           IAssemblyExporter exporter,
                                           IRemoteDebuggerAssemblyHandler assemblyHandler) : base(service, exporter, assemblyHandler)
        {
            _assemblyFactory = assemblyHandler;
        }


        public static void UpdateRemoteDebuggerPlugin<TPlugin>(string projectName)
        {
            Console.WriteLine(projectName);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRegistrationService, RegistrationService>();
            serviceCollection.AddScoped<ISolutionContext, SolutionContext>();
            serviceCollection.AddScoped<IAssemblyExporter, AssemblyExporter>();
            serviceCollection.AddScoped<IAssemblyImporter, AssemblyImporter>();
            serviceCollection.AddSingleton<IAssemblyFactory, AssemblyFactory>();
            serviceCollection.AddSingleton<IRemoteDebuggerAssemblyHandler, RemoteDebuggerAssemblyHandler>();
            serviceCollection.AddSingleton<RemoteDebuggerPluginHandler>();


            ParseSolutionSettings(projectName, out string pluginSolutionUniqueName, out string connectionString);

            serviceCollection.Configure<SolutionSettings>((settings) =>
            {
                settings.ConnectionString = connectionString;
                settings.PluginSolutionUniqueName = pluginSolutionUniqueName;
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            Console.WriteLine($"You are about to modify the debug session");
            Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
            var r = Console.ReadLine();
            while (r != "y" && r != "n")
            {
                Console.WriteLine($"Do you want to register new steps to debug ? (y/n)");
                r = Console.ReadLine();
            }
            if (r == "n")
            {
                return;
            }

            var remoteDebuggerPluginHandler = serviceProvider.GetRequiredService<RemoteDebuggerPluginHandler>();

            remoteDebuggerPluginHandler.UpdateDebugger<TPlugin>(projectName);

        }


        public void UpdateDebugger<TPlugin>(string projectName)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(typeof(TPlugin));

            Console.WriteLine("Fetching Debug Assembly...");


            var debugAssembly = _assemblyFactory.CreateFromDebugAssembly(_registrationService, "XrmFramework.RemoteDebuggerPlugin", out Guid debugPluginId);


            Console.Write("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, debugAssembly);

            var updatedDebugAssembly = _assemblyFactory.CreateDebugAssemblyFromAssembly(localAssembly, typeof(TPlugin), debugPluginId);

            _flatAssemblyContext = _assemblyFactory.CreateFlatAssemblyContextFromAssemblyContext(updatedDebugAssembly);


            DeleteAllComponents(_flatAssemblyContext.StepImages);
            DeleteAllComponents(_flatAssemblyContext.Steps);

            CreateAllComponents(_flatAssemblyContext.Steps);
            CreateAllComponents(_flatAssemblyContext.StepImages);


            UpdateAllComponents(_flatAssemblyContext.Steps);
            UpdateAllComponents(_flatAssemblyContext.StepImages);
        }
    }
}
