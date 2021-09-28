// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;

namespace XrmFramework.DeployUtils.Generators
{
    public class MockGenerator
    {
        public static void GenerateMocks(string loggedServiceFolder, IEnumerable<Type> types, Type nullableAttributeType)
        {
            foreach (var type in types)
            {
                GenerateLogServiceFile(loggedServiceFolder, type, nullableAttributeType);
            }
        }

        private static void GenerateLogServiceFile(string basePath, Type type, Type nullableAttributeType)
        {
            string loggedServiceName = GetLogServiceName(type.Name);

            var fileName = Path.Combine(basePath, loggedServiceName + ".cs");

            var generator = new LoggedServiceCodeGenerator(nullableAttributeType);

            var fileContent = generator.Generate(type);

            File.WriteAllText(fileName, fileContent);
        }

        private static string GetLogServiceName(string serviceName)
        {
            return $"Logged{serviceName.Substring(1)}";
        }
    }
}
