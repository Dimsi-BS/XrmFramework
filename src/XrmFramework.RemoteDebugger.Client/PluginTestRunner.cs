// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.Client.ConsoleUI;
using XrmFramework.RemoteDebugger.Common;
using XrmFramework.RemoteDebugger.Common.ConsoleUI;

namespace XrmFramework.RemoteDebugger.Client;

/// <summary>
/// Executes a Dynamics 365 plugin from a recorded test session,
/// replaying all calls to the CRM organization service from the recorded responses.
/// No network connection is required.
/// </summary>
/// <remarks>
/// This class is designed to be used by the unit tests automatically generated
/// by <c>XrmFramework.RemoteDebugger.Generator</c>.
/// </remarks>
public static class PluginTestRunner
{
    /// <summary>
    /// Executes the plugin described in the session JSON and returns the modified
    /// execution context (with OutputParameters, SharedVariables, etc. updated).
    /// All calls to the CRM organization service are replayed from the recorded responses.
    /// </summary>
    /// <param name="sessionJson">JSON content of a <see cref="PluginTestSession"/>.</param>
    /// <returns>
    /// The execution context after the plugin has executed.
    /// This context can be compared via Verify to create a test snapshot.
    /// </returns>
    /// <exception cref="ArgumentNullException">If <paramref name="sessionJson"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// If the JSON is invalid, if the plugin type cannot be resolved,
    /// or if the plugin makes more OrgService calls than were recorded.
    /// </exception>
    public static RemoteDebugExecutionContext RunFromJson(string sessionJson)
    {
        if (sessionJson == null) throw new ArgumentNullException(nameof(sessionJson));

        var session = JsonConvert.DeserializeObject<PluginTestSession>(
            sessionJson,
            RemoteDebuggerSettings.JsonSerializerSettings);

        if (session == null)
            throw new InvalidOperationException("Unable to deserialize the plugin test session.");

        return Run(session);
    }

    /// <summary>
    /// Executes the plugin described in the session and returns the modified execution context.
    /// </summary>
    /// <param name="session">The test session to replay.</param>
    /// <returns>The execution context after the plugin has executed.</returns>
    public static RemoteDebugExecutionContext Run(PluginTestSession session)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));

        var callIndex = 0;
        var inputContext = session.InputContext;

        // Inject the original execution date into the context.
        // This triggers InitializeDateTimeProvider() in LocalContext, which substitutes
        // SystemDateTimeProvider with FixedDateTimeProvider(session.ExecutionDate).
        // Result: clock.UtcNow returns the same value as at recording time,
        // making relative date calculations reproducible (e.g. clock.UtcNow.AddDays(30)).
        if (session.ExecutionDate != default)
        {
            inputContext.ExecutionDate = session.ExecutionDate;
        }

        // Configure the service provider with an OrgService that replays the recorded responses
        var serviceProvider = new LocalServiceProvider(inputContext);

        serviceProvider.RequestSent += request =>
        {
            if (callIndex >= session.OrgServiceCalls.Count)
            {
                throw new InvalidOperationException(
                    $"Unexpected OrgService call #{callIndex + 1}. " +
                    $"Only {session.OrgServiceCalls.Count} call(s) were recorded in this session. " +
                    $"The plugin's behavior may have changed since the recording.");
            }

            var recorded = session.OrgServiceCalls[callIndex++];

            // Return the recorded response as a RemoteDebuggerMessage
            return new RemoteDebuggerMessage
            {
                MessageType = RemoteDebuggerMessageType.Response,
                PluginExecutionId = inputContext.Id,
                Content = recorded.ResponseJson
            };
        };

        // Resolve the plugin type (remove Version/PublicKeyToken for portability)
        var pluginType = ResolvePluginType(inputContext.TypeAssemblyQualifiedName);

        if (pluginType == null)
        {
            throw new InvalidOperationException(
                $"Unable to resolve the plugin type '{inputContext.TypeAssemblyQualifiedName}'. " +
                "Make sure the plugin's assembly is referenced by the test project.");
        }

        if (inputContext.IsWorkflowContext)
        {
            RunWorkflowActivity(pluginType, serviceProvider, inputContext);
        }
        else
        {
            RunPlugin(pluginType, serviceProvider, inputContext);
        }

        return inputContext;
    }

    private static Type ResolvePluginType(string assemblyQualifiedName)
    {
        if (string.IsNullOrEmpty(assemblyQualifiedName))
            return null;

        // Remove the Version and PublicKey tokens to allow portability across versions
        var parts = assemblyQualifiedName
            .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
            .Where(p => !p.StartsWith("Version=") && !p.StartsWith("PublicKeyToken=") && !p.StartsWith("Culture="))
            .ToList();

        var typeName = string.Join(", ", parts);
        return Type.GetType(typeName);
    }

    private static void RunPlugin(
        Type pluginType,
        LocalServiceProvider serviceProvider,
        RemoteDebugExecutionContext context)
    {
        IPlugin plugin;

        if (pluginType.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
        {
            plugin = (IPlugin)Activator.CreateInstance(
                pluginType,
                context.UnsecureConfig,
                context.SecureConfig);
        }
        else
        {
            plugin = (IPlugin)Activator.CreateInstance(pluginType);
        }

        plugin.Execute(serviceProvider);
    }

    private static void RunWorkflowActivity(
        Type activityType,
        LocalServiceProvider serviceProvider,
        RemoteDebugExecutionContext context)
    {
        var codeActivity = (CodeActivity)Activator.CreateInstance(activityType);
        var invoker = new WorkflowInvoker(codeActivity);

        AddWorkflowExtension<IWorkflowContext>(serviceProvider, invoker);
        AddWorkflowExtension<IOrganizationServiceFactory>(serviceProvider, invoker);
        AddWorkflowExtension<IServiceEndpointNotificationService>(serviceProvider, invoker);
        AddWorkflowExtension<ITracingService>(serviceProvider, invoker);

        var inputs = context.Arguments.ToDictionary(k => k.Key, k => k.Value);
        var outputs = invoker.Invoke(inputs);

        context.Arguments.Clear();
        foreach (var output in outputs)
        {
            context.Arguments[output.Key] = output.Value;
        }
    }

    private static void AddWorkflowExtension<TService>(IServiceProvider provider, WorkflowInvoker invoker)
        where TService : class
    {
        var service = provider.GetService(typeof(TService));
        if (service != null)
        {
            invoker.Extensions.Add<TService>(() => (TService)service);
        }
    }
}
