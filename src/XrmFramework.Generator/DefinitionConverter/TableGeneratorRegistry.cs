using System.Collections.Generic;
using System.Linq;
using BoDi;

namespace XrmFramework.Generator.DefinitionConverter
{
    public class TableGeneratorRegistry : IFeatureGeneratorRegistry
    {
        private readonly List<ITableGeneratorProvider> providers;

        public TableGeneratorRegistry(IObjectContainer objectContainer)
        {
            providers = objectContainer.ResolveAll<ITableGeneratorProvider>().ToList().OrderBy(item => item.Priority).ToList();
        }

        public ITableGenerator CreateGenerator(SpecFlowDocument document)
        {
            var providerItem = FindProvider(document);
            return providerItem.CreateGenerator(document);
        }

        private ITableGeneratorProvider FindProvider(SpecFlowDocument feature)
        {
            return providers.First(item => item.CanGenerate(feature));
        }
    }
}