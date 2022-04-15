using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    class AssemblyContext : IAssemblyContext
    {
        public PluginAssembly Assembly { get; set; } = new();

        public ICollection<Plugin> Plugins { get; } = new List<Plugin>();
        public ICollection<CustomApi> CustomApis { get; } = new List<CustomApi>();
        public ICollection<Plugin> Workflows { get; } = new List<Plugin>();

        public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
        {
            get
            {
                List<ICrmComponent> pool = new();
                pool.Add(Assembly);
                foreach (var plugin in Plugins)
                {
                    CreateSolutionComponentPoolRecursive(pool, plugin);
                }

                foreach (var customApi in CustomApis)
                {
                    CreateSolutionComponentPoolRecursive(pool, customApi);

                }

                foreach (var workflow in Workflows)
                {
                    CreateSolutionComponentPoolRecursive(pool, workflow);
                }

                return pool;
            }
        }

        private static void CreateSolutionComponentPoolRecursive(ICollection<ICrmComponent> terminalStack,
            ICrmComponent component)
        {
            terminalStack.Add(component);
            foreach (var child in component.Children)
            {
                CreateSolutionComponentPoolRecursive(terminalStack, child);
            }
        }
    }
}
