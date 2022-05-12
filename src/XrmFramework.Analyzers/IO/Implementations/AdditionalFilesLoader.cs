using Newtonsoft.Json;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.IO
{
    internal class AdditionalFilesLoader : IModelLoader, ITableLoader
    {
        private readonly IAdditionalFilesProvider _provider;

        public AdditionalFilesLoader(IAdditionalFilesProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public IEnumerable<Core.Model> Load(CancellationToken cancellationToken = default)
        {
            var additionalTexts = _provider.Files.Where(f => f.Path.EndsWith(".model"));

            foreach (var additionalText in additionalTexts)
            {
                var sourceText = additionalText.GetText(cancellationToken);

                if (sourceText == null)
                {
                    continue;
                }

                var text = sourceText.ToString();

                var model = JsonConvert.DeserializeObject<Core.Model>(text);

                yield return model;
            }
        }

        /// <inheritdoc />
        TableCollection ITableLoader.Load(CancellationToken cancellationToken)
        {
            var tableCollection = new TableCollection();

            var additionalTexts = _provider.Files.Where(f => f.Path.EndsWith(".table"));

            foreach (var additionalText in additionalTexts)
            {
                var sourceText = additionalText.GetText(cancellationToken);

                if (sourceText == null)
                {
                    continue;
                }

                var text = sourceText.ToString();

                var table = JsonConvert.DeserializeObject<Core.Table>(text);

                tableCollection.Add(table);
            }

            return tableCollection;
        }
    }
}
