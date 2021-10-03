using System.Collections.Generic;
using BoDi;
using XrmFramework.Analytics;
using XrmFramework.Generator;
using XrmFramework.Generator.Interfaces;

namespace XrmFramework.MSBuild.Generation
{
    public class WrappedGeneratorContainerBuilder
    {
        private readonly GeneratorContainerBuilder _generatorContainerBuilder;
        private readonly GenerateTableFileCodeBehindTaskConfiguration _generateTableFileCodeBehindTaskConfiguration;

        public WrappedGeneratorContainerBuilder(GeneratorContainerBuilder generatorContainerBuilder, GenerateTableFileCodeBehindTaskConfiguration generateTableFileCodeBehindTaskConfiguration)
        {
            _generatorContainerBuilder = generatorContainerBuilder;
            _generateTableFileCodeBehindTaskConfiguration = generateTableFileCodeBehindTaskConfiguration;
        }

        public IObjectContainer BuildGeneratorContainer(
            SpecFlowConfigurationHolder specFlowConfigurationHolder,
            ProjectSettings projectSettings,
            IReadOnlyCollection<GeneratorPluginInfo> generatorPluginInfos,
            IObjectContainer rootObjectContainer)
        {
            var objectContainer = _generatorContainerBuilder.CreateContainer(specFlowConfigurationHolder, projectSettings, generatorPluginInfos, rootObjectContainer);

            objectContainer.RegisterTypeAs<ProjectCodeBehindGenerator, IProjectCodeBehindGenerator>();
            objectContainer.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();

            if (_generateTableFileCodeBehindTaskConfiguration.OverrideTableFileCodeBehindGenerator is null)
            {
                objectContainer.RegisterTypeAs<TableFileCodeBehindGenerator, ITableFileCodeBehindGenerator>();
            }
            else
            {
                objectContainer.RegisterInstanceAs(_generateTableFileCodeBehindTaskConfiguration.OverrideTableFileCodeBehindGenerator);
            }

            return objectContainer;
        }
    }
}
