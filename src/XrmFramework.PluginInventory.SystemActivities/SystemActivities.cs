// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// The whole point of this file is to declare types in a namespace owned by the .NET Framework, so
// that a net462/net48 assembly referencing System.Activities, Version=4.0.0.0 can be loaded by a
// .NET-modern host. See the project file for why this is safe and where the surface comes from.
//
// Shapes (abstract/sealed, base types, accessor visibility, exact signatures) are taken from the
// real reference assembly shipped by Microsoft.NETFramework.ReferenceAssemblies, not from memory.
// SystemActivitiesShimTests enforces that they stay in step with it.
namespace System.Activities
{
    /// <summary>Stand-in for <c>System.Activities.Activity</c>.</summary>
    public abstract class Activity
    {
        /// <remarks>
        ///     Real WF resolves <see cref="DisplayName" /> to the activity's short type name when it
        ///     was never set. Reproducing that here is what makes a Custom Workflow Activity that
        ///     never calls <c>SetDisplayName</c> produce the SAME manifest under this host as under
        ///     .NET Framework.
        /// </remarks>
        protected Activity() => DisplayName = GetType().Name;

        /// <summary>
        ///     Set by <c>XrmFramework.Workflow.CustomWorkflowActivity.SetDisplayName</c>, and the one
        ///     member of this assembly the inventory actually reads.
        /// </summary>
        public string DisplayName { get; set; }

        protected virtual void CacheMetadata(ActivityMetadata metadata)
        {
        }
    }

    /// <summary>Stand-in for <c>System.Activities.ActivityWithResult</c>.</summary>
    public abstract class ActivityWithResult : Activity
    {
    }

    /// <summary>Stand-in for <c>System.Activities.Activity&lt;TResult&gt;</c>.</summary>
    public abstract class Activity<TResult> : ActivityWithResult
    {
    }

    /// <summary>Stand-in for <c>System.Activities.CodeActivity</c>.</summary>
    /// <remarks>
    ///     Derives from <see cref="Activity" />, not <see cref="ActivityWithResult" /> - matching the
    ///     real hierarchy. <c>Execute</c> must stay <c>protected abstract void</c> taking a
    ///     <see cref="CodeActivityContext" />: a derived activity overriding a method this class does
    ///     not declare with that exact signature fails to load with a <c>TypeLoadException</c>.
    /// </remarks>
    public abstract class CodeActivity : Activity
    {
        protected abstract void Execute(CodeActivityContext context);

        protected virtual void CacheMetadata(CodeActivityMetadata metadata)
        {
        }
    }

    /// <summary>Stand-in for <c>System.Activities.CodeActivity&lt;TResult&gt;</c>.</summary>
    public abstract class CodeActivity<TResult> : Activity<TResult>
    {
        protected abstract TResult Execute(CodeActivityContext context);

        protected virtual void CacheMetadata(CodeActivityMetadata metadata)
        {
        }
    }

    /// <summary>Stand-in for <c>System.Activities.ActivityContext</c>.</summary>
    public class ActivityContext
    {
        /// <summary>
        ///     How <c>XrmFramework.Workflow.LocalContext</c> reaches the Dataverse services
        ///     (<c>IWorkflowContext</c>, <c>ITracingService</c>, <c>IOrganizationServiceFactory</c>).
        /// </summary>
        /// <remarks>
        ///     Only ever called from a running activity, so an inventory never reaches it - which is
        ///     exactly why it was missing until the shim-coverage test read the metadata and said so.
        ///     The <c>class</c> constraint is the real one's.
        /// </remarks>
        public T GetExtension<T>() where T : class => throw Argument.Unsupported();
    }

    /// <summary>Stand-in for <c>System.Activities.CodeActivityContext</c>.</summary>
    public class CodeActivityContext : ActivityContext
    {
    }

    /// <summary>Stand-in for <c>System.Activities.ArgumentDirection</c>.</summary>
    public enum ArgumentDirection
    {
        In = 0,
        Out = 1,
        InOut = 2,
    }

