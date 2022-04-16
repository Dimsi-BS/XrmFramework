#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace XrmFramework.Analyzers.Model
{
    public class TableAdditionalText : AdditionalText
    {
        private string _text;

        public TableAdditionalText((string path, byte[] content) infos)
        {
            Path = infos.path;
            _text = Encoding.UTF8.GetString(infos.content);
        }

        /// <inheritdoc />
        public override SourceText? GetText(CancellationToken cancellationToken = new CancellationToken())
            => SourceText.From(_text);

        /// <inheritdoc />
        public override string Path { get; }
    }
}
