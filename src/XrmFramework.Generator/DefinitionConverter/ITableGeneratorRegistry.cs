namespace XrmFramework.Generator.DefinitionConverter
{
    public interface IFeatureGeneratorRegistry
    {
        ITableGenerator CreateGenerator(XrmFrameworkDocument document);
    }
}