    /// <summary>Stand-in for <c>System.Activities.ActivityMetadata</c>.</summary>
    /// <remarks>
    ///     A STRUCT in the real assembly, and that has to be reproduced exactly: a signature blob
    ///     spells a value type and a class with different bytes, and overrides are matched on the raw
    ///     blob while the runtime lays a type out. Declaring either metadata type as a class makes an
    ///     activity that overrides <c>CacheMetadata</c> fail to load, with a
    ///     <c>TypeLoadException</c> naming the USER's type and nothing pointing back here.
    /// </remarks>
    public struct ActivityMetadata
    {
    }

    /// <summary>Stand-in for <c>System.Activities.CodeActivityMetadata</c>.</summary>
    /// <remarks>A struct, for the same reason as <see cref="ActivityMetadata" />.</remarks>
    public struct CodeActivityMetadata
    {
    }

    /// <summary>Stand-in for <c>System.Activities.Argument</c>.</summary>
    public abstract class Argument
    {
        /// <remarks>Public getter only, as in the real assembly.</remarks>
        public ArgumentDirection Direction => throw Unsupported();

        /// <remarks>Public getter only, as in the real assembly.</remarks>
        public Type ArgumentType => throw Unsupported();

        /// <summary>
        ///     Referenced by <c>CustomWorkflowActivity.DumpInputArguments</c>, which only runs inside
        ///     <c>Execute</c> - never during an inventory. Declared so the member reference resolves.
        /// </summary>
        public object Get(ActivityContext context) => throw Unsupported();

        public void Set(ActivityContext context, object value) => throw Unsupported();

        internal static NotSupportedException Unsupported() => new NotSupportedException(
            "System.Activities here is a stand-in used by the XrmFramework plugin inventory to load " +
            "a .NET Framework plugin assembly. It describes workflow activities; it cannot run them.");
    }

    /// <summary>Stand-in for <c>System.Activities.InArgument</c>.</summary>
    public abstract class InArgument : Argument
    {
    }

    /// <summary>Stand-in for <c>System.Activities.OutArgument</c>.</summary>
    public abstract class OutArgument : Argument
    {
    }

    /// <summary>Stand-in for <c>System.Activities.InOutArgument</c>.</summary>
    public abstract class InOutArgument : Argument
    {
    }

    /// <summary>Stand-in for <c>System.Activities.InArgument&lt;T&gt;</c>.</summary>
    /// <remarks>
    ///     Sealed, like the real one. The type matters even when nothing reads it: an activity's
    ///     <c>public InArgument&lt;string&gt; Foo { get; set; }</c> has a backing field whose type the
    ///     runtime resolves when it lays the activity's type out.
    /// </remarks>
    public sealed class InArgument<T> : InArgument
    {
        public InArgument()
        {
        }

        public InArgument(T constValue)
        {
        }

        public new T Get(ActivityContext context) => throw Unsupported();

        public void Set(ActivityContext context, T value) => throw Unsupported();
    }

    /// <summary>Stand-in for <c>System.Activities.OutArgument&lt;T&gt;</c>.</summary>
    public sealed class OutArgument<T> : OutArgument
    {
        public OutArgument()
        {
        }

        public new T Get(ActivityContext context) => throw Unsupported();

        public void Set(ActivityContext context, T value) => throw Unsupported();
    }

    /// <summary>Stand-in for <c>System.Activities.InOutArgument&lt;T&gt;</c>.</summary>
    public sealed class InOutArgument<T> : InOutArgument
    {
        public InOutArgument()
        {
        }

        public InOutArgument(T constValue)
        {
        }

        public new T Get(ActivityContext context) => throw Unsupported();

        public void Set(ActivityContext context, T value) => throw Unsupported();
    }

    /// <summary>Stand-in for <c>System.Activities.RequiredArgumentAttribute</c>.</summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RequiredArgumentAttribute : Attribute
    {
    }
}
