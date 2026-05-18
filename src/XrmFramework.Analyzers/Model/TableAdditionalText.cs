#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Threading;

namespace XrmFramework.Analyzers.Model
{
    /// <summary>
    /// In-memory <see cref="AdditionalText"/> implementation used by the
    /// <c>XrmFramework.Analyzers.Tests</c> harness to feed <c>.table</c> /
    /// <c>.model</c> JSON content to the incremental source generators
    /// without writing files to disk.
    /// </summary>
    public class TableAdditionalText : AdditionalText
    {
        private readonly string _text;

        public TableAdditionalText((string path, byte[] content) infos)
        {
            Path = infos.path;
            _text = Encoding.UTF8.GetString(infos.content);
        }

        public TableAdditionalText(string path, string content)
        {
            Path = path;
            _text = content;
        }

        /// <inheritdoc />
        public override SourceText? GetText(CancellationToken cancellationToken = default)
            => SourceText.From(_text);

        /// <inheritdoc />
        public override string Path { get; }
    }
}
