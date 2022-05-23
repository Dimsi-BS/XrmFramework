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

        /// <summary>
        /// Computes the difference between two <see cref="IAssemblyContext"/> 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="target"></param>
        /// <returns>A copy of the <paramref name="from"/> AssemblyContext
        /// with computed <see cref="RegistrationState"/> properties according to the other <paramref name="target"/> AssemblyContext</returns>
        public IAssemblyContext ComputeDiffPatch(IAssemblyContext from, IAssemblyContext target)
        {
            //Clone the from AssemblyContext
            var fromCopy = _mapper.Map<IAssemblyContext>(from);

            if (target == null)
            {
                FlagAllFromComponent(fromCopy, RegistrationState.ToCreate);
                return fromCopy;
            }

            var fromPool = fromCopy.ComponentsOrderedPool;
            var targetPool = target.ComponentsOrderedPool;

            ComputeDiffPatchFromPool(fromPool, targetPool);

            return fromCopy;
        }

        /// <summary>
        /// Computes the difference between two Collections of <see cref="ICrmComponent"/>,
        /// fills the <paramref name="fromPool"/>'s component's <see cref="RegistrationState"/>,
        /// and adds the components that are only present in the <paramref name="targetPool"/> as <see cref="RegistrationState.ToDelete"/>
        /// </summary>
        /// <param name="fromPool"></param>
        /// <param name="targetPool"></param>
        /// <returns></returns>
        private void ComputeDiffPatchFromPool(IReadOnlyCollection<ICrmComponent> fromPool, IReadOnlyCollection<ICrmComponent> targetPool)
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
                // If already computed, don't do it again
                if (fromComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }

                // See if this component is present in the other pool
                var targetComponent = _comparer.CorrespondingComponent(fromComponent, targetPool);

                // If not, mark it and all its children as ToCreate
                if (targetComponent == null)
                {
                    FlagAllFromComponent(fromComponent, RegistrationState.ToCreate);
                    continue;
                }

                // If there, transfer the ids and see if the component is up to date
                fromComponent.Id = targetComponent.Id;
                fromComponent.ParentId = targetComponent.ParentId;
                if (fromComponent is CustomApi fromApi)
                {
                    fromApi.AssemblyId = ((CustomApi)targetComponent).AssemblyId;
                }

                fromComponent.RegistrationState = _comparer.NeedsUpdate(fromComponent, targetComponent)
                    ? RegistrationState.ToUpdate
                    : RegistrationState.Ignore;
                targetComponent.RegistrationState = RegistrationState.Computed;
            }

            // Go through every component of target that have not yet been computed
            // This mean there were no corresponding component in from, so they all have to be deleted
            foreach (var targetComponent in targetPool)
            {
                if (targetComponent.RegistrationState != RegistrationState.NotComputed)
                {
                    continue;
                }

                FlagAllFromComponent(targetComponent, RegistrationState.ToDelete);
                var componentFather = targetComponent is CustomApi api
                ? fromPool.First(c => c.Id == api.AssemblyId)
                : fromPool.First(c => c.Id == targetComponent.ParentId);
                componentFather.AddChild(targetComponent);
            }
        }

        /// <summary>
        /// Flags a <paramref name="target"/> <see cref="ICrmComponent"/> and all its children recursively
        /// with the given <see cref="RegistrationState"/> <paramref name="state"/>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="state"></param>
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