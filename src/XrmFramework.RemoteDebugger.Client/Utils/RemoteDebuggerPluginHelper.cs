using System;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public class RemoteDebuggerPluginHelper
    {
        private readonly IAssemblyExporter _exporter;
        private readonly IRemoteDebuggerAssemblyHandler _assemblyHandler;
        private readonly IRegistrationService _registrationService;
        private IFlatAssemblyContext _flatAssemblyContext;

        public RemoteDebuggerPluginHelper(IRegistrationService service,
                                           IAssemblyExporter exporter,
                                           IRemoteDebuggerAssemblyHandler assemblyHandler)
        {
            _exporter = exporter;
            _assemblyHandler = assemblyHandler;
            _registrationService = service;
        }

        public void UpdateDebugger<TPlugin>(string projectName)
        {
            Console.Write("Fetching Local Assembly...");

            var localAssembly = _assemblyHandler.CreateFromLocalAssemblyContext(typeof(TPlugin));

            localAssembly.Workflows.Clear();
            localAssembly.CustomApis.Clear();

            Console.WriteLine("Fetching Debug Assembly...");

            var debugAssembly = _assemblyHandler.CreateFromDebugAssembly(_registrationService, "XrmFramework.RemoteDebuggerPlugin", out Guid debugPluginId);

            Console.WriteLine("Computing Difference...");

            AssemblyDiffFactory.ComputeAssemblyDiff(localAssembly, debugAssembly);

            var updatedDebugAssembly = _assemblyHandler.CreateDebugAssemblyFromAssembly(localAssembly, typeof(TPlugin));

            updatedDebugAssembly.Plugins.First().Id = debugPluginId;

            _flatAssemblyContext = _assemblyHandler.CreateFlatAssemblyContextFromAssemblyContext(updatedDebugAssembly);

            Console.WriteLine("Updating Steps...");

            _exporter.DeleteAllComponents(_flatAssemblyContext.StepImages);
            _exporter.DeleteAllComponents(_flatAssemblyContext.Steps);

            _exporter.CreateAllComponents(_flatAssemblyContext.Steps);
            _exporter.CreateAllComponents(_flatAssemblyContext.StepImages);


            _exporter.UpdateAllComponents(_flatAssemblyContext.Steps);
            _exporter.UpdateAllComponents(_flatAssemblyContext.StepImages);
        }
    }
}
