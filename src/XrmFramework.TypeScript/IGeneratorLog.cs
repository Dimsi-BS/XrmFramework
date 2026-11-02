// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

/// <summary>
/// Where the generator reports what it could not do.
/// </summary>
/// <remarks>
/// The generator takes this rather than MSBuild's <c>TaskLoggingHelper</c>, and file paths rather
/// than <c>ITaskItem</c>s, so that it can run — and be tested — outside MSBuild.
/// <c>Microsoft.Build.Utilities.Core</c> ships reference-only assemblies, so a test host cannot
/// load them at all: a generator reachable only through the task would be a generator no test can
/// reach. <see cref="GenerateTsFilesTask" /> is the thin adapter that bridges the two.
/// </remarks>
public interface IGeneratorLog
{
    /// <summary>Something the build should know about but which does not fail it.</summary>
    void Warning(string message);

    /// <summary>Something that makes the generated output wrong; the build fails.</summary>
    void Error(string message);

    /// <summary>Progress, shown only at a high enough verbosity.</summary>
    void Message(string message);
}
