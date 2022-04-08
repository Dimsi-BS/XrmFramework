using System;
using XrmFramework.DeployUtils.Model;

namespace XrmFramework.DeployUtils.Context
{
    public class DiffComponent
    {
        public Guid CorrespondingId { get; set; }
        public RegistrationState DiffResult { get; set; }
        public ISolutionComponent SolutionComponent { get; set; }
    }
}
