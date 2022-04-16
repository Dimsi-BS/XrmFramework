using System.Threading.Tasks;
using VerifyXunit;
using XrmFramework.Analyzers.Generators;
using Xunit;

namespace XrmFramework.Analyzers.Tests;

[UsesVerify]
public class EnumGeneratorTests
{
    [Fact]
    public async Task EnumGenerator()
    {
        // The source code to test
        var source = @"
namespace XrmFramework {

    [AttributeUsage(AttributeTargets.Class)]
    public class EnumGenerationAttribute : Attribute
    {
    }

    [EnumGeneration]
    public partial class Messages
    {
        public static Messages Create = new Messages(""Create"");
        public static Messages Update = new Messages(""Update"");
    
        private Messages(string name)
        {
            MessageName = name;
        }

        public string MessageName { get; set;}
    }
}";

        // Pass the source code to our helper and snapshot test the output
        await TestHelper.Verify<EnumGenerator>(source);

    }
}