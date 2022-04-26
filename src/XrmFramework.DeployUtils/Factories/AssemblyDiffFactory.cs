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

        internal IAssemblyContext ComputeDiffPatch(IAssemblyContext from, IAssemblyContext target)
        {
            var fromCopy = _mapper.Map<IAssemblyContext>(from);

            var fromPool = fromCopy.ComponentsOrderedPool;
            var targetPool = target.ComponentsOrderedPool;

            var deleteComponents = ComputeDiffPatchFromPool(fromPool, targetPool);

            foreach (var deleteComponent in deleteComponents)
            {
                fromCopy.AddChild(deleteComponent);
            }

            return fromCopy;
        }

        private ICollection<ICrmComponent> ComputeDiffPatchFromPool(IReadOnlyCollection<ICrmComponent> fromPool, IReadOnlyCollection<ICrmComponent> targetPool)
        {
            /*
             * Some explanation here :
             * Collections are ordered such as a component's parent is always before it.
             * This way, once we encounter a parent whose all children can be computed fast (FlagAllFromComponent),
             * We can compute them ahead of time and they won't be computed a second time when encountered later on.
             * This is done because CorrespondingComponent is a costly function.
             */


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

            foreach (var fromComponent in fromPool)
            {
                if (fromComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }
                var targetComponent = _comparer.CorrespondingComponent(fromComponent, targetPool);
                if (targetComponent == null)
                {
                    FlagAllFromComponent(fromComponent, RegistrationState.ToCreate);
                    continue;
                }

                fromComponent.Id = targetComponent.Id;
                fromComponent.ParentId = targetComponent.ParentId;

                fromComponent.RegistrationState = _comparer.NeedsUpdate(fromComponent, targetComponent)
                    ? RegistrationState.ToUpdate
                    : RegistrationState.Ignore;
                targetComponent.RegistrationState = RegistrationState.Computed;
            }

            var deleteComponents = new List<ICrmComponent>();

            foreach (var targetComponent in targetPool)
            {
                if (targetComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }

                FlagAllFromComponent(targetComponent, RegistrationState.ToDelete);
                if (targetComponent.Rank == 1)
                {
                    deleteComponents.Add(targetComponent);
                }
                else
                {
                    var componentFather = fromPool.First(c => c.Id == targetComponent.ParentId);
                    componentFather.AddChild(targetComponent);
                }
            }

            return deleteComponents;
        }


        private static void FlagAllFromComponent(ICrmComponent target, RegistrationState state)
        {
            target.RegistrationState = state;
            foreach (var child in target.Children)
            {
                FlagAllFromComponent(child, state);
            }
        }
    }
}