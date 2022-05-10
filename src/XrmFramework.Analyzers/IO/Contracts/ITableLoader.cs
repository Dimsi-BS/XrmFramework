using XrmFramework.Core;

namespace XrmFramework.Analyzers.IO;

public interface ITableLoader
{
    TableCollection Load(CancellationToken cancellationToken = default);
}