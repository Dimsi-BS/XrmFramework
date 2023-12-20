using System.Reflection;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using XrmFramework.RemoteDebugger.Client.Configuration;

namespace XrmFramework.DeployUtils.Factories
{
    public partial interface IAssemblyFactory
    {
        /// <summary>
        /// Retrieves the <c>RemoteDebugger Assembly</c> and wraps it to match the structure of any <see cref="IAssemblyContext"/>
        /// </summary>
        /// <param name="service">The Client used for communicating with the Crm</param>
        /// <param name="debugSettings">The information on the target <c>RemoteDebugger</c></param>
        /// <returns>The Debug Assembly ready to be compared to any other <see cref="IAssemblyContext"/></returns>
        IAssemblyContext CreateFromDebugAssembly(IRegistrationService service, DebugAssemblySettings debugSettings);

        /// <summary>
        /// Wraps the given <paramref name="deployAssemblyDiff"/> into an <see cref="IAssemblyContext"/>
        /// ready to be differentiated with the <c>Remote Debugger Assembly</c>
        /// </summary>
        /// <remarks>
        /// This removes the <see cref="ICrmComponent"/>s with <see cref="RegistrationState"/> <c>ToDelete</c> or <c>Ignore</c><br/>
        /// using <see cref="ICrmComponent.CleanChildrenWithState"/> because they shouldn't appear on the <c>RemoteDebugger Assembly</c>
        /// </remarks>
        /// <param name="deployAssemblyDiff">An Assembly returned by <see cref="AssemblyDiffFactory.ComputeDiffPatch"/></param>
        /// <returns>A clone of the <paramref name="deployAssemblyDiff"/> with the unnecessary <see cref="ICrmComponent"/>s removed</returns>
        IAssemblyContext WrapDiffAssemblyForDebugDiff(IAssemblyContext deployAssemblyDiff);

        /// <summary>
        /// Wraps the given <paramref name="from"/> Assembly to match the specific structure of a <c>RemoteDebugger Assembly</c> 
        /// </summary>
        /// <remarks>
        /// The <paramref name="TPlugin"/> type is used to retrieve necessary metadata
        /// which will be inserted in a <see cref="Model.Step"/>'s <see cref="StepConfiguration"/>
        /// </remarks>
        /// <param name="from">The Assembly to wrap</param>
        /// <param name="debugAssemblySettings">Information on the target <c>RemoteDebugger</c></param>
        /// <param name="TPlugin">The Type the <paramref name="from"/> Assembly was created from</param>
        /// <returns></returns>
        IAssemblyContext WrapDebugDiffForDebugStrategy(IAssemblyContext from, DebugAssemblySettings debugAssemblySettings, Assembly Assembly);
    }
}
