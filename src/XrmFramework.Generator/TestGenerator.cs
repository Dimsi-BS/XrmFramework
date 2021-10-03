using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using XrmFramework.Generator.CodeDom;
using XrmFramework.Generator.Interfaces;

namespace XrmFramework.Generator
{
    public class TestGenerator : ErrorHandlingTestGenerator, ITestGenerator
    {
        protected readonly ProjectSettings projectSettings;
        protected readonly ITestHeaderWriter testHeaderWriter;
        protected readonly ITestUpToDateChecker testUpToDateChecker;
        protected readonly CodeDomHelper codeDomHelper;
        private readonly ITableGeneratorRegistry tableGeneratorRegistry;


        public TestGenerator(ProjectSettings projectSettings,
            ITestHeaderWriter testHeaderWriter,
            ITestUpToDateChecker testUpToDateChecker,
            ITableGeneratorRegistry tableGeneratorRegistry,
            CodeDomHelper codeDomHelper)
        {
            if (projectSettings == null) throw new ArgumentNullException(nameof(projectSettings));
            if (testHeaderWriter == null) throw new ArgumentNullException(nameof(testHeaderWriter));
            if (testUpToDateChecker == null) throw new ArgumentNullException(nameof(testUpToDateChecker));
            if (tableGeneratorRegistry == null) throw new ArgumentNullException(nameof(tableGeneratorRegistry));
            
            this.testUpToDateChecker = testUpToDateChecker;
            this.tableGeneratorRegistry = tableGeneratorRegistry;
            this.codeDomHelper = codeDomHelper;
            this.testHeaderWriter = testHeaderWriter;
            this.projectSettings = projectSettings;
        }

        protected override TestGeneratorResult GenerateTestFileWithExceptions(TableFileInput tableFileInput, GenerationSettings settings)
        {
            if (tableFileInput == null) throw new ArgumentNullException("tableFileInput");
            if (settings == null) throw new ArgumentNullException("settings");

            var generatedTestFullPath = GetTestFullPath(tableFileInput);
            bool? preliminaryUpToDateCheckResult = null;
            if (settings.CheckUpToDate)
            {
                preliminaryUpToDateCheckResult = testUpToDateChecker.IsUpToDatePreliminary(tableFileInput, generatedTestFullPath, settings.UpToDateCheckingMethod);
                if (preliminaryUpToDateCheckResult == true)
                    return new TestGeneratorResult(null, true);
            }

            string generatedTestCode = GetGeneratedTestCode(tableFileInput);
            if(string.IsNullOrEmpty(generatedTestCode))
                return new TestGeneratorResult(null, true);

            if (settings.CheckUpToDate && preliminaryUpToDateCheckResult != false)
            {
                var isUpToDate = testUpToDateChecker.IsUpToDate(tableFileInput, generatedTestFullPath, generatedTestCode, settings.UpToDateCheckingMethod);
                if (isUpToDate)
                    return new TestGeneratorResult(null, true);
            }

            if (settings.WriteResultToFile)
            {
                File.WriteAllText(generatedTestFullPath, generatedTestCode, Encoding.UTF8);
            }

            return new TestGeneratorResult(generatedTestCode, false);
        }

        protected string GetGeneratedTestCode(TableFileInput tableFileInput)
        {
            using (var outputWriter = new IndentProcessingWriter(new StringWriter()))
            {
                var codeProvider = codeDomHelper.CreateCodeDomProvider();
                var codeNamespace = GenerateTestFileCode(tableFileInput);

                if (codeNamespace == null) return "";

                var options = new CodeGeneratorOptions
                                  {
                                      BracingStyle = "C",
                                  };

                AddSpecFlowHeader(codeProvider, outputWriter);
                codeProvider.GenerateCodeFromNamespace(codeNamespace, outputWriter, options);
                AddSpecFlowFooter(codeProvider, outputWriter);

                outputWriter.Flush();
                var generatedTestCode = outputWriter.ToString();
                return FixVBNetGlobalNamespace(generatedTestCode);
            }
        }

        private string FixVBNetGlobalNamespace(string generatedTestCode)
        {
            return generatedTestCode
                    .Replace("Global.GlobalVBNetNamespace", "Global")
                    .Replace("GlobalVBNetNamespace", "Global");
        }

