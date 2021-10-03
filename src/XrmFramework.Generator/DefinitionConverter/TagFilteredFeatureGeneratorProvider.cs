using System;

namespace XrmFramework.Generator.DefinitionConverter
{
    public abstract class TagFilteredFeatureGeneratorProvider : ITableGeneratorProvider
    {
        protected readonly ITagFilterMatcher tagFilterMatcher;
        protected readonly string registeredName;

        public virtual int Priority { get { return PriorityValues.Low; } }

        protected TagFilteredFeatureGeneratorProvider(ITagFilterMatcher tagFilterMatcher, string registeredName)
        {
            if (tagFilterMatcher == null) throw new ArgumentNullException("tagFilterMatcher");
            if (registeredName == null) throw new ArgumentNullException("registeredName");

            this.tagFilterMatcher = tagFilterMatcher;
            this.registeredName = registeredName;
        }

        public bool CanGenerate(SpecFlowDocument document)
        {
            return tagFilterMatcher.MatchPrefix(registeredName, document);
        }

        public abstract ITableGenerator CreateGenerator(SpecFlowDocument document);
    }
}