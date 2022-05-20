using Deploy;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    /// <summary>
    /// Metadata of the target solution
    /// </summary>
    public interface ISolutionContext
    {
        /// <summary>Name of the solution</summary>
        string SolutionName { get; }

        /// <summary>Target solution as entity on the Crm</summary>
        Solution Solution { get; }

        /// <summary>Publisher</summary>
        Publisher Publisher { get; }

        /// <summary>List of Components of the solution</summary>
        List<SolutionComponent> Components { get; }

        /// <summary>Filters used in the solution</summary>
        List<SdkMessageFilter> Filters { get; }

        /// <summary>Messages</summary>
        Dictionary<Messages, EntityReference> Messages { get; }

        /// <summary>Users</summary>
        List<KeyValuePair<string, Guid>> Users { get; }

        /// <summary>Retrieves all metadata of the solution</summary>
        /// <remarks>This method is not used anymore, but is left because it would be a pain to write it again if it's ever needed</remarks>
        void InitMetadata();

        /// <summary>
        /// Retrieves only the necessary metadata for the given set of <see cref="Step"/>
        /// </summary>
        /// <param name="steps"></param>
        void InitExportMetadata(IEnumerable<Step> steps);

        /// <summary>
        /// Inits the components essential for importing an <see cref="IAssemblyContext"/>
        /// The solutionName is already configured in <see cref="DeployUtils"/>
        /// </summary>
        /// <param name="solutionName"></param>
        void InitSolutionContext(string solutionName = null);


    }
}
