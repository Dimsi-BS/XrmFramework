using System;
using System.IO;
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
            var folderPath = Directory.GetParent(args[0]).Parent.Parent.Parent.Parent.Parent.FullName + "\\obj";

            var nullableType = assembly.GetType("XrmFramework.NullableAttribute");
            var iServiceType = assembly.GetType("XrmFramework.IService");

            var generator = new LoggedServiceCodeGenerator(nullableType);

            var types = assembly.GetTypes().Where(t => iServiceType.IsAssignableFrom(t) && t.IsInterface);
            
            MockGenerator.GenerateMocks(folderPath, types, nullableType);

            Console.WriteLine("Logged service generation completed");
        }
    }
}
