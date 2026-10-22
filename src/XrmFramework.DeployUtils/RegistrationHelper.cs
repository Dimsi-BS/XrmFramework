// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Spectre.Console;
using XrmFramework.DeployUtils.CommandOptions;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.DeployUtils.Context;
using XrmFramework.DeployUtils.Exporters;
using XrmFramework.DeployUtils.Factories;
using XrmFramework.DeployUtils.Model;
using XrmFramework.DeployUtils.Model.Interfaces;
using XrmFramework.DeployUtils.Service;
using IAssemblyFactory = XrmFramework.DeployUtils.Factories.IAssemblyFactory;

namespace XrmFramework.DeployUtils;

public partial class RegistrationHelper
{
    private readonly AssemblyDiffFactory _assemblyDiffFactory;
    private readonly IAssemblyExporter _assemblyExporter;
    private readonly IAssemblyFactory _assemblyFactory;
    private readonly IConsoleService _consoleService;
    private readonly IRegistrationService _registrationService;

    public RegistrationHelper(IRegistrationService registrationService,
      IAssemblyExporter assemblyExporter,
      IAssemblyFactory assemblyFactory,
      IConsoleService consoleService,
      AssemblyDiffFactory assemblyDiffFactory)
    {
        _registrationService = registrationService;
        _assemblyExporter = assemblyExporter;
        _assemblyFactory = assemblyFactory;
        _consoleService = consoleService;
        _assemblyDiffFactory = assemblyDiffFactory;
    }

    /// <summary>
    ///     Entrypoint for registering the <typeparamref name="TPlugin" /> assembly in the CRM solution
    ///     <paramref name="projectName" />.
    /// </summary>
    /// <typeparam name="TPlugin">
    ///     Root type of all components to deploy, should be <c>XrmFramework.Plugin</c>.
    /// </typeparam>
    /// <param name="projectName">
    ///     Name of the target CRM solution (e.g. <c>"MyProject.Plugins"</c>).
    /// </param>
    /// <param name="isOnPremise">
    ///     <see langword="true" /> for an On-Premises CRM; <see langword="false" /> for Dataverse Online.
    /// </param>
    /// <param name="args">
    ///     Command-line arguments forwarded from <c>Program.cs</c>.
    ///     Supported options:
    ///     <list type="bullet">
    ///         <item>
    ///             <c>-n</c> / <c>--noprompt</c> / <c>-NoPrompt</c> — skip the interactive connection
    ///             confirmation (CI/CD mode).
    ///         </item>
    ///     </list>
    /// </param>
    public static void RegisterPluginsAndWorkflows<TPlugin>(
        string projectName,
        bool isOnPremise,
        string[] args)
    {
        // ── 1. Parse the args to extract the deployment options ─────────
        var noPrompt = false;

        Parser.Default
            .ParseArguments<DeployCommandOptions>(CommandLineAliases.NormalizeNoPrompt(args))
            .WithParsed(opts => noPrompt = opts.NoPrompt)
            .WithNotParsed(_ => { /* unknown options silently ignored */ });

        // The assembly is resolved at compile time via the provided root type.
        var exitCode = RegisterPluginsAndWorkflows(typeof(TPlugin).Assembly, projectName, isOnPremise, noPrompt);
        if (exitCode != 0)
            Environment.Exit(exitCode);
    }

    /// <summary>
    ///     Registers the <paramref name="localDll" /> assembly (plugins, custom APIs, workflows)
    ///     into the CRM solution <paramref name="projectName" />.
    /// </summary>
    /// <param name="localDll">
    ///     Assembly to deploy. Must contain the XrmFramework base types (compiled from
    ///     the <c>XrmFramework.Plugin</c> source package) and appear in <c>xrmFramework.config</c>.
    /// </param>
    /// <param name="projectName">Name of the target project/solution (e.g. <c>"MyProject.Plugins"</c>).</param>
    /// <param name="isOnPremise"><see langword="true" /> for On-Premises; otherwise Dataverse Online.</param>
    /// <param name="noPrompt">Silent mode: skips the interactive confirmation (CI/CD).</param>
    /// <returns>Exit code: <c>0</c> success (or cancellation at the prompt), <c>3</c> unexpected error.</returns>
    /// <summary>
    ///     Compatibility overload: infers the loaded assembly's path (<c>localDll.Location</c>)
    ///     and delegates to <see cref="RegisterPluginsAndWorkflows(string,string,bool,bool)" />.
    /// </summary>
    public static int RegisterPluginsAndWorkflows(
        Assembly localDll,
        string projectName,
        bool isOnPremise,
        bool noPrompt)
        => RegisterPluginsAndWorkflows(localDll.Location, projectName, isOnPremise, noPrompt);

    public static int RegisterPluginsAndWorkflows(
        string dllPath,
        string projectName,
        bool isOnPremise,
        bool noPrompt)
    {
        try
        {
            var deployOptions = new DeployOptions
            {
                IsOnPremise = isOnPremise,
                NoPrompt = noPrompt
            };

            // ── Initialize the DI container ─────────────────────────────────────
            var serviceCollection = DeployServiceCollectionFactory
                .CreateServiceCollection(projectName, deployOptions);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var deploySettings = serviceProvider.GetRequiredService<DeploySettings>();

            // ── Display the connection summary ─────────────────────────────────
            AnsiConsole.WriteLine($"Assembly  : {Path.GetFileNameWithoutExtension(dllPath)}");
            AnsiConsole.WriteLine($"Target    : {deploySettings.Url}");
            AnsiConsole.WriteLine($"ClientId  : {deploySettings.ClientId}");
            AnsiConsole.WriteLine($"OnPremise : {(isOnPremise ? "yes" : "no")}");

            // ── Interactive confirmation (unless silent mode) ────────────────
            if (!deployOptions.NoPrompt && !AnsiConsole.Confirm("Continue the deployment?"))
            {
                return 0;
            }

            // ── Connection and deployment ───────────────────────────────────────
            AnsiConsole.WriteLine("Connecting to CRM...");

            var solutionContext = serviceProvider.GetRequiredService<ISolutionContext>();
            solutionContext.InitSolutionContext();

            var registrationHelper = serviceProvider.GetRequiredService<RegistrationHelper>();
            registrationHelper.Register(dllPath);

            return 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return 3;
        }
    }

