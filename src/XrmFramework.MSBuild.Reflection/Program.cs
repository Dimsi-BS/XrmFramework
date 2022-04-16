using System;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Generators;
using XrmFramework.Generator.Generators;

namespace XrmFramework.MSBuild.Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialisation");

            var assembly = Assembly.LoadFrom(args[0]);
            var folderPath = args[1];

            var nullableType = assembly.GetType("XrmFramework.NullableAttribute");
            var iServiceType = assembly.GetType("XrmFramework.IService");
            var defaultServiceType = assembly.GetType("XrmFramework.DefaultService");
            var iLoggedServiceType = assembly.GetType("XrmFramework.ILoggedService");

            var types = assembly.GetTypes().Where(t => iServiceType.IsAssignableFrom(t) && t.IsInterface).ToList();

            MockGenerator.GenerateMocks($"{folderPath}\\LoggedServices", types, nullableType);
            Console.WriteLine("Logged service generation completed");

            InternalDependencyProviderGenerator.Generate($"{folderPath}\\LoggedServices", types, iServiceType, defaultServiceType,
                iLoggedServiceType);

            Console.WriteLine("Dependency injection generation completed");

            DependencyInjectionGenerator.Generate(folderPath, types);

            Console.WriteLine("Dependency injection generation completed");
        }
    }
}
