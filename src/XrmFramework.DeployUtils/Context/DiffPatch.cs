using Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class DiffPatch : IDiffPatch
    {
        private readonly IEnumerable<DiffComponent> _diffResults;

        public DiffPatch(IEnumerable<DiffComponent> diffList)
        {
            _diffResults = diffList;
            var targetPluginAssemblyDiff = _diffResults.FirstOrDefault(d => d.Component is PluginAssembly);
            if (targetPluginAssemblyDiff != null)
            {
                PluginAssembly = (PluginAssembly)targetPluginAssemblyDiff.Component;
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

        public PluginAssembly PluginAssembly { get; }

        public IEnumerable<ICrmComponent> RetrieveWhere(Func<DiffComponent, bool> predicate)
        {
            return _diffResults.Where(predicate)
                .Select(d => d.Component);
        }
    }
}
