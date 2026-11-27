// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

// Shapes for WorkflowActivitySupportTests, declared the way Stubs.cs declares the XrmFramework ones:
// what matters to the code under test is the full NAME of a base type, not its members.
//
// Safe to declare System.Activities here because this project is net8.0, where that namespace does
// not exist: the real assembly is .NET Framework only, which is the whole reason the debugger's .NET
// leg has to recognise an activity by name in the first place.

namespace System.Activities
{
    /// <summary>
    ///     Stands for <c>System.Activities.CodeActivity</c>, by full name only.
    /// </summary>
    public abstract class CodeActivity
    {
    }
}

namespace NotAnActivity
{
    /// <summary>
    ///     Same simple name, different namespace. Here so that recognition is pinned to the full
    ///     name: matching on <c>Name</c> alone would call this an activity.
    /// </summary>
    public abstract class CodeActivity
    {
    }
}
