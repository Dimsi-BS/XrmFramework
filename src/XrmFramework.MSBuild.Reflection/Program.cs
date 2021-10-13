using System;
using System.Linq;
using System.Reflection;
using XrmFramework.DeployUtils.Generators;

namespace XrmFramework.MSBuild.Reflection
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFrom(args[0]);
            var folderPath = args[1];

            var generationType = args[2];

            var nullableType = assembly.GetType("XrmFramework.NullableAttribute");
            var iServiceType = assembly.GetType("XrmFramework.IService");
            var defaultServiceType = assembly.GetType("XrmFramework.DefaultService");
            var iLoggedServiceType = assembly.GetType("XrmFramework.ILoggedService");

            var types = assembly.GetTypes().Where(t => iServiceType.IsAssignableFrom(t) && t.IsInterface).ToList();

            if (generationType == "LoggedServices")
            {
                MockGenerator.GenerateMocks(folderPath, types, nullableType);

                InternalDependencyProviderGenerator.Generate(folderPath, types, iServiceType, defaultServiceType,
                    iLoggedServiceType);

                Console.WriteLine("Logged service generation completed");
            }
            else if (generationType == "DependencyInjection")
            {
                DependencyInjectionGenerator.Generate(folderPath, types, iServiceType, defaultServiceType);

                Console.WriteLine("Dependency injection generation completed");
            }
        }
    }
}
