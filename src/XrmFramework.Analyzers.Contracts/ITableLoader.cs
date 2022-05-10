using System.Threading;
using XrmFramework.Core;

namespace XrmFramework.Analyzers.Contracts;

public interface ITableLoader
{
    TableCollection Load(CancellationToken cancellationToken = default);
}