        private CodeNamespace GenerateTestFileCode(TableFileInput tableFileInput)
        {
            string targetNamespace = GetTargetNamespace(tableFileInput) ?? "SpecFlow.GeneratedTests";
            
            SpecFlowDocument specFlowDocument;
            using (var contentReader = tableFileInput.GetTableFileContentReader(projectSettings))
            {
                specFlowDocument = ParseContent(parser, contentReader, GetSpecFlowDocumentLocation(tableFileInput));
            }

            if (specFlowDocument.SpecFlowTable == null) return null;

            var tableGenerator = tableGeneratorRegistry.CreateGenerator(specFlowDocument);

            var codeNamespace = tableGenerator.GenerateUnitTestFixture(specFlowDocument, null, targetNamespace);
            return codeNamespace;
        }

        private SpecFlowDocumentLocation GetSpecFlowDocumentLocation(TableFileInput tableFileInput)
        {
            return new SpecFlowDocumentLocation(
                tableFileInput.GetFullPath(projectSettings), 
                GetTableFolderPath(tableFileInput.ProjectRelativePath));
        }

        private string GetTableFolderPath(string projectRelativeFilePath)
        {
            string directoryName = Path.GetDirectoryName(projectRelativeFilePath);
            if (string.IsNullOrWhiteSpace(directoryName)) return null;

            return string.Join("/", directoryName.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
        }

        protected virtual SpecFlowDocument ParseContent(IGherkinParser parser, TextReader contentReader, SpecFlowDocumentLocation documentLocation)
        {
            return parser.Parse(contentReader, documentLocation);
        }

        protected string GetTargetNamespace(TableFileInput tableFileInput)
        {
            if (!string.IsNullOrEmpty(tableFileInput.CustomNamespace))
                return tableFileInput.CustomNamespace;

            string targetNamespace = projectSettings == null || string.IsNullOrEmpty(projectSettings.DefaultNamespace)
                ? null
                : projectSettings.DefaultNamespace;

            var directoryName = Path.GetDirectoryName(tableFileInput.ProjectRelativePath);
            string namespaceExtension = string.IsNullOrEmpty(directoryName) ? null :
                string.Join(".", directoryName.TrimStart('\\', '/', '.').Split('\\', '/').Select(f => f.ToIdentifier()).ToArray());
            if (!string.IsNullOrEmpty(namespaceExtension))
                targetNamespace = targetNamespace == null
                    ? namespaceExtension
                    : targetNamespace + "." + namespaceExtension;
            return targetNamespace;
        }

        public string GetTestFullPath(TableFileInput tableFileInput)
        {
            var path = tableFileInput.GetGeneratedTestFullPath(projectSettings);
            if (path != null)
                return path;

            return tableFileInput.GetFullPath(projectSettings) + GenerationTargetLanguage.GetExtension(projectSettings.ProjectPlatformSettings.Language);
        }

        #region Header & Footer
        protected override Version DetectGeneratedTestVersionWithExceptions(TableFileInput tableFileInput)
        {
            var generatedTestFullPath = GetTestFullPath(tableFileInput);
            return testHeaderWriter.DetectGeneratedTestVersion(tableFileInput.GetGeneratedTestContent(generatedTestFullPath));
        }

        protected void AddSpecFlowHeader(CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            const string specFlowHeaderTemplate = @"------------------------------------------------------------------------------
 <auto-generated>
     This code was generated by SpecFlow (https://www.specflow.org/).
     SpecFlow Version:{0}
     SpecFlow Generator Version:{1}

     Changes to this file may cause incorrect behavior and will be lost if
     the code is regenerated.
 </auto-generated>
------------------------------------------------------------------------------";

            var headerReader = new StringReader(string.Format(specFlowHeaderTemplate,
                GetCurrentSpecFlowVersion(),
                TestGeneratorFactory.GeneratorVersion
                ));

            string line;
            while ((line = headerReader.ReadLine()) != null)
            {
                codeProvider.GenerateCodeFromStatement(new CodeCommentStatement(line), outputWriter, null);
            }

            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetStartRegionStatement("Designer generated code"), outputWriter, null);
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetDisableWarningsPragma(), outputWriter, null);
        }

        protected void AddSpecFlowFooter(CodeDomProvider codeProvider, TextWriter outputWriter)
        {
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetEnableWarningsPragma(), outputWriter, null);
            codeProvider.GenerateCodeFromStatement(codeDomHelper.GetEndRegionStatement(), outputWriter, null);
        }

        protected Version GetCurrentSpecFlowVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        #endregion
    }
}
