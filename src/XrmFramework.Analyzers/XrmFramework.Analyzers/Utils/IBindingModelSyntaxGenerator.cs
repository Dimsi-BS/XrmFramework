using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XrmFramework.Analyzers.Utils
{
    public interface IBindingModelSyntaxGenerator
    {
        CompilationUnitSyntax AddUsings(CompilationUnitSyntax compilationSyntax);
        SubEntitySyntaxGenerator GetSyntaxGenerator();
    }
}