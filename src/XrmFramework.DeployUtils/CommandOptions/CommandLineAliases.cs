// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.CommandOptions;

/// <summary>
/// Rewrites the command-line aliases that the parsers cannot declare themselves.
/// </summary>
/// <remarks>
/// Backward compatibility: deployment scripts and CI/CD pipelines already in place call the
/// deployment with <c>-NoPrompt</c>, the spelling a PowerShell script reaches for, and those calls
/// have to keep silencing the prompt. Neither parser can accept it as declared: CommandLineParser
/// and Spectre.Console.Cli both read a single dash as a group of one-letter switches
/// (<c>-N -o -P ...</c>), and Spectre rejects a short option name longer than one character at
/// startup — so the token is translated before parsing rather than declared, next to <c>-n</c>
/// and <c>--noprompt</c>.
/// </remarks>
public static class CommandLineAliases
{
    private const string NoPromptOption = "--noprompt";

    /// <summary>
    /// Replaces every <c>-NoPrompt</c> / <c>--NoPrompt</c> token (any casing) with the
    /// canonical <c>--noprompt</c> option, so that the calls written against the older spelling
    /// keep working. Tokens placed after the <c>--</c> separator are operands, not options,
    /// and are left untouched.
    /// </summary>
    /// <param name="args">Arguments as received by the entry point.</param>
    /// <returns>The arguments to hand over to the parser.</returns>
    public static string[] NormalizeNoPrompt(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            return args ?? Array.Empty<string>();
        }

        var normalized = new List<string>(args.Length);
        var endOfOptions = false;

        foreach (var arg in args)
        {
            if (endOfOptions)
            {
                normalized.Add(arg);
                continue;
            }

            if (arg == "--")
            {
                endOfOptions = true;
                normalized.Add(arg);
                continue;
            }

            normalized.Add(IsNoPromptAlias(arg) ? NoPromptOption : arg);
        }

        return normalized.ToArray();
    }

    private static bool IsNoPromptAlias(string arg)
        => string.Equals(arg, "-noprompt", StringComparison.OrdinalIgnoreCase)
        || string.Equals(arg, NoPromptOption, StringComparison.OrdinalIgnoreCase);
}
