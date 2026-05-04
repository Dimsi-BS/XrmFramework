using NUnit.Framework;
using System.Threading.Tasks;


using XrmFramework.Analyzers.Generators;

namespace XrmFramework.Analyzers.Tests;


[TestFixture]
public class EnumGeneratorTests
{
    [Test]
    public async Task EnumGenerator()
    {
        // Source contains a smart-enum-style class flagged with [EnumGeneration].
        // The generator must emit a partial class with a static Items collection
        // listing every public static field of the same type as the class.
        var source = @"
using System;

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
        [Obsolete]
        public static Messages SetState = new Messages(""SetState"");

        private Messages(string name)
        {
            MessageName = name;
        }

        public string MessageName { get; set; }
    }
}";

        await TestHelper.Verify<EnumGenerator>(source);
    }
}
