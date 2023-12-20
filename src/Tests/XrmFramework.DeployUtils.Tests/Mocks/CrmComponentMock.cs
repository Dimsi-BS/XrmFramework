using Moq;
using XrmFramework.DeployUtils.Model.Interfaces;

namespace XrmFramework.DeployUtils.Tests.Mocks
{
    public class CrmComponentMock
    {
        public Mock<ICrmComponent> Mock = new();

        public CrmComponentMock()
        {
            Mock.SetupProperty(x => x.RegistrationState, RegistrationState.NotComputed);
        }
    }
}
