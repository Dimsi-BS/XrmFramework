using Microsoft.CodeAnalysis;

namespace XrmFramework.Analyzers.IO;

public interface IAdditionalFilesProvider
{
    ICollection<AdditionalText> Files { get; }
}