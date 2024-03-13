using MediatR;
using Newtonsoft.Json;
using Spectre.Console;
using XrmFramework.DeployUtils.Model.Record;
using XrmFramework.RemoteDebugger.Client.Notifications;

namespace XrmFramework.RemoteDebugger.Client.Recorder;

public class ConsoleService : IConsoleService
{
    private readonly Layout _mainLayout = new Layout();
    private Screens _currentScreen = Screens.SessionView;
    private readonly HashSet<IRecordedCorrelation> _correlations = new();
    private readonly IMediator _mediator;
    private LiveDisplayContext _displayContext;
    private bool _isStopped;
    private bool _isMainScreenVisible = false;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private IRecordedCorrelation CurrentCorrelation => _correlations.FirstOrDefault(c => c.Selected);

    public ConsoleService(IMediator mediator)
    {
        _mainLayout
                    .SplitRows(
                        new Layout()
                        .SplitColumns(
                            new Layout("Left")
                            .Size(40),
                            new Layout("Content")
                        ),
                        new Layout("Bottom")
                        .Size(1));
        _mediator = mediator;
    }

    public void AddCorrelation(IRecordedCorrelation recordedCorrelation)
    {
        if (!_correlations.Any())
        {
            recordedCorrelation.Selected = true;
        }

        _correlations.Add(recordedCorrelation);

        RefreshMainContent();
    }

    public void Dispose()
        => Stop();

    public void Refresh()
        => _displayContext.Refresh();

    public void RefreshMainContent()
    {
        Panel panel = null;
        if (_currentScreen == Screens.SessionView)
        {
            panel = GetSessionViewPanel();
        }
        else if (_currentScreen == Screens.CorrelationView)
        {
            panel = GetCorrelationViewPanel();
        }
        else
        {
            panel = new Panel("Unknown").Expand();
        }

        _mainLayout["Content"].Update(panel);

        _displayContext.Refresh();
    }

    private Panel GetSessionViewPanel()
    {
        var table = new Table()
                .AddColumns("CorrelationId", "NbPlugins")
                .Expand();
        var panel =
            new Panel(table)
            .Header("Session", Justify.Left)
            .Expand();

        foreach (var correlation in _correlations)
        {
            table.AddRow(new[] {
                new Markup((correlation.Selected? "[bold]* " : "  ") + correlation.Id.ToString().Substring(0, 5) + " " + correlation.Children.FirstOrDefault()?.Name + (correlation.Selected? "[/]": string.Empty)),
                new Markup(correlation.Children.Count.ToString())
            });
        }

        return panel;
    }

    private Panel GetCorrelationViewPanel()
    {
        var table = new Table()
                .AddColumns("PluginName", "NbMessages")
                .Expand();
        var panel =
            new Panel(table)
            .Header("Correlation", Justify.Left)
            .Expand();

        foreach (var plugin in CurrentCorrelation.Children)
        {
            table.AddRow(new[] {
                new Markup((plugin.Selected? "[bold]* " : "  ") + plugin.Id.ToString().Substring(0, 5) + " " + plugin.Name + (plugin.Selected? "[/]": string.Empty)),
                new Markup(plugin.Children.Count.ToString())
            });
        }

        return panel;
    }

    private void SaveSessionToFile()
    {
        _cancellationTokenSource.Cancel();

        var selectionPrompt = new SelectionPrompt<IRemoteDebuggerObject>()
            .UseConverter(m => m.Name)
            .Title("Choose the element you want to save");

        foreach (var correlation in _correlations)
        {
            selectionPrompt.AddChoiceGroup(correlation, correlation.Children);
        }

        var selectedObject = AnsiConsole.Prompt(selectionPrompt);

        var content = JsonConvert.SerializeObject(selectedObject, Formatting.Indented);

        
    }

