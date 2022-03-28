
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XrmFramework;
using XrmFramework.Utils;

namespace XrmFramework
{
    public static class ModelManager
    {
        
        public static void TestFunction()
        {
            Console.WriteLine("Connecting to CRM");

        }
        private static void GenerateNewBindingModel(Entity entity)
        {

            Console.WriteLine("Testay");
            ModelDefinition lol = ModelFactory.CreateFromType(null);

        }

        public static void SerializeModelDefinition(Type modelToSerialize)
        {
            var modelDefinition = ModelFactory.CreateFromType(modelToSerialize);
            Console.WriteLine(modelToSerialize.Name);
            
            JsonSerializer.TrySerialize<ModelDefinition>(modelDefinition, out string sortie, out string errorMessage);
            Console.Write(sortie);
        }

    }
}
