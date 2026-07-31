// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// One C# attribute written inside an attribute list, e.g. <c>GeneratedCode("XrmFramework", "2.0")</c>.
    /// </summary>
    internal sealed class CSharpAttributeEntry
    {
        /// <summary>Simple name, unqualified and without the <c>Attribute</c> suffix.</summary>
        public string Name = string.Empty;

        /// <summary>Source text of the entry, arguments included, trimmed.</summary>
        public string Text = string.Empty;
    }

    /// <summary>
    /// One attribute list, i.e. a single <c>[ ... ]</c> group, which may declare several attributes.
    /// </summary>
    internal sealed class CSharpAttributeList
    {
        /// <summary>Start of the trivia preceding the <c>[</c>: the span to delete to remove the list entirely.</summary>
        public int FullStart;

        /// <summary>Position of the <c>[</c>.</summary>
        public int Start;

        /// <summary>Position just after the <c>]</c>.</summary>
        public int End;

        public IList<CSharpAttributeEntry> Entries = new List<CSharpAttributeEntry>();
    }

    /// <summary>
    /// One member of a C# scope (file, namespace or type body), located by its spans in the source text.
    /// </summary>
    /// <remarks>
    /// The members returned by <see cref="CSharpMemberReader"/> tile the analyzed range: the
    /// <see cref="FullStart"/> of a member is the <see cref="End"/> of the previous one. Deleting
    /// <c>[FullStart, End)</c> therefore removes the member along with the trivia that precedes it
    /// (blank line, XML doc comment) without touching the neighbouring members.
    /// </remarks>
    internal sealed class CSharpMember
    {
        /// <summary>Start of the leading trivia (end of the previous member).</summary>
        public int FullStart;

        /// <summary>Position just after the terminator (<c>;</c> or closing <c>}</c>).</summary>
        public int End;

        /// <summary>Position of the first character after the attribute lists.</summary>
        public int DeclarationStart;

        /// <summary>Position just after the opening <c>{</c> of the body, or <c>-1</c> if there is none.</summary>
        public int BodyStart = -1;

        /// <summary>Position of the closing <c>}</c> of the body, or <c>-1</c> if there is none.</summary>
        public int BodyEnd = -1;

        /// <summary>
        /// <c>class</c>, <c>struct</c>, <c>interface</c>, <c>enum</c>, <c>record</c>, <c>namespace</c>,
        /// <c>using</c>; empty for a field, property or method.
        /// </summary>
        public string Keyword = string.Empty;

        /// <summary>Position of the <see cref="Keyword"/> token, or <c>-1</c>.</summary>
        public int KeywordStart = -1;

        /// <summary>Declared name (dotted for a namespace).</summary>
        public string Name = string.Empty;

        /// <summary>Span of <see cref="Name"/> in the source text.</summary>
        public int NameStart = -1;

        /// <summary>Span of <see cref="Name"/> in the source text.</summary>
        public int NameEnd = -1;

        public IList<CSharpAttributeList> Attributes = new List<CSharpAttributeList>();

        /// <summary>Every identifier of the declaration header, in order (modifiers included).</summary>
        public IList<string> HeaderTokens = new List<string>();

        public bool HasModifier(string modifier)
        {
            foreach (var token in HeaderTokens)
                if (string.Equals(token, modifier, StringComparison.Ordinal))
                    return true;

            return false;
        }
    }

    /// <summary>
    /// Minimal lexical reader that splits a C# scope into members without compiling anything.
    /// </summary>
    /// <remarks>
    /// It handles what is needed to rewrite a <c>*Definition.cs</c> file: comments, string / verbatim /
    /// interpolated / character literals, attribute lists, and nesting of <c>{}</c>, <c>()</c> and <c>[]</c>.
    /// It is deliberately partial — it is a *scanner*, not a parser: any construct it cannot bracket
    /// reliably makes it return <c>null</c>, and the caller then leaves the file untouched. Preferring an
    /// unmodified file over a wrong rewrite is the whole point, since the migration deletes source files.
    /// </remarks>
    internal static class CSharpMemberReader
    {
        private static readonly HashSet<string> TypeKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "class", "struct", "interface", "enum", "record", "namespace"
        };

        /// <summary>
        /// Splits <c>[start, end)</c> — a file, a namespace body or a type body — into members.
        /// </summary>
        /// <returns>The members found, or <c>null</c> if the range could not be read reliably.</returns>
        public static IList<CSharpMember> ReadMembers(string source, int start, int end)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (start < 0 || end > source.Length || start > end)
                throw new ArgumentOutOfRangeException(nameof(start));

            var members = new List<CSharpMember>();
            var pos = start;

            while (true)
            {
                var fullStart = pos;
                pos = SkipTrivia(source, pos, end);
                if (pos >= end)
                    return members;

                var member = new CSharpMember { FullStart = fullStart };

                // 1. Attribute lists, which may be several in a row.
                while (pos < end && source[pos] == '[')
                {
                    var list = ReadAttributeList(source, pos, end);
                    if (list == null)
                        return null;

                    list.FullStart = member.Attributes.Count == 0
                        ? fullStart
                        : member.Attributes[member.Attributes.Count - 1].End;

                    member.Attributes.Add(list);
                    pos = SkipTrivia(source, list.End, end);
                }

                if (pos >= end)
                    return null;

                member.DeclarationStart = pos;

                // 2. Declaration, up to its terminator.
                if (!ReadDeclaration(source, pos, end, member))
                    return null;

                members.Add(member);
                pos = member.End;
            }
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Declaration
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Scans the declaration starting at <paramref name="start"/> and fills in the spans of
        /// <paramref name="member"/>. Returns false if the declaration is not correctly bracketed.
        /// </summary>
        private static bool ReadDeclaration(string source, int start, int end, CSharpMember member)
        {
            var braces = 0;
            var parens = 0;
            var brackets = 0;
            var headerEnd = -1;
            var terminator = -1;
            var i = start;

            while (i < end && terminator < 0)
            {
                var skipped = SkipTriviaOrLiteral(source, i, end);
                if (skipped != i)
                {
                    i = skipped;
                    continue;
                }

                var c = source[i];
                var atTopLevel = braces == 0 && parens == 0 && brackets == 0;

                switch (c)
                {
                    case '{':
                        if (atTopLevel && headerEnd < 0) headerEnd = i;
                        if (atTopLevel && member.BodyStart < 0) member.BodyStart = i + 1;
                        braces++;
                        i++;
                        break;

                    case '}':
                        braces--;
                        if (braces < 0) return false;
                        if (braces == 0)
                        {
                            if (member.BodyEnd < 0) member.BodyEnd = i;
                            i++;

                            // A body may be followed by an initializer or a semicolon:
                            //   public int X { get; set; } = 5;   class C { };
                            var next = SkipTrivia(source, i, end);

                            if (next < end && source[next] == ';')
                                terminator = next + 1;
                            else if (next < end && source[next] == '=')
                                i = next + 1;      // keep scanning, the ';' is still ahead
                            else
                                terminator = i;
                        }
                        else
                        {
                            i++;
                        }

                        break;

                    case '(':
                        if (atTopLevel && headerEnd < 0) headerEnd = i;
                        parens++;
                        i++;
                        break;

                    case ')':
                        parens--;
                        if (parens < 0) return false;
                        i++;
                        break;

                    case '[':
                        brackets++;
                        i++;
                        break;

                    case ']':
                        brackets--;
                        if (brackets < 0) return false;
                        i++;
                        break;

                    case '=':
                        if (atTopLevel && headerEnd < 0) headerEnd = i;
                        i++;
                        break;

                    case ';':
                        if (atTopLevel)
                        {
                            if (headerEnd < 0) headerEnd = i;
                            terminator = i + 1;
                        }
                        else
                        {
                            i++;
                        }

                        break;

                    default:
                        i++;
                        break;
                }
            }

            if (terminator < 0)
                return false;

            member.End = terminator;
            ReadHeader(source, start, headerEnd < 0 ? member.End : headerEnd, member);
            return true;
        }

        /// <summary>
        /// Reads the identifiers of the declaration header and derives the keyword and the declared name.
        /// </summary>
        private static void ReadHeader(string source, int start, int headerEnd, CSharpMember member)
        {
            var tokens = ReadIdentifiers(source, start, headerEnd);
            foreach (var token in tokens)
                member.HeaderTokens.Add(token.Text);

            if (tokens.Count == 0)
                return;

            // "using X;" / "using static X;" / "using X = Y;"
            if (string.Equals(tokens[0].Text, "using", StringComparison.Ordinal))
            {
                member.Keyword = "using";
                member.KeywordStart = tokens[0].Start;
                return;
            }

            // The last type keyword wins, so that "record class Foo" resolves to "class".
            var keywordIndex = -1;
            for (var k = 0; k < tokens.Count; k++)
                if (TypeKeywords.Contains(tokens[k].Text))
                    keywordIndex = k;

            if (keywordIndex >= 0 && keywordIndex + 1 < tokens.Count)
            {
                member.Keyword = tokens[keywordIndex].Text;
                member.KeywordStart = tokens[keywordIndex].Start;

                var nameToken = tokens[keywordIndex + 1];
                member.NameStart = nameToken.Start;

                if (string.Equals(member.Keyword, "namespace", StringComparison.Ordinal))
                {
                    // A namespace name is dotted: it runs to the end of the header.
                    var nameEnd = headerEnd;
                    while (nameEnd > nameToken.Start && char.IsWhiteSpace(source[nameEnd - 1]))
                        nameEnd--;

                    member.NameEnd = nameEnd;
                    member.Name = source.Substring(nameToken.Start, nameEnd - nameToken.Start);
                }
                else
                {
                    member.NameEnd = nameToken.End;
                    member.Name = nameToken.Text;
                }

                return;
            }

            // Field, constant, property or method: the name is the last identifier of the header.
            var last = tokens[tokens.Count - 1];
            member.Name = last.Text;
            member.NameStart = last.Start;
            member.NameEnd = last.End;
        }

        private struct Token
        {
            public string Text;
            public int Start;
            public int End;
        }

        private static IList<Token> ReadIdentifiers(string source, int start, int end)
        {
            var tokens = new List<Token>();
            var i = start;

            while (i < end)
            {
                var skipped = SkipTriviaOrLiteral(source, i, end);
                if (skipped != i)
                {
                    i = skipped;
                    continue;
                }

                var c = source[i];
                if (c == '_' || c == '@' || char.IsLetter(c))
                {
                    var tokenStart = i;
                    i++;
                    while (i < end && (source[i] == '_' || char.IsLetterOrDigit(source[i])))
                        i++;

                    tokens.Add(new Token
                    {
                        Text = source.Substring(tokenStart, i - tokenStart).TrimStart('@'),
                        Start = tokenStart,
                        End = i
                    });
                    continue;
                }

                i++;
            }

            return tokens;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Attribute lists
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the attribute list starting at <paramref name="start"/> (which points at the <c>[</c>).
        /// </summary>
        private static CSharpAttributeList ReadAttributeList(string source, int start, int end)
        {
            var list = new CSharpAttributeList { Start = start };
            var entries = new List<CSharpAttributeEntry>();

            var brackets = 1;
            var parens = 0;
            var i = start + 1;
            var entryStart = i;

            while (i < end)
            {
                var skipped = SkipTriviaOrLiteral(source, i, end);
                if (skipped != i)
                {
                    i = skipped;
                    continue;
                }

                var c = source[i];
                switch (c)
                {
                    case '[':
                        brackets++;
                        i++;
                        break;

                    case ']':
                        brackets--;
                        if (brackets == 0)
                        {
                            AddEntry(source, entries, entryStart, i);
                            list.End = i + 1;
                            list.Entries = entries;
                            return list;
                        }

                        i++;
                        break;

                    case '(':
                        parens++;
                        i++;
                        break;

                    case ')':
                        parens--;
                        if (parens < 0) return null;
                        i++;
                        break;

                    case ',':
                        if (brackets == 1 && parens == 0)
                        {
                            AddEntry(source, entries, entryStart, i);
                            entryStart = i + 1;
                        }

                        i++;
                        break;

                    default:
                        i++;
                        break;
                }
            }

            return null;
        }

        private static void AddEntry(string source, ICollection<CSharpAttributeEntry> entries, int start, int end)
        {
            var text = source.Substring(start, end - start).Trim();
            if (text.Length == 0)
                return;

            // Name = what precedes the arguments, without its target specifier ("assembly:"),
            // without its qualification ("System.CodeDom.Compiler.") and without the "Attribute" suffix.
            var name = text;

            var paren = name.IndexOf('(');
            if (paren >= 0) name = name.Substring(0, paren);

            var colon = name.LastIndexOf(':');
            if (colon >= 0) name = name.Substring(colon + 1);

            name = name.Trim();

            var dot = name.LastIndexOf('.');
            if (dot >= 0) name = name.Substring(dot + 1);

            if (name.Length > "Attribute".Length && name.EndsWith("Attribute", StringComparison.Ordinal))
                name = name.Substring(0, name.Length - "Attribute".Length);

            entries.Add(new CSharpAttributeEntry { Name = name, Text = text });
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Trivia and literals
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>Skips whitespace and comments; returns <paramref name="i"/> if there is nothing to skip.</summary>
        private static int SkipTrivia(string source, int i, int end)
        {
            while (i < end)
            {
                var c = source[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (c == '/' && i + 1 < end)
                {
                    if (source[i + 1] == '/')
                    {
                        i += 2;
                        while (i < end && source[i] != '\n') i++;
                        continue;
                    }

                    if (source[i + 1] == '*')
                    {
                        i += 2;
                        while (i + 1 < end && !(source[i] == '*' && source[i + 1] == '/')) i++;
                        i = Math.Min(i + 2, end);
                        continue;
                    }
                }

                break;
            }

            return i;
        }

        /// <summary>
        /// Skips trivia or, failing that, a complete literal (string, verbatim, interpolated, character).
        /// Returns <paramref name="i"/> if the current character starts neither.
        /// </summary>
        private static int SkipTriviaOrLiteral(string source, int i, int end)
        {
            var afterTrivia = SkipTrivia(source, i, end);
            if (afterTrivia != i)
                return afterTrivia;

            if (i >= end)
                return i;

            var c = source[i];

            if (c == '"')
                return SkipQuoted(source, i, end, verbatim: false);

            if (c == '\'')
                return SkipQuoted(source, i, end, verbatim: false, quote: '\'');

            // @"...", $"...", $@"...", @$"..."
            if (c == '@' || c == '$')
            {
                var j = i;
                var verbatim = false;

                while (j < end && (source[j] == '@' || source[j] == '$'))
                {
                    if (source[j] == '@') verbatim = true;
                    j++;
                }

                if (j < end && source[j] == '"')
                    return SkipQuoted(source, j, end, verbatim);
            }

            return i;
        }

        /// <summary>
        /// Skips a quoted literal starting at <paramref name="i"/> (which points at the opening quote).
        /// </summary>
        private static int SkipQuoted(string source, int i, int end, bool verbatim, char quote = '"')
        {
            i++;

            while (i < end)
            {
                var c = source[i];

                if (verbatim)
                {
                    if (c == quote)
                    {
                        if (i + 1 < end && source[i + 1] == quote)
                        {
                            i += 2;
                            continue;
                        }

                        return i + 1;
                    }

                    i++;
                    continue;
                }

                if (c == '\\')
                {
                    i += 2;
                    continue;
                }

                if (c == quote)
                    return i + 1;

                // An unterminated non-verbatim literal cannot span a line: bail out here rather
                // than swallowing the rest of the file.
                if (c == '\n')
                    return i;

                i++;
            }

            return i;
        }
    }
}
