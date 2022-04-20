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
                List<ICrmComponent> pool = new() { AssemblyInfo };
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
                        _workflows.RemoveAll(w => w.Id == plugin.Id);
                    }
                    else _plugins.RemoveAll(p => p.Id == plugin.Id);
                    break;
                case CustomApi api:
                    _customApis.RemoveAll(c => c.Id == api.Id);
                    break;
                default:
                    throw new ArgumentException("AssemblyContext doesn't take this type of children");
            }
        }

        public void CleanChildrenWithState(RegistrationState state)
        {
            foreach (var child in Children)
            {
                child.CleanChildrenWithState(state);
                if (!child.Children.Any() && child.RegistrationState == state)
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
