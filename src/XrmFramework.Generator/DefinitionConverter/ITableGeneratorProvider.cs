namespace XrmFramework.Generator.DefinitionConverter
{
    public interface ITableGeneratorProvider
    {
        int Priority { get; }
        bool CanGenerate(SpecFlowDocument document);
        ITableGenerator CreateGenerator(SpecFlowDocument document);
    }
}