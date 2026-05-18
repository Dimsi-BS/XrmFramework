using NUnit.Framework;
using System.Threading.Tasks;
using VerifyCS = XrmFramework.Analyzers.Test.CSharpCodeFixVerifier<
    XrmFramework.Analyzers.XrmFrameworkAnalyzersAnalyzer,
    XrmFramework.Analyzers.XrmFrameworkAnalyzersCodeFixProvider>;

namespace XrmFramework.Analyzers.Test
{
    [TestFixture]
    public class XrmFrameworkAnalyzersUnitTest
    {
        //No diagnostics expected to show up
        [Test]
        public async Task TestMethod1()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Test]
        public async Task TestMethod2()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
        }
    }";

            var expected = VerifyCS.Diagnostic("XrmFrameworkAnalyzers").WithLocation(0).WithArguments("TypeName");
            await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
        }
    }
}
