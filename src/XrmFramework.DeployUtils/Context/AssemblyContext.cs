using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    class AssemblyContext : IAssemblyContext
    {
        public AssemblyInfo AssemblyInfo { get; set; } = new();


        public ICollection<Plugin> Plugins { get; } = new List<Plugin>();
        public ICollection<CustomApi> CustomApis { get; } = new List<CustomApi>();
        public ICollection<Plugin> Workflows { get; } = new List<Plugin>();

        public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
        {
            get
            {
                List<ICrmComponent> pool = new() { AssemblyInfo };
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

                pool.Sort((x, y) => x.Rank.CompareTo(y.Rank));
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
        public Guid Id
        {
            get => AssemblyInfo.Id;
            set
            {
                foreach (var child in Children)
                {
                    child.ParentId = value;
                }

                AssemblyInfo.Id = value;
            }
        }

        public IEnumerable<ICrmComponent> Children
        {
            get
            {
                var children = new List<ICrmComponent>();
                children.AddRange(Plugins);
                children.AddRange(Workflows);
                children.AddRange(CustomApis);
                return children;
            }
        }

        public void AddChild(ICrmComponent child)
        {
            switch (child)
            {
                case Plugin plugin:
                    if (plugin.IsWorkflow) Workflows.Add(plugin);
                    else Plugins.Add(plugin);
                    break;
                case CustomApi api:
                    api.AssemblyId = Id;
                    CustomApis.Add(api);
                    break;
                default:
                    throw new ArgumentException("AssemblyContext doesn't take this type of children");
            }
        }

        #region AssemblyInfo ProxyProperties

        public string UniqueName => AssemblyInfo.UniqueName;
        public int Rank => AssemblyInfo.Rank;
        public bool DoAddToSolution => AssemblyInfo.DoAddToSolution;
        public bool DoFetchTypeCode => AssemblyInfo.DoFetchTypeCode;

        public RegistrationState RegistrationState
        {
            get => AssemblyInfo.RegistrationState;
            set => AssemblyInfo.RegistrationState = value;
        }

        public Guid ParentId { get; set; }
        public string EntityTypeName => AssemblyInfo.EntityTypeName;
        #endregion

    }
}
