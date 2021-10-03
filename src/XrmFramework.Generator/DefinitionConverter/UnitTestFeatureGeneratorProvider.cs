namespace XrmFramework.Generator.DefinitionConverter
{
    public class UnitTestFeatureGeneratorProvider : ITableGeneratorProvider
    {
        private readonly UnitTestFeatureGenerator _unitTestFeatureGenerator;

        public UnitTestFeatureGeneratorProvider(UnitTestFeatureGenerator unitTestFeatureGenerator)
        {
            _unitTestFeatureGenerator = unitTestFeatureGenerator;
        }

        public int Priority => PriorityValues.Lowest;

        public bool CanGenerate(SpecFlowDocument document)
        {
            return true;
        }

        public ITableGenerator CreateGenerator(SpecFlowDocument document)
        {
            return _unitTestFeatureGenerator;
        }
    }
}