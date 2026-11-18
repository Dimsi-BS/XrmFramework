// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using XrmFramework.RemoteDebugger.Common;

#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
#endif

namespace XrmFramework.RemoteDebugger.Client
{
    /// <summary>
    ///     Running a Custom Workflow Activity locally - the one part of the debugger that .NET
    ///     Framework can do and .NET cannot.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Everything else the debugger does is portable: the Azure Relay listener, the JSON wire
    ///         format, the local <c>IServiceProvider</c> and the <c>IPlugin</c> execution path all run
    ///         anywhere. Workflow activities do not, because they are built on Windows Workflow
    ///         Foundation - <c>System.Activities</c>, which has no .NET 5+ equivalent, and
    ///         <c>Microsoft.Xrm.Sdk.Workflow</c>, which Microsoft ships for net462 only. There is no
    ///         <c>WorkflowInvoker</c> to call.
    ///     </para>
    ///     <para>
    ///         Both implementations live in this one file on purpose. The alternative - a
    ///         <c>#if</c> around each of the four call sites in <c>RemoteDebugger</c> and
    ///         <c>PluginTestRunner</c> - spreads the same condition over code that has nothing else to
    ///         say about target frameworks, and hides that <c>RemoteDebugExecutionContext.Arguments</c>
    ///         itself only exists on the full-framework leg (it comes from the
    ///         <c>XrmFramework.Workflows</c> partial, which DeployUtils links only there).
    ///     </para>
    ///     <para>
    ///         The symbol is positive and named after the capability rather than
    ///         <c>!NET8_0_OR_GREATER</c>: it says what is present instead of which runtimes are
    ///         excluded, and the day a stand-in can actually run activities, this file is the only
    ///         one that has to change.
    ///     </para>
    /// </remarks>
    internal static class WorkflowActivitySupport
    {
        /// <summary>Whether this runtime can execute Custom Workflow Activities.</summary>
        internal static bool IsSupported =>
#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
            true;
#else
            false;
#endif

        /// <summary>
        ///     Whether <paramref name="type" /> is a Custom Workflow Activity rather than a plugin.
        /// </summary>
        /// <remarks>
        ///     Asked of the resolved type, not of the context's <c>IsWorkflowContext</c> flag: a
        ///     <c>CodeActivity</c> cannot be run as an <c>IPlugin</c> whatever the context claims, and
        ///     a session recorded before that flag was set correctly still has to replay.
        /// </remarks>
        internal static bool IsWorkflowActivity(Type type)
        {
            if (type == null)
                return false;

#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
            return typeof(CodeActivity).IsAssignableFrom(type);
#else
            // By base type NAME, since the type itself is unreferenceable here. The plugin assembly
            // under debug is still a net4x one, so a workflow activity in it derives from the real
            // System.Activities.CodeActivity - or from a stand-in that carries the same full name.
            // Same classification by full name as PluginInventoryEngine, and for the same reason.
            for (var current = type.BaseType; current != null; current = current.BaseType)
            {
                if (current.FullName == "System.Activities.CodeActivity")
                    return true;
            }

            return false;
#endif
        }

        /// <summary>
        ///     Executes a Custom Workflow Activity, writing its output arguments back into
        ///     <paramref name="context" />.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">
        ///     On .NET, where Windows Workflow Foundation does not exist.
        /// </exception>
        internal static void Run(
            Type activityType, LocalServiceProvider serviceProvider, RemoteDebugExecutionContext context)
        {
#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
            var activity = (CodeActivity)Activator.CreateInstance(activityType);
            var invoker = new WorkflowInvoker(activity);

            AddExtension<IWorkflowContext>(serviceProvider, invoker);
            AddExtension<IOrganizationServiceFactory>(serviceProvider, invoker);
            AddExtension<IServiceEndpointNotificationService>(serviceProvider, invoker);
            AddExtension<ITracingService>(serviceProvider, invoker);

            var inputs = context.Arguments.ToDictionary(k => k.Key, k => k.Value);
            var outputs = invoker.Invoke(inputs);

            context.Arguments.Clear();
            foreach (var output in outputs)
            {
                context.Arguments[output.Key] = output.Value;
            }
#else
            // Actionable rather than apologetic: the activity loaded fine, so the person is one
            // runner away from debugging it, and nothing about their code needs changing.
            throw new PlatformNotSupportedException(
                $"'{activityType?.FullName}' is a Custom Workflow Activity, and those can only be run "
                + "by the .NET Framework build of the remote debugger: they are built on Windows "
                + "Workflow Foundation (System.Activities), which does not exist on .NET, and "
                + "Microsoft.Xrm.Sdk.Workflow ships for net462 only. Run the debugger project on "
                + "net48 to debug it. Plugins and Custom APIs in the same assembly debug fine here.");
#endif
        }

#if XRMFRAMEWORK_WORKFLOW_ACTIVITIES
        private static void AddExtension<TService>(IServiceProvider provider, WorkflowInvoker invoker)
            where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service != null)
                invoker.Extensions.Add(() => (TService)service);
        }
#endif
    }
}
