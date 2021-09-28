using System.Collections.Generic;
using System.Linq;

namespace XrmFramework.Generator.DefinitionConverter
{
    public class ParallelizeDecorator : ITestClassDecorator
    {
        private readonly string[] ignoreParallelCodeGenerationTags;
        private readonly ITagFilterMatcher tagFilterMatcher;
        private readonly bool generateParallelCodeForFeatures;

        public ParallelizeDecorator(ITagFilterMatcher tagFilterMatcher, SpecFlowConfiguration generatorConfiguration)
        {
            this.tagFilterMatcher = tagFilterMatcher;
            this.generateParallelCodeForFeatures = generatorConfiguration.MarkFeaturesParallelizable;
            this.ignoreParallelCodeGenerationTags = generatorConfiguration.SkipParallelizableMarkerForTags;
        }

        public int Priority
        { 
            get { return PriorityValues.Low; }
        }

        public bool CanDecorateFrom(TestClassGenerationContext generationContext)
        {
            return CanGenerateParallelCodeBasedOnConfiguration(generationContext) && !IgnoreParallelCodeGenerationBasedOnTags(generationContext.Feature.Tags.Select(x=>x.GetNameWithoutAt()));
        }

        public void DecorateFrom(TestClassGenerationContext generationContext)
        {
            generationContext.UnitTestGeneratorProvider.SetTestClassParallelize(generationContext);
        }

        private bool CanGenerateParallelCodeBasedOnConfiguration(TestClassGenerationContext generationContext)
        {
            return generationContext.UnitTestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution) && generateParallelCodeForFeatures;
        }
        private bool IgnoreParallelCodeGenerationBasedOnTags(IEnumerable<string> tagName)
        {
            return ignoreParallelCodeGenerationTags.Any(x => tagFilterMatcher.Match(x, tagName));
        }
    }
}