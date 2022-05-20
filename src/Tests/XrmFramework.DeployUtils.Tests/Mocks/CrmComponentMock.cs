using Moq;
using XrmFramework.DeployUtils.Model;

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
