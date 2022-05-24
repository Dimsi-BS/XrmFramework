using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace XrmFramework.Analyzers.Generators
{
    [Generator]
    public class ModelSourceFileGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Get the .model files


            // Read their contents and save their ,aes
        }

        private void WriteModelFiles(SourceProductionContext productionContext, List<XrmFramework.Core.Model> models)
        {

        }
    }
}
