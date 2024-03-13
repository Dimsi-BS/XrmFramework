using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils;
using XrmFramework.RemoteDebugger.Client.Recorder;
using XrmFramework.DeployUtils.Service;

namespace XrmFramework.RemoteDebugger.Client.Commands;

internal class RemoteDebuggerCommand : AsyncCommand<RemoteDebuggerCommand.Settings>
{
	private readonly ISolutionContext _solutionContext;
	private readonly RegistrationHelper _registrationHelper;
	private readonly IConsoleService _consoleService;
	private readonly IRemoteDebuggerMessageManager _messageManager;

	public RemoteDebuggerCommand(ISolutionContext solutionContext,
	                             RegistrationHelper registrationHelper,
	                             IConsoleService consoleService,
	                             IRemoteDebuggerMessageManager messageManager)
	{
		_solutionContext    = solutionContext;
		_registrationHelper = registrationHelper;
		_consoleService     = consoleService;
		_messageManager     = messageManager;
	}

	public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
	{
		var consoleTask = _consoleService.StartAsync();

		try
		{
			var assembliesToDebug = Assembly.GetCallingAssembly().GetReferencedAssemblies()
			   .Select(Assembly.Load)
			   .Where(a => a.GetType("XrmFramework.Plugin")                             != null
			               || a.GetType("XrmFramework.CustomApi")                       != null
			               || a.GetType("XrmFramework.Workflow.CustomWorkflowActivity") != null
				)
			   .ToList();

			if (!assembliesToDebug.Any())
			{
				throw new ArgumentException(
					"No project containing components to debug were found, please check that they are referenced");
			}

			assembliesToDebug.ForEach(assembly =>
			{
				var targetSolutionName = new TargetSolutionProvider(assembly.GetName().Name).GetTargetSolution();
				_solutionContext.InitSolutionContext(targetSolutionName);
				_registrationHelper.UpdateDebugger(assembly);
			});

			await _messageManager.RunAsync();

			await consoleTask;

			return 0;
		}
		catch (Exception ex)
		{
			AnsiConsole.WriteLine();
			AnsiConsole.MarkupLine("An error occured:");
			AnsiConsole.WriteException(ex);

			return -1;
		}
	}

	public class Settings : CommandSettings { }
}