    public Task SetStatus(string statusMarkup, Color? color = null)
    {
        _mainLayout["Bottom"].Update(
            new Panel(
                color.HasValue
                ? new Markup($"[grey]Status: [/][bold]{statusMarkup}[/]", color)
                : new Markup($"[grey]Status: [/][bold]{statusMarkup}[/]")
            ).NoBorder()
        );
        _displayContext.Refresh();

        return Task.CompletedTask;
    }

    public async Task StartAsync()
    {
        _isMainScreenVisible = false;

        _mainLayout["Content"].Update(
            new Panel("")
            .Header("Correlations", Justify.Left)
            .Expand()
            );

        _mainLayout["Left"].Update(
            new Panel("")
            .Header("Infos", Justify.Center)
            .Expand()
            );

        _mainLayout["Bottom"].Update(
            new Panel(new Markup("[grey]Status: [/]"))
            .NoBorder()
            );

        var _ = _mediator.Publish(new StartMainScreenNotification());
    }

    private async Task ConsoleInputReader(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var key = await AnsiConsole.Console.Input.ReadKeyAsync(true, cancellationToken);

            if (key.HasValue)
            {
                switch (key.Value.Key)
                {
                    case ConsoleKey.DownArrow:
                        SelectNextElement();
                        break;
                    case ConsoleKey.UpArrow:
                        SelectPreviousElement();
                        break;
                    case ConsoleKey.Q:
                        if ((key.GetValueOrDefault().Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        {
                            Stop();
                        }
                        break;
                    case ConsoleKey.S:
                        if ((key.GetValueOrDefault().Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                        {
                            SaveSessionToFile();
                        }
                        break;
                    case ConsoleKey.Enter:
                        if (_currentScreen == Screens.SessionView)
                        {
                            _currentScreen = Screens.CorrelationView;
                        }
                        break;
                    case ConsoleKey.Escape:
                        if (_currentScreen is Screens.CorrelationView)
                        {
                            _currentScreen = Screens.SessionView;
                        }
                        break;
                }
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                RefreshMainContent();
            }
        }
    }

    public async Task StartMainScreenAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = _cancellationTokenSource.Token;

        var taskReader = ConsoleInputReader(cancellationToken);

        var liveMainScreen = AnsiConsole
                .Live(_mainLayout)
                .AutoClear(true)
                .StartAsync(async ctx =>
                {
                    _displayContext = ctx;

                    while (true)
                    {
                        await Task.Delay(500);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        ctx.Refresh();
                    }
                });

        await Task.WhenAll(taskReader, liveMainScreen);
    }

    private void StopMainScreen()
        => _isMainScreenVisible = false;

    private void SelectNextElement()
    {
        var collection = GetScreenCollection();

        var selectedElement = collection.FirstOrDefault(c => c.Selected);

        var newSelectedElement = collection.SkipWhile(c => !c.Selected).Skip(1).FirstOrDefault();

        if (newSelectedElement != null)
        {
            selectedElement.Selected = false;
            newSelectedElement.Selected = true;
        }
    }

    private void SelectPreviousElement()
    {
        var collection = GetScreenCollection().Reverse();

        var selectedElement = collection.FirstOrDefault(c => c.Selected);

        var newSelectedElement = collection.SkipWhile(c => !c.Selected).Skip(1).FirstOrDefault();

        if (newSelectedElement != null)
        {
            selectedElement.Selected = false;
            newSelectedElement.Selected = true;
        }
    }

    private ICollection<IRemoteDebuggerObject> GetScreenCollection()
    {
        var collection = _correlations.Cast<IRemoteDebuggerObject>().ToList();

        if (_currentScreen == Screens.CorrelationView)
        {
            collection = CurrentCorrelation.Children.Cast<IRemoteDebuggerObject>().ToList();
        }

        return collection;
    }

    public void Stop()
    {
        _isStopped = true;
    }

    public async Task IsRunning()
    {
        while (true)
        {
            await Task.Delay(200);
            if (_isStopped)
            {
                return;
            }
        }
    }
}

public enum Screens
{
    SessionView,
    CorrelationView
}
