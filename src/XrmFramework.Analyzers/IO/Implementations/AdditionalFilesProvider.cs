using Microsoft.CodeAnalysis;

namespace XrmFramework.Analyzers.IO;

public class AdditionalFilesProvider : IAdditionalFilesProvider
{
    public AdditionalFilesProvider(ICollection<AdditionalText> files)
    {
        Files = files.ToList();
    }

    public ICollection<AdditionalText> Files { get; }
}