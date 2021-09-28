using System.CodeDom;

namespace XrmFramework.Generator.DefinitionConverter
{
    public interface ITableGenerator
    {
        CodeNamespace GenerateUnitTestFixture(SpecFlowDocument document, string testClassName, string targetNamespace);
    }
}