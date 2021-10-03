using BoDi;
using XrmFramework.Generator.Configuration;
using XrmFramework.Generator.Generation;
using XrmFramework.Generator.Interfaces;
using XrmFramework.Generator.Plugins;
using XrmFramework.Generator.DefinitionConverter;
using XrmFramework.Parser;
using XrmFramework.Tracing;
using XrmFramework.Utils;

namespace XrmFramework.Generator
{
    internal partial class DefaultDependencyProvider
    {
        partial void RegisterDefinitionGeneratorProviders(ObjectContainer container);

        public virtual void RegisterDefaults(ObjectContainer container)
        {
            container.RegisterTypeAs<FileSystem, IFileSystem>();
            
            container.RegisterTypeAs<InProcGeneratorInfoProvider, IGeneratorInfoProvider>();
            container.RegisterTypeAs<TestGenerator, ITestGenerator>();
            container.RegisterTypeAs<TestHeaderWriter, ITestHeaderWriter>();
            container.RegisterTypeAs<TestUpToDateChecker, ITestUpToDateChecker>();

            container.RegisterTypeAs<GeneratorPluginLoader, IGeneratorPluginLoader>();
            container.RegisterTypeAs<DefaultListener, ITraceListener>();

            container.RegisterTypeAs<DefinitionTableGenerator, DefinitionTableGenerator>();
            container.RegisterTypeAs<TableGeneratorRegistry, ITableGeneratorRegistry>();
            container.RegisterTypeAs<DefinitionTableGeneratorProvider, ITableGeneratorProvider>("default");
            container.RegisterTypeAs<TagFilterMatcher, ITagFilterMatcher>();

            container.RegisterTypeAs<DecoratorRegistry, IDecoratorRegistry>();
            container.RegisterTypeAs<IgnoreDecorator, ITestClassTagDecorator>("ignore");
            container.RegisterTypeAs<IgnoreDecorator, ITestMethodTagDecorator>("ignore");
            container.RegisterTypeAs<ParallelizeDecorator, ITestClassDecorator>("parallelize");

            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.CSharp), GenerationTargetLanguage.CSharp, dispose: true);
            container.RegisterInstanceAs(GenerationTargetLanguage.CreateCodeDomHelper(GenerationTargetLanguage.VB), GenerationTargetLanguage.VB, dispose: true);
            
            RegisterDefinitionGeneratorProviders(container);
        }
    }
}
