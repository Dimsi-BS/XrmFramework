// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
using XrmFramework.DeployUtils.Reporting;
using XrmFramework.DeployUtils.Service;
using IAssemblyFactory = XrmFramework.DeployUtils.Factories.IAssemblyFactory;

namespace XrmFramework.DeployUtils;

public partial class RegistrationHelper
{
    /// <summary>How often the deployment asks Dataverse whether it has finished unpacking a package.</summary>
    private static readonly TimeSpan PackagePollInterval = TimeSpan.FromSeconds(5);

    private readonly AssemblyDiffFactory _assemblyDiffFactory;
    private readonly IAssemblyExporter _assemblyExporter;
    private readonly IAssemblyFactory _assemblyFactory;
    private readonly IConsoleService _consoleService;
    private readonly IRegistrationService _registrationService;
    private readonly DeploySettings _deploySettings;
    private readonly DeployOptions _deployOptions;

    public RegistrationHelper(IRegistrationService registrationService,
      IAssemblyExporter assemblyExporter,
      IAssemblyFactory assemblyFactory,
      IConsoleService consoleService,
      AssemblyDiffFactory assemblyDiffFactory,
      DeploySettings deploySettings,
      DeployOptions deployOptions)
    {
        _registrationService = registrationService;
        _assemblyExporter = assemblyExporter;
        _assemblyFactory = assemblyFactory;
        _consoleService = consoleService;
        _assemblyDiffFactory = assemblyDiffFactory;
        _deploySettings = deploySettings;
        _deployOptions = deployOptions;
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
        => RegisterPluginsAndWorkflows(dllPath, projectName, isOnPremise, noPrompt, null);

    /// <param name="packagePath">
    ///     Explicit <c>.nupkg</c> to push, overriding the default resolution next to the assembly.
    ///     Only read for a project configured with <c>deployment="package"</c>.
    /// </param>
    /// <inheritdoc cref="RegisterPluginsAndWorkflows(string,string,bool,bool)" />
    public static int RegisterPluginsAndWorkflows(
        string dllPath,
        string projectName,
        bool isOnPremise,
        bool noPrompt,
        string packagePath)
    {
        try
        {
            var deployOptions = new DeployOptions
            {
                IsOnPremise = isOnPremise,
                NoPrompt = noPrompt,
                PackagePath = packagePath
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
            AnsiConsole.WriteLine($"Deploy as : {(deploySettings.UsePluginPackage ? "plugin package" : "plugin assembly")}");

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
        catch (OperationCanceledException ex)
        {
            // A declined confirmation is a choice, not a failure. Nothing has been written when it
            // is raised, and a non-zero code would fail a pipeline that did exactly as it was told.
            AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(ex.Message)}[/]");
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

        var localPackage = localAssembly.AssemblyInfo.Package;

        if (localPackage != null)
        {
            GuardPackageMode(localAssembly);
            PushPackageAndWait(localAssembly, localPackage);
        }

        _consoleService.SetStatus("Fetching Remote Assembly...");

        var registeredAssembly = _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, localAssembly.AssemblyInfo.Name);

        _consoleService.SetStatus("Computing Difference...");

        var registrationStrategy = _assemblyDiffFactory.ComputeDiffPatch(localAssembly, registeredAssembly);

        PrintChangeSummary(registrationStrategy.ComponentsOrderedPool);

        _consoleService.SetStatus(@"Executing Registration Strategy...");

        ExecuteStrategy(registrationStrategy);

        if (localPackage != null)
        {
            // The pluginassembly row belongs to the package, but solution membership does not come
            // along: it is added here, now that the platform has actually created the row.
            _assemblyExporter.AddComponentToSolution(registeredAssembly.AssemblyInfo);
        }
    }

    /// <summary>
    ///     Prints an explicit, human-readable list of every component that is about to be
    ///     created, updated or deleted, so the changes a deployment is going to make are never silent.
    /// </summary>
    /// <param name="componentsPool">The full component pool, already flagged with a <see cref="RegistrationState" />.</param>
    private static void PrintChangeSummary(IEnumerable<ICrmComponent> componentsPool)
    {
        AnsiConsole.WriteLine();

        foreach (var line in DeploymentChangeSummary.BuildLines(componentsPool))
        {
            AnsiConsole.MarkupLine(line);
        }

        AnsiConsole.WriteLine();
    }

    /// <summary>
    ///     Rejects, before anything is written, what Dataverse does not support in a plugin package.
    /// </summary>
    private void GuardPackageMode(IAssemblyContext localAssembly)
    {
        if (_deploySettings.IsOnPremise)
            throw new NotSupportedException(
                "Plugin packages are not supported on-premise. Set deployment=\"assembly\" for this " +
                "project in xrmFramework.config, or target a Dataverse environment.");

        var workflowNames = localAssembly.Workflows.Select(w => w.FullName).ToList();

        if (workflowNames.Count > 0)
            throw new NotSupportedException(
                "Custom workflow activities cannot be deployed in a plugin package, and this assembly " +
                $"contains {workflowNames.Count}: {string.Join(", ", workflowNames)}. Move them to a " +
                "separate, signed assembly project deployed with deployment=\"assembly\".");
    }

    /// <summary>
    ///     Pushes the package, then waits for Dataverse to unpack it.
    ///     <para>
    ///         Steps that point at a plugin type the new package no longer carries are unregistered
    ///         first: the platform rejects a package update that would strand a step registration.
    ///     </para>
    /// </summary>
    private void PushPackageAndWait(IAssemblyContext localAssembly, PluginPackage localPackage)
    {
        _consoleService.SetStatus("Fetching Registered Package...");

        var registeredPackage = _registrationService.GetPackageByUniqueName(localPackage.UniqueName);

        if (registeredPackage != null)
        {
            _consoleService.SetStatus("Unregistering Obsolete Steps...");
            DeleteStepsOfRemovedPluginTypes(localAssembly);
        }

        _consoleService.SetStatus($"Pushing Package {localPackage.UniqueName}...");
        _assemblyExporter.PushPackage(localPackage, registeredPackage);

        _consoleService.SetStatus("Waiting for Dataverse to Unpack the Package...");
        WaitForPluginTypes(localAssembly);
    }

    /// <summary>
    ///     Deletes the steps and images whose plugin type is about to disappear from the package.
    ///     Everything else is left to the normal diff, which runs once the package is in place.
    /// </summary>
    private void DeleteStepsOfRemovedPluginTypes(IAssemblyContext localAssembly)
    {
        var remoteAssembly =
            _assemblyFactory.CreateFromRemoteAssemblyContext(_registrationService, localAssembly.AssemblyInfo.Name);

        if (remoteAssembly?.AssemblyInfo == null || remoteAssembly.AssemblyInfo.Id == Guid.Empty) return;

        var orphanedComponents = PluginPackageReconciler.FindStepsOfRemovedPluginTypes(
            remoteAssembly.Plugins, LocalPluginTypeNames(localAssembly));

        if (orphanedComponents.Count == 0) return;

        PrintRemovalSummary(orphanedComponents);
        ConfirmRemovalsOrCancel(orphanedComponents);

        _assemblyExporter.DeleteAllComponents(orphanedComponents);
    }

    /// <summary>
    ///     Asks whether to go ahead with the unregistrations a package push requires.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Only reached when something is actually about to be removed, so a deployment that
    ///         adds and updates still runs end to end without a second prompt.
    ///     </para>
    ///     <para>
    ///         Skipped under <c>--noprompt</c>, the same way the connection confirmation is: that
    ///         switch exists for CI, where there is nobody to answer and a blocked prompt would
    ///         hang the agent.
    ///     </para>
    ///     <para>
    ///         Declining cancels the whole deployment rather than just the removals, because the
    ///         package push that follows is what required them - the platform rejects an update
    ///         that would strand those step registrations. Nothing has been written at this point,
    ///         so the environment is left exactly as it was.
    ///     </para>
    /// </remarks>
    /// <exception cref="OperationCanceledException">The removals were declined.</exception>
    private void ConfirmRemovalsOrCancel(IEnumerable<ICrmComponent> components)
    {
        if (!DeploymentChangeSummary.RequiresRemovalConfirmation(components, _deployOptions.NoPrompt)) return;

        // Defaults to no: this unregisters steps that are live in the environment.
        if (!AnsiConsole.Confirm("Unregister these and push the package?", false))
        {
            throw new OperationCanceledException(
                "Deployment cancelled: the unregistrations listed above were declined. " +
                "Nothing has been written to the environment.");
        }
    }

    /// <summary>
    ///     Prints what is about to be unregistered to let the package push through.
    /// </summary>
    /// <remarks>
    ///     These deletions happen before the diff is computed, so the change summary never sees
    ///     them: without this they are the one part of a package deployment that writes - and
    ///     unregisters live steps at that - without saying so.
    /// </remarks>
    private static void PrintRemovalSummary(IEnumerable<ICrmComponent> components)
    {
        var lines = DeploymentChangeSummary.BuildRemovalLines(components);

        if (lines.Count == 0) return;

        AnsiConsole.WriteLine();

        foreach (var line in lines)
        {
            AnsiConsole.MarkupLine(line);
        }

        AnsiConsole.WriteLine();
    }

    /// <summary>
    ///     Polls until every locally inventoried plugin type exists server-side, because the steps
    ///     registered right after are hung off those rows. Packages holding hundreds of plugin types
    ///     take minutes to import, hence a bounded wait rather than an assumption.
    /// </summary>
    private void WaitForPluginTypes(IAssemblyContext localAssembly)
    {
        var expectedTypeNames = LocalPluginTypeNames(localAssembly).ToList();
        var deadline = DateTime.UtcNow + _deployOptions.PackageProcessingTimeout;
        var missing = new List<string>();

        while (true)
        {
            var assembly = _registrationService.GetAssemblyInfoByName(localAssembly.AssemblyInfo.Name);

            if (assembly != null)
            {
                var registeredTypeNames = new HashSet<string>(
                    _registrationService.GetRegisteredPluginTypes(assembly.Id).Select(t => t.TypeName),
                    StringComparer.Ordinal);

                missing = expectedTypeNames.Where(n => !registeredTypeNames.Contains(n)).ToList();

                if (missing.Count == 0) return;
            }

            if (DateTime.UtcNow >= deadline)
                throw new TimeoutException(
                    $"Dataverse did not finish unpacking the plugin package within {_deployOptions.PackageProcessingTimeout.TotalMinutes:0.#} min. " +
                    (assembly == null
                        ? $"The assembly '{localAssembly.AssemblyInfo.Name}' has not appeared yet."
                        : $"Still missing {missing.Count} plugin type(s): {string.Join(", ", missing.Take(5))}.") +
                    " Raise DeployOptions.PackageProcessingTimeout and run the deployment again - it is idempotent.");

            Thread.Sleep(PackagePollInterval);
        }
    }

    /// <summary>
    ///     Full type names the platform is expected to register out of the package: the plugins, plus
    ///     the types backing the custom APIs.
    /// </summary>
    private static IEnumerable<string> LocalPluginTypeNames(IAssemblyContext localAssembly)
        => localAssembly.Plugins.Select(p => p.FullName)
            .Concat(localAssembly.CustomApis.Select(c => c.FullName));

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
