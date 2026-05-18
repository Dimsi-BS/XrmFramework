// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore.Internal;

namespace XrmFramework.DeployUtils.Tests.Internal;

[TestFixture]
public class IndentedStringBuilderTests
{
    // ──────────────────────────────────────────────
    //  Append
    // ──────────────────────────────────────────────

    [Test]
    public void Append_NoIndent_AppendsTextDirectly()
    {
        var sb = new IndentedStringBuilder();
        sb.Append("hello");

        Assert.AreEqual("hello", sb.ToString());
    }

    [Test]
    public void Append_WithIndent_PrependsFourSpaces()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.Append("hello");

        Assert.AreEqual("    hello", sb.ToString());
    }

    [Test]
    public void Append_MultipleCallsSameLine_NoDuplicateIndent()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.Append("foo").Append("bar");

        // Indent only applied once at start of line
        Assert.AreEqual("    foobar", sb.ToString());
    }

    // ──────────────────────────────────────────────
    //  AppendLine
    // ──────────────────────────────────────────────

    [Test]
    public void AppendLine_EmptyLine_AddsNewline()
    {
        var sb = new IndentedStringBuilder();
        sb.AppendLine();

        Assert.AreEqual(Environment.NewLine, sb.ToString());
    }

    [Test]
    public void AppendLine_WithText_AppendsTextAndNewline()
    {
        var sb = new IndentedStringBuilder();
        sb.AppendLine("line1");

        Assert.AreEqual("line1" + Environment.NewLine, sb.ToString());
    }

    [Test]
    public void AppendLine_WithIndent_PrependsFourSpaces()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.AppendLine("text");

        Assert.AreEqual("    text" + Environment.NewLine, sb.ToString());
    }

    [Test]
    public void AppendLine_TwoLines_BothIndented()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.AppendLine("a");
        sb.AppendLine("b");

        var expected = "    a" + Environment.NewLine + "    b" + Environment.NewLine;
        Assert.AreEqual(expected, sb.ToString());
    }

    // ──────────────────────────────────────────────
    //  Indent levels
    // ──────────────────────────────────────────────

    [Test]
    public void IncrementIndent_TwiceThenAppend_EightSpaces()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.IncrementIndent();
        sb.Append("x");

        Assert.AreEqual("        x", sb.ToString());
    }

    [Test]
    public void DecrementIndent_BelowZero_NoNegativeIndent()
    {
        var sb = new IndentedStringBuilder();
        sb.DecrementIndent(); // should be clamped at 0
        sb.Append("x");

        Assert.AreEqual("x", sb.ToString());
    }

    [Test]
    public void DecrementIndent_AfterIncrement_ResetsToNoIndent()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.DecrementIndent();
        sb.Append("x");

        Assert.AreEqual("x", sb.ToString());
    }

    // ──────────────────────────────────────────────
    //  Indent() disposable scope
    // ──────────────────────────────────────────────

    [Test]
    public void Indent_UsingSyntax_IncreasesAndThenRestoresIndent()
    {
        var sb = new IndentedStringBuilder();
        sb.AppendLine("outer");

        using (sb.Indent())
        {
            sb.AppendLine("inner");
        }

        sb.AppendLine("outer-again");

        var expected =
            "outer" + Environment.NewLine +
            "    inner" + Environment.NewLine +
            "outer-again" + Environment.NewLine;

        Assert.AreEqual(expected, sb.ToString());
    }

    [Test]
    public void Indent_NestedScopes_CorrectIndentAtEachLevel()
    {
        var sb = new IndentedStringBuilder();

        using (sb.Indent())
        using (sb.Indent())
        {
            sb.Append("deep");
        }

        Assert.AreEqual("        deep", sb.ToString());
    }

    // ──────────────────────────────────────────────
    //  Clear
    // ──────────────────────────────────────────────

    [Test]
    public void Clear_ResetsContent()
    {
        var sb = new IndentedStringBuilder();
        sb.Append("hello");
        sb.Clear();

        Assert.AreEqual(string.Empty, sb.ToString());
    }

    [Test]
    public void Clear_LengthBecomesZero()
    {
        var sb = new IndentedStringBuilder();
        sb.Append("hello");
        sb.Clear();

        Assert.AreEqual(0, sb.Length);
    }

    // ──────────────────────────────────────────────
    //  Length
    // ──────────────────────────────────────────────

    [Test]
    public void Length_AfterAppend_ReflectsContent()
    {
        var sb = new IndentedStringBuilder();
        sb.Append("abc");

        Assert.AreEqual(3, sb.Length);
    }

    // ──────────────────────────────────────────────
    //  Copy constructor
    // ──────────────────────────────────────────────

    [Test]
    public void CopyConstructor_InheritsCurrentIndentLevel()
    {
        var original = new IndentedStringBuilder();
        original.IncrementIndent();

        var copy = new IndentedStringBuilder(original);
        copy.Append("x");

        // The copy starts with the same indent level (1 × 4 = 4 spaces)
        Assert.AreEqual("    x", copy.ToString());
    }

    // ──────────────────────────────────────────────
    //  AppendLines
    // ──────────────────────────────────────────────

    [Test]
    public void AppendLines_MultilineString_EachLineIndented()
    {
        var sb = new IndentedStringBuilder();
        sb.IncrementIndent();
        sb.AppendLines("line1\nline2");

        var result = sb.ToString();
        Assert.IsTrue(result.Contains("    line1"));
        Assert.IsTrue(result.Contains("    line2"));
    }
}
