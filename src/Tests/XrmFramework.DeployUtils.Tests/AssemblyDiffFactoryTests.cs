using AutoMapper;
using Moq;
using XrmFramework.DeployUtils.Utils;

namespace XrmFramework.DeployUtils.Tests
{
    [TestClass]
    public class AssemblyDiffFactoryTests
    {

        private readonly AssemblyDiffFactory _diffFactory;

        private readonly Mock<ICrmComponentComparer> _mockComparer;
        private readonly Mock<IMapper> _mockMapper;


        public AssemblyDiffFactoryTests()
        {
            _mockComparer = new();
            _mockMapper = new();

            _diffFactory = new(_mockComparer.Object, _mockMapper.Object);
        }

    }
}
