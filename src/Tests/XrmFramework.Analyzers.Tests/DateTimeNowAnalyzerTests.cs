using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace XrmFramework.Analyzers.Tests
{

    //[UsesVerify]
    public class DateTimeNowAnalyzerTests
    {
        [Fact]
        public async Task IsDateTimeNowCalled()
        {
            var sourceCode = @"
                using System;

                public class TestClass
                {
                    public void TestMethod() {
                        var date = DateTime.Now;
                        var date2 = DateTime.UtcNow;
                    }
                }";

            var analyzerTest = new CSharpAnalyzerTest<DateTimeNowRuleAnalyzer, DefaultVerifier>();

            analyzerTest.TestCode = sourceCode;

            analyzerTest.ExpectedDiagnostics.Add(new DiagnosticResult("XRM0300", Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).WithLocation(7, 36));
            analyzerTest.ExpectedDiagnostics.Add(new DiagnosticResult("XRM0300", Microsoft.CodeAnalysis.DiagnosticSeverity.Warning).WithLocation(8, 37));

            await analyzerTest.RunAsync();
        }
    }
}
