// Copyright (c) Dimsi. All rights reserved.

namespace MsBuildTypeScript;

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
            var generator = new TsGenerator(Log, OutputDirectory, TypePrefix);
            generator.Generate(SourceFiles);
            return !Log.HasLoggedErrors;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, true);
            return false;
        }
    }
}
