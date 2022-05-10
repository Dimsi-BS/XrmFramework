namespace XrmFramework.Analyzers.IO;

public interface IModelLoader
{
    IEnumerable<Core.Model> Load(CancellationToken cancellationToken = default);
}