namespace XrmFramework.DeployUtils.Service;

/// <summary>
///     Provides a simple console output interface for the deploy pipeline
/// </summary>
public interface IConsoleService
{
    void SetStatus(string message);
}
