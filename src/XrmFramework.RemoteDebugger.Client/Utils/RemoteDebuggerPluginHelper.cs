using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Service;
using XrmFramework.DeployUtils.Utils;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.RemoteDebugger.Client.Utils
{
    public class RemoteDebuggerPluginHelper
    {
        private readonly Guid _debugSessionId;
        private readonly IAssemblyExporter _exporter;
        private readonly IRemoteDebuggerAssemblyHandler _assemblyHandler;
        private readonly IRegistrationService _registrationService;
        private IFlatAssemblyContext _flatAssemblyContext;

        public RemoteDebuggerPluginHelper(IRegistrationService service,
                                           IAssemblyExporter exporter,
                                           IRemoteDebuggerAssemblyHandler assemblyHandler,
                                           IOptions<DebugSessionSettings> settings)
        {
            _exporter = exporter;
            _assemblyHandler = assemblyHandler;
            _registrationService = service;
            _debugSessionId = settings.Value.DebugSessionId;
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

            _exporter.InitExportMetadata(_flatAssemblyContext.Steps);

            Console.WriteLine("Updating Steps...");

            _exporter.DeleteAllComponents(_flatAssemblyContext.StepImages);
            _exporter.DeleteAllComponents(_flatAssemblyContext.Steps);

            _exporter.CreateAllComponents(_flatAssemblyContext.Steps);
            _exporter.CreateAllComponents(_flatAssemblyContext.StepImages);


            _exporter.UpdateAllComponents(_flatAssemblyContext.Steps);
            _exporter.UpdateAllComponents(_flatAssemblyContext.StepImages);

            RegisterStepsToDebugSession(_flatAssemblyContext.Steps);
        }

        private void RegisterStepsToDebugSession(ICollection<Step> steps)
        {
            //var debugSession = BindingModelHelper.GetById<DebugSession>(_registrationService, _debugSessionId);
            //var stepsHashes = steps
            //    .Select(s => s.Description.GetHashCode());

        }
    }
}
