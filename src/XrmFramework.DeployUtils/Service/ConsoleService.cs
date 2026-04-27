using Spectre.Console;

namespace XrmFramework.DeployUtils.Service;

/// <summary>
///     Simple console service that writes deploy status messages using AnsiConsole
/// </summary>
public class ConsoleService : IConsoleService
{
    public void SetStatus(string message)
    {
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(message)}[/]");
    }
}
