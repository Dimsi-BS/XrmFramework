// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using CommandLine;
using NUnit.Framework;
using XrmFramework.DeployUtils.CommandOptions;

namespace XrmFramework.DeployUtils.Tests.CommandOptions;

[TestFixture]
public class CommandLineAliasesTests
{
    // ──────────────────────────────────────────────
    //  NormalizeNoPrompt
    // ──────────────────────────────────────────────

    [TestCase("-NoPrompt")]
    [TestCase("-noprompt")]
    [TestCase("-NOPROMPT")]
    [TestCase("--NoPrompt")]
    public void NormalizeNoPrompt_Alias_IsRewrittenToCanonicalOption(string alias)
    {
        var normalized = CommandLineAliases.NormalizeNoPrompt(new[] { alias });

        Assert.AreEqual(new[] { "--noprompt" }, normalized);
    }

    [Test]
    public void NormalizeNoPrompt_DeclaredOptions_AreLeftUntouched()
    {
        var args = new[] { "-n", "--noprompt", "--on-premise" };

        var normalized = CommandLineAliases.NormalizeNoPrompt(args);

        Assert.AreEqual(new[] { "-n", "--noprompt", "--on-premise" }, normalized);
    }

    [Test]
    public void NormalizeNoPrompt_OtherArguments_AreLeftUntouched()
    {
        var args = new[] { "deploy", "plugins", "--project", "NoPrompt" };

        var normalized = CommandLineAliases.NormalizeNoPrompt(args);

        Assert.AreEqual(args, normalized);
    }

    [Test]
    public void NormalizeNoPrompt_AfterEndOfOptionsSeparator_IsLeftUntouched()
    {
        var args = new[] { "-NoPrompt", "--", "-NoPrompt" };

        var normalized = CommandLineAliases.NormalizeNoPrompt(args);

        Assert.AreEqual(new[] { "--noprompt", "--", "-NoPrompt" }, normalized);
    }

    [Test]
    public void NormalizeNoPrompt_NoArguments_ReturnsEmptyArray()
    {
        Assert.AreEqual(Array.Empty<string>(), CommandLineAliases.NormalizeNoPrompt(Array.Empty<string>()));
        Assert.AreEqual(Array.Empty<string>(), CommandLineAliases.NormalizeNoPrompt(null!));
    }

    // ──────────────────────────────────────────────
    //  End to end: what RegistrationHelper does with the args
    // ──────────────────────────────────────────────

    [TestCase("-n")]
    [TestCase("--noprompt")]
    [TestCase("-NoPrompt")]
    public void DeployCommandOptions_EverySpelling_TurnsOnSilentMode(string arg)
    {
        var noPrompt = false;

        using var parser = new Parser(with => with.HelpWriter = null);

        parser.ParseArguments<DeployCommandOptions>(CommandLineAliases.NormalizeNoPrompt(new[] { arg }))
              .WithParsed(opts => noPrompt = opts.NoPrompt)
              .WithNotParsed(_ => { });

        Assert.IsTrue(noPrompt, $"'{arg}' should enable silent mode.");
    }

    [Test]
    public void DeployCommandOptions_NoArgument_KeepsThePrompt()
    {
        var noPrompt = true;

        using var parser = new Parser(with => with.HelpWriter = null);

        parser.ParseArguments<DeployCommandOptions>(CommandLineAliases.NormalizeNoPrompt(Array.Empty<string>()))
              .WithParsed(opts => noPrompt = opts.NoPrompt)
              .WithNotParsed(_ => { });

        Assert.IsFalse(noPrompt);
    }
}
