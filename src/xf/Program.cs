using System;
using System.Linq;
using System.Threading.Tasks;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace XrmFramework.Tools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var manager = new AnalyzerManager("../../../../TestPlugins.sln");
            var workspace = manager.GetWorkspace();

            var solution = workspace.CurrentSolution;

            var project = solution.Projects.FirstOrDefault(p => p.Name == "TestPlugins") ?? throw new Exception("Project not found");

            var compilation = await project.GetCompilationAsync() ??
                throw new Exception("Compilation failed.");


            var iService = compilation.GetTypeByMetadataName("XrmFramework.IService");

            var nullableType = compilation.GetTypeByMetadataName("XrmFramework.NullableAttribute");

            var implementations = await SymbolFinder.FindImplementationsAsync(iService, solution);
            
            var generator = new LoggedServiceCodeGenerator(nullableType);

            foreach (var namedTypeSymbol in implementations.Where(i => !i.IsAbstract).SelectMany(i => ((INamedTypeSymbol)i).Interfaces.Where(j => j.AllInterfaces.Any(k => k.Equals(iService)))))
            {
                var fileContent = generator.Generate(namedTypeSymbol);

                Console.WriteLine(fileContent);

                //var members = namedTypeSymbol.GetMembers();
                //Console.WriteLine($"Interface : {namedTypeSymbol}");

                //foreach (var member in members)
                //{
                //    switch (member)
                //    {
                //        case IMethodSymbol method:
                //            Console.WriteLine($"\tMethod : {method.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}");

                //            foreach (var parameter in method.Parameters)
                //            {
                //                Console.WriteLine($"Parameter : {parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}");
                //            }

                //            break;
                //        case IPropertySymbol property:
                //            Console.WriteLine($"\tProperty : {property}");
                //            break;
                //    }
                //}
            }
        }
    }
}