    /// <summary>
    ///     Main algorithm for deploying the plugin assembly located at <paramref name="dllPath" />.
    /// </summary>
    /// <param name="dllPath">The local plugin assembly path, should appear in <c>xrmFramework.config</c>.</param>
    private void Register(string dllPath)
    {
        _consoleService.SetStatus("Fetching Local Assembly...");

        // The local inventory is obtained by EXECUTING the registration code (constructors /
        // AddSteps) via XrmFramework.PluginInventory: in-process net462, out-of-process net462 exe on net8.
        var localAssembly = _assemblyFactory.CreateFromLocalAssemblyContext(dllPath);

        _consoleService.SetStatus("Fetching Remote Assembly...");

        var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, localAssembly.AssemblyInfo.Name);

        _consoleService.SetStatus("Computing Difference...");

        var registrationStrategy = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

        PrintChangeSummary(registrationStrategy.ComponentsOrderedPool);

        _consoleService.SetStatus(@"Executing Registration Strategy...");

        ExecuteStrategy(registrationStrategy);
    }

    /// <summary>
    ///     Prints an explicit, human-readable list of every component that is about to be
    ///     created, updated or deleted, so the changes a deployment is going to make are never silent.
    /// </summary>
    /// <param name="componentsPool">The full component pool, already flagged with a <see cref="RegistrationState" />.</param>
    private static void PrintChangeSummary(IEnumerable<ICrmComponent> componentsPool)
    {
        var changes = componentsPool
            .Where(c => c.RegistrationState is RegistrationState.ToCreate or RegistrationState.ToUpdate or RegistrationState.ToDelete)
            .ToList();

        AnsiConsole.WriteLine();

        if (changes.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No changes detected: nothing to deploy.[/]");
            AnsiConsole.WriteLine();
            return;
        }

        AnsiConsole.MarkupLine("[bold]Changes to apply:[/]");

        foreach (var component in changes)
        {
            var (verb, color) = component.RegistrationState switch
            {
                RegistrationState.ToCreate => ("Add", "green"),
                RegistrationState.ToUpdate => ("Update", "yellow"),
                RegistrationState.ToDelete => ("Remove", "red"),
                _ => ("?", "grey")
            };

            AnsiConsole.MarkupLine($"  [{color}]{verb}[/] {Markup.Escape(DescribeComponent(component))}");
        }

        AnsiConsole.WriteLine();
    }

    /// <summary>
    ///     Builds a short, human-readable description of a component for the change summary
    ///     (e.g. <c>plugin 'MySolution.Plugins.AccountPlugin'</c>, <c>assembly 'MySolution.Plugins' (version 1.0.0.2)</c>).
    /// </summary>
    private static string DescribeComponent(ICrmComponent component) => component switch
    {
        AssemblyInfo assembly => $"assembly '{assembly.Name}' (version {assembly.Version})",
        PluginPackage package => $"NuGet package '{package.Name}' (version {package.Version})",
        Plugin { IsWorkflow: true } workflow => $"workflow activity '{workflow.DisplayName}'",
        Plugin plugin => $"plugin '{plugin.FullName}'",
        Step step => $"step '{step.Description}'",
        StepImage image => $"{(image.IsPreImage ? "pre" : "post")}-image of step '{image.FatherStep.Description}'",
        CustomApi api => $"Custom API '{api.Name}'",
        CustomApiRequestParameter param => $"Custom API parameter '{param.UniqueName}'",
        CustomApiResponseProperty prop => $"Custom API response property '{prop.UniqueName}'",
        _ => $"{component.GetType().Name} '{component.UniqueName}'"
    };

    /// <summary>
    ///     Deploy the <paramref name="strategy" />'s components according to their <see cref="RegistrationState" />
    /// </summary>
    /// <param name="strategy"></param>
    private void ExecuteStrategy(IAssemblyContext strategy)
    {
        var strategyPool = strategy.ComponentsOrderedPool;

        var stepsForMetadata = strategyPool.OfType<Step>()
          .Where(s => s.RegistrationState is RegistrationState.ToCreate or RegistrationState.ToUpdate);

        _assemblyExporter.InitExportMetadata(stepsForMetadata);

        var componentsToDelete = strategyPool.Where(c =>
                                                        c.RegistrationState == RegistrationState.ToDelete);
        _assemblyExporter.DeleteAllComponents(componentsToDelete);

        var componentsToUpdate = strategyPool.Where(c =>
                                                        c.RegistrationState == RegistrationState.ToUpdate);
        _assemblyExporter.UpdateAllComponents(componentsToUpdate);

        var componentsToCreate = strategyPool.Where(c =>
            c.RegistrationState == RegistrationState.ToCreate);
        _assemblyExporter.CreateAllComponents(componentsToCreate);
    }
}
