// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#nullable enable

using System;
using System.Text;

namespace XrmFramework.Analyzers.Generators.Mapping;

/// <summary>
/// Minimal indented string builder, local so the analyzer package carries no extra dependency.
/// </summary>
internal sealed class CodeWriter
{
    private readonly StringBuilder _sb     = new();
    private int                    _indent;
    private bool                   _pendingIndent = true;

    public void Line(string text = "")
    {
        if (text.Length > 0) DoIndent();
        _sb.AppendLine(text);
        _pendingIndent = true;
    }

    public void Indent()  => _indent++;
    public void Dedent()  => _indent = Math.Max(0, _indent - 1);

    public void OpenBrace()  { Line("{"); Indent(); }
    public void CloseBrace() { Dedent(); Line("}"); }

    private void DoIndent()
    {
        if (_pendingIndent && _indent > 0)
            _sb.Append(new string(' ', _indent * 4));
        _pendingIndent = false;
    }

    public override string ToString() => _sb.ToString();
}
