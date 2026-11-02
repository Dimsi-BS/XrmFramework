// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

public class GenerateTsFilesTask : Task
{
    [Required]
    public ITaskItem[] SourceFiles { get; set; } = [];

    [Required]
    public string OutputDirectory { get; set; } = string.Empty;

    public string TypePrefix { get; set; } = "type";

    public override bool Execute()
    {
        try
        {
            var generator = new TsGenerator(new TaskLog(Log), OutputDirectory, TypePrefix);

            generator.Generate(SourceFiles.Select(file => file.GetMetadata("FullPath")));

            return !Log.HasLoggedErrors;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, true);
            return false;
        }
    }

    /// <summary>
    /// Hands the generator's reports to MSBuild. This is the whole of the generator's dependency on
    /// MSBuild, and the reason the generator itself has none.
    /// </summary>
    private sealed class TaskLog(TaskLoggingHelper log) : IGeneratorLog
    {
        public void Warning(string message) => log.LogWarning(message);

        public void Error(string message) => log.LogError(message);

        public void Message(string message) => log.LogMessage(MessageImportance.High, message);
    }
}
