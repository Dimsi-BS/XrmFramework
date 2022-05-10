using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace XrmFramework.Analyzers.Contracts;

public interface IAdditionalFilesProvider
{
    ICollection<AdditionalText> Files { get; }
}