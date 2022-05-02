using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    partial class AssemblyContext : IAssemblyContext
    {
        public AssemblyInfo AssemblyInfo { get; set; } = new();

        private readonly List<Plugin> _plugins = new();
        private readonly List<CustomApi> _customApis = new();
        private readonly List<Plugin> _workflows = new();

        public ICollection<Plugin> Plugins => _plugins;
        public ICollection<CustomApi> CustomApis => _customApis;
        public ICollection<Plugin> Workflows => _workflows;

        public IReadOnlyCollection<ICrmComponent> ComponentsOrderedPool
        {
            get
            {
                List<ICrmComponent> pool = new() { this };
                foreach (var plugin in _plugins)
                {
                    CreateSolutionComponentPoolRecursive(pool, plugin);
                }

                foreach (var customApi in _customApis)
                {
                    CreateSolutionComponentPoolRecursive(pool, customApi);

                }

                foreach (var workflow in _workflows)
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
                foreach (var child in Plugins)
                {
                    child.ParentId = value;
                }

                foreach (var child in Workflows)
                {
                    child.ParentId = value;
                }
                foreach (var child in CustomApis)
                {
                    child.AssemblyId = value;
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
                    if (plugin.IsWorkflow) _workflows.Add(plugin);
                    else _plugins.Add(plugin);
                    break;
                case CustomApi api:
                    api.AssemblyId = Id;
                    _customApis.Add(api);
                    break;
                default:
                    throw new ArgumentException("AssemblyContext doesn't take this type of children");
            }
        }

        private void RemoveChild(ICrmComponent child)
        {
            switch (child)
            {
                case Plugin plugin:
                    if (plugin.IsWorkflow)
                    {
                        _workflows.Remove(plugin);
                    }
                    else _plugins.Remove(plugin);
                    break;
                case CustomApi api:
                    _customApis.Remove(api);
                    break;
                default:
                    throw new ArgumentException("AssemblyContext doesn't have this type of children");
            }
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            var childrenWithStateSafe = Children
                .Where(c => c.RegistrationState == state)
                .ToList();
            foreach (var child in childrenWithStateSafe)

            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any())
                {
                    RemoveChild(child);
                }
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
