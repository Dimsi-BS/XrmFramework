using System.CodeDom;
using System.Collections.Generic;

namespace XrmFramework.Generator
{
    public class TestClassGenerationContext
    {
        public ITableGeneratorProvider UnitTestGeneratorProvider { get; private set; }
        public XrmFrameworkDocument Document { get; private set; }

        public XrmFrameworkTable Table => Document.XrmFrameworkTable;

        public CodeNamespace Namespace { get; private set; }
        public CodeTypeDeclaration TestClass { get; private set; }
        public CodeMemberMethod TestClassInitializeMethod { get; private set; }
        public CodeMemberMethod TestClassCleanupMethod { get; private set; }
        public CodeMemberMethod TestInitializeMethod { get; private set; }
        public CodeMemberMethod TestCleanupMethod { get; private set; }
        public CodeMemberMethod ScenarioInitializeMethod { get; private set; }
        public CodeMemberMethod ScenarioStartMethod { get; private set; }
        public CodeMemberMethod ScenarioCleanupMethod { get; private set; }
        public CodeMemberMethod TableBackgroundMethod { get; private set; }
        public CodeMemberField TestRunnerField { get; private set; }

        public bool GenerateRowTests { get; private set; }

        public IDictionary<string, object> CustomData { get; private set; }

        public TestClassGenerationContext(
            ITableGeneratorProvider unitTestGeneratorProvider,
            XrmFrameworkDocument document,
            CodeNamespace ns,
            CodeTypeDeclaration testClass,
            CodeMemberField testRunnerField,
            CodeMemberMethod testClassInitializeMethod,
            CodeMemberMethod testClassCleanupMethod,
            CodeMemberMethod testInitializeMethod,
            CodeMemberMethod testCleanupMethod,
            CodeMemberMethod scenarioInitializeMethod,
            CodeMemberMethod scenarioStartMethod,
            CodeMemberMethod scenarioCleanupMethod,
            CodeMemberMethod featureBackgroundMethod,
            bool generateRowTests)
        {
            UnitTestGeneratorProvider = unitTestGeneratorProvider;
            Document = document;
            Namespace = ns;
            TestClass = testClass;
            TestRunnerField = testRunnerField;
            TestClassInitializeMethod = testClassInitializeMethod;
            TestClassCleanupMethod = testClassCleanupMethod;
            TestInitializeMethod = testInitializeMethod;
            TestCleanupMethod = testCleanupMethod;
            ScenarioInitializeMethod = scenarioInitializeMethod;
            ScenarioStartMethod = scenarioStartMethod;
            ScenarioCleanupMethod = scenarioCleanupMethod;
            TableBackgroundMethod = featureBackgroundMethod;
            GenerateRowTests = generateRowTests;

            CustomData = new Dictionary<string, object>();
        }
    }
}