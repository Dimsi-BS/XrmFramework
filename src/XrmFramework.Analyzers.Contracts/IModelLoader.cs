using System.Collections.Generic;
using System.Threading;

namespace XrmFramework.Analyzers.Contracts;

public interface IModelLoader
{
    IEnumerable<Core.Model> Load(CancellationToken cancellationToken = default);
}