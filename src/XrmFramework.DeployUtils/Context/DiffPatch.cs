using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    class DiffPatch : IDiffPatch
    {
        private readonly IEnumerable<DiffComponent> _diffResults;

        public DiffPatch(IEnumerable<DiffComponent> diffList)
        {
            _diffResults = diffList;
            var targetPluginAssemblyDiff = _diffResults.FirstOrDefault(d => d.Component is AssemblyInfo);
            if (targetPluginAssemblyDiff != null)
            {
                PluginAssembly = (AssemblyInfo)targetPluginAssemblyDiff.Component;
                PluginAssembly.RegistrationState = targetPluginAssemblyDiff.DiffResult;
                targetPluginAssemblyDiff.DiffResult = RegistrationState.Computed;
                foreach (var diffComponent in _diffResults)
                {
                    switch (diffComponent.Component)
                    {
                        case Plugin plugin:
                            plugin.ParentId = PluginAssembly.Id;
                            break;
                        case CustomApi customApi:
                            customApi.AssemblyId = PluginAssembly.Id;
                            break;
                    }
                }
            }
        }

        public AssemblyInfo PluginAssembly { get; }

        public IEnumerable<ICrmComponent> GetComponentsWhere(Func<DiffComponent, bool> predicate)
        {
            return _diffResults.Where(predicate)
                .Select(d => d.Component);
        }

        public void SetComputedWhere(Func<DiffComponent, bool> predicate)
        {
            foreach (var diffComponent in _diffResults.Where(predicate))
            {
                diffComponent.DiffResult = RegistrationState.Computed;
            }
        }

    }
}
