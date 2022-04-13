using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Utils
{
    public class AssemblyDiffFactory
    {
        private readonly ICrmComponentComparer _comparer;
        private readonly IMapper _mapper;
        public AssemblyDiffFactory(ICrmComponentComparer comparer, IMapper mapper)
        {
            _comparer = comparer;
            _mapper = mapper;
        }

        public IDiffPatch ComputeDiffPatchFromAssemblies(IAssemblyContext from, IAssemblyContext target)
        {
            var fromCopy = _mapper.Map<AssemblyContext>(from);
            var fromPool = fromCopy.ComponentsOrderedPool;
            var targetPool = target.ComponentsOrderedPool;
            return ComputeDiffPatchFromPool(fromPool, targetPool);
        }

        public IDiffPatch ComputeDiffPatchFromPool(IReadOnlyCollection<ICrmComponent> fromPool, IReadOnlyCollection<ICrmComponent> targetPool)
        {
            /*
             * Some explanation here :
             * Collections are ordered such as a component's parent is always before it.
             * This way, once we encounter a parent whose all children can be computed fast (FlagAllFromComponent),
             * We can compute them ahead of time and they won't be computed a second time when encountered later on.
             * This is done because CorrespondingComponent is a costly function.
             */

            fromPool.ToList().Sort((x, y) => x.Rank.CompareTo(y.Rank));
            targetPool.ToList().Sort((x, y) => x.Rank.CompareTo(y.Rank));

            /*
             * Reset registration states if already computed
             */

            #region Reset
            if (fromPool.Any(c => c.RegistrationState != RegistrationState.NotComputed))
            {
                foreach (var crmComponent in fromPool)
                {
                    crmComponent.RegistrationState = RegistrationState.NotComputed;
                }
            }

            if (targetPool.Any(c => c.RegistrationState != RegistrationState.NotComputed))
            {
                foreach (var crmComponent in targetPool)
                {
                    crmComponent.RegistrationState = RegistrationState.NotComputed;
                }
            }
            #endregion

            var diffComponents = new List<DiffComponent>();
            foreach (var fromComponent in fromPool)
            {
                if (fromComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }
                var targetComponent = _comparer.CorrespondingComponent(fromComponent, targetPool);
                if (targetComponent == null)
                {
                    FlagAllFromComponent(fromComponent, diffComponents, RegistrationState.ToCreate);
                    continue;
                }

                var diffComponent = new DiffComponent()
                {
                    Component = fromComponent,
                    OriginalId = fromComponent.Id,
                    OriginalParentId = fromComponent.ParentId,
                    DiffResult = _comparer.NeedsUpdate(fromComponent, targetComponent)
                        ? RegistrationState.ToUpdate
                        : RegistrationState.Ignore
                };
                diffComponents.Add(diffComponent);

                fromComponent.Id = targetComponent.Id;
                fromComponent.ParentId = targetComponent.ParentId;

                fromComponent.RegistrationState = RegistrationState.Computed;
                targetComponent.RegistrationState = RegistrationState.Computed;
            }

            foreach (var targetComponent in targetPool)
            {
                if (targetComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }

                FlagAllFromComponent(targetComponent, diffComponents, RegistrationState.ToDelete);
            }

            return new DiffPatch(diffComponents);
        }


        private static void FlagAllFromComponent(ICrmComponent target, ICollection<DiffComponent> terminalStack, RegistrationState state)
        {
            terminalStack.Add(new DiffComponent(target, state));
            target.RegistrationState = RegistrationState.Computed;
            foreach (var child in target.Children)
            {
                FlagAllFromComponent(child, terminalStack, state);
            }
        }
    }
}