// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// What must be done with a <c>*Definition.cs</c> file once rewritten.
    /// </summary>
    public enum DefinitionRewriteOutcome
    {
        /// <summary>The file could not be read reliably: it must be left untouched.</summary>
        Skipped,

        /// <summary>Nothing survives the migration: the file must be deleted.</summary>
        Delete,

        /// <summary>Members remain: the file must be written as <c>*Definition.partial.cs</c>.</summary>
        Rewrite
    }

    /// <summary>
    /// Result of rewriting a <c>*Definition.cs</c> file.
    /// </summary>
    public sealed class DefinitionRewriteResult
    {
        public DefinitionRewriteOutcome Outcome { get; set; }

        /// <summary>New content of the file. Only meaningful for <see cref="DefinitionRewriteOutcome.Rewrite"/>.</summary>
        public string NewText { get; set; }

        /// <summary>Reason the file was skipped. Only meaningful for <see cref="DefinitionRewriteOutcome.Skipped"/>.</summary>
        public string Reason { get; set; }

        /// <summary>Members removed from the Definition class, for reporting.</summary>
        public IList<string> RemovedMembers { get; } = new List<string>();

        /// <summary>Namespace-level option set enums removed because the generator re-emits them.</summary>
        public IList<string> RemovedEnums { get; } = new List<string>();

        /// <summary>Members of the Definition class that survived the migration.</summary>
        public IList<string> KeptMembers { get; } = new List<string>();
    }

    /// <summary>
    /// Strips from a <c>*Definition.cs</c> written by XrmFramework 2.* everything that version 3.1
    /// generates from the <c>.table</c> file, and reports what is left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In 2.*, the DefinitionManager wrote a physical <c>*Definition.cs</c> next to the <c>.table</c>.
    /// From 3.1 on, <c>TableSourceFileGenerator</c> emits that class at compile time from the
    /// <c>.table</c> alone, so keeping the file produces duplicate members and duplicate types.
    /// </para>
    /// <para>
    /// Removed: the <c>EntityName</c> and <c>EntityCollectionName</c> constants, the nested
    /// <c>Columns</c>, <c>AlternateKeyNames</c>, <c>ManyToManyRelationships</c>,
    /// <c>ManyToOneRelationships</c> and <c>OneToManyRelationships</c> classes with their attributes,
    /// and the namespace-level option set enums the generator re-emits.
    /// </para>
    /// <para>
    /// Everything the project added by hand is kept. If anything survives, the class is realigned on
    /// what the generator emits — <c>partial</c> modifier, <c>XrmFramework</c> namespace, no duplicate
    /// <c>[GeneratedCode]</c> / <c>[EntityDefinition]</c> / <c>[ExcludeFromCodeCoverage]</c> — so that
    /// it merges with the generated part instead of colliding with it.
    /// </para>
    /// <para>
    /// <see cref="RewriteOptionSets"/> covers the other file 2.* wrote, <c>OptionSetDefinitions.cs</c>,
    /// which holds enums and no Definition class.
    /// </para>
    /// </remarks>
    public static class DefinitionSourceRewriter
    {
        /// <summary>Namespace the 3.1 generator emits, and therefore the only one a surviving partial can use.</summary>
        public const string GeneratedNamespace = "XrmFramework";

        /// <summary>Constants the generator re-emits from the <c>.table</c>.</summary>
        private static readonly HashSet<string> GeneratedConstants = new HashSet<string>(StringComparer.Ordinal)
        {
            "EntityName",
            "EntityCollectionName"
        };

        /// <summary>Nested classes the generator re-emits from the <c>.table</c>.</summary>
        private static readonly HashSet<string> GeneratedNestedClasses = new HashSet<string>(StringComparer.Ordinal)
        {
            "Columns",
            "AlternateKeyNames",
            "ManyToManyRelationships",
            "ManyToOneRelationships",
            "OneToManyRelationships"
        };

        /// <summary>
        /// Class-level attributes the generator re-emits. None of them allows multiple use, so leaving
        /// them on the surviving partial would break the build (CS0579).
        /// </summary>
        private static readonly HashSet<string> GeneratedClassAttributes = new HashSet<string>(StringComparer.Ordinal)
        {
            "GeneratedCode",
            "EntityDefinition",
            "ExcludeFromCodeCoverage"
        };

        /// <summary>
        /// Rewrites <paramref name="source"/>, the content of <c>{<paramref name="definitionClassName"/>}.cs</c>.
        /// </summary>
        /// <param name="source">Content of the <c>*Definition.cs</c> file.</param>
        /// <param name="definitionClassName">
        /// Name of the class to migrate, e.g. <c>ContactDefinition</c>. Matched case-insensitively when
        /// no class matches it exactly.
        /// </param>
        /// <param name="generatedEnumNames">
        /// Names of the option set enums the generator will emit for this directory's <c>.table</c> files.
        /// A namespace-level enum in the file is only removed if it appears here.
        /// </param>
        public static DefinitionRewriteResult Rewrite(
            string source, string definitionClassName, ICollection<string> generatedEnumNames)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(definitionClassName)) throw new ArgumentNullException(nameof(definitionClassName));

            var result = new DefinitionRewriteResult();

            // 1. Locate the scope holding the Definition class: a block namespace, a file-scoped
            //    namespace, or the file itself.
            CSharpMember namespaceMember;
            IList<CSharpMember> scopeMembers;

            var unreadable = ReadScope(source, out namespaceMember, out scopeMembers);
            if (unreadable != null)
                return Skip(result, unreadable);

            // 2. Locate the Definition class.
            var target = FindDefinitionClass(scopeMembers, definitionClassName);

            if (target == null)
                return Skip(result, $"no class named {definitionClassName}");

            if (target.BodyStart < 0)
                return Skip(result, $"class {definitionClassName} has no body");

            var classMembers = CSharpMemberReader.ReadMembers(source, target.BodyStart, target.BodyEnd);
            if (classMembers == null)
                return Skip(result, $"unreadable body of class {definitionClassName}");

            var edits = new List<Edit>();

            // 3. Remove from the class what the generator re-emits.
            foreach (var member in classMembers)
            {
                if (IsGenerated(member))
                {
                    edits.Add(Edit.Delete(member.FullStart, member.End));
                    result.RemovedMembers.Add(Describe(member));
                }
                else
                {
                    result.KeptMembers.Add(Describe(member));
                }
            }

            // 4. Remove the namespace-level enums the generator re-emits, and count what else is around
            //    the Definition class.
            var otherScopeMembers = 0;

            foreach (var member in scopeMembers)
            {
                if (ReferenceEquals(member, target)) continue;
                if (string.Equals(member.Keyword, "using", StringComparison.Ordinal)) continue;
                if (string.Equals(member.Keyword, "namespace", StringComparison.Ordinal)) continue;

                if (string.Equals(member.Keyword, "enum", StringComparison.Ordinal)
                    && generatedEnumNames != null && generatedEnumNames.Contains(member.Name))
                {
                    edits.Add(Edit.Delete(member.FullStart, member.End));
                    result.RemovedEnums.Add(member.Name);
                    continue;
                }

                otherScopeMembers++;
            }

            // 5. Decide the fate of the file.
            if (result.KeptMembers.Count == 0)
            {
                // Nothing left in the Definition class: the generated part covers it entirely.
                edits.Add(Edit.Delete(target.FullStart, target.End));

                if (otherScopeMembers == 0)
                {
                    result.Outcome = DefinitionRewriteOutcome.Delete;
                    return result;
                }
            }
            else
            {
                // The class survives: realign it on the generated part so the two merge.
                if (!target.HasModifier("partial") && target.KeywordStart >= 0)
                    edits.Add(Edit.Insert(target.KeywordStart, "partial "));

                foreach (var attributeList in target.Attributes)
                    AddAttributeEdit(edits, attributeList);
            }

            // 6. The generator emits into "XrmFramework": a surviving partial has to live there too.
            if (namespaceMember != null
                && namespaceMember.NameStart >= 0
                && !string.Equals(namespaceMember.Name, GeneratedNamespace, StringComparison.Ordinal))
            {
                edits.Add(Edit.Replace(namespaceMember.NameStart, namespaceMember.NameEnd, GeneratedNamespace));
            }

            result.Outcome = DefinitionRewriteOutcome.Rewrite;
            result.NewText = ApplyEdits(source, edits);
            return result;
        }

        /// <summary>
        /// Strips from <c>OptionSetDefinitions.cs</c> — the separate file the 2.* DefinitionManager
        /// wrote for the option set enums — everything the 3.1 generator re-emits.
        /// </summary>
        /// <remarks>
        /// That file holds no Definition class, only namespace-level enums, so it goes through its own
        /// pass. Only the enums the generator really re-emits are dropped: one that no selected column
        /// references is not regenerated, and the code still needs it. When nothing survives, the file
        /// as a whole is <see cref="DefinitionRewriteOutcome.Delete"/>d.
        /// </remarks>
        /// <param name="source">Content of the <c>OptionSetDefinitions.cs</c> file.</param>
        /// <param name="generatedEnumNames">
        /// Names of the option set enums the generator will emit for this directory's <c>.table</c> files.
        /// </param>
        public static DefinitionRewriteResult RewriteOptionSets(
            string source, ICollection<string> generatedEnumNames)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var result = new DefinitionRewriteResult();

            CSharpMember namespaceMember;
            IList<CSharpMember> scopeMembers;

            var unreadable = ReadScope(source, out namespaceMember, out scopeMembers);
            if (unreadable != null)
                return Skip(result, unreadable);

            var edits = new List<Edit>();

            foreach (var member in scopeMembers)
            {
                if (string.Equals(member.Keyword, "using", StringComparison.Ordinal)) continue;
                if (string.Equals(member.Keyword, "namespace", StringComparison.Ordinal)) continue;

                if (string.Equals(member.Keyword, "enum", StringComparison.Ordinal)
                    && generatedEnumNames != null && generatedEnumNames.Contains(member.Name))
                {
                    edits.Add(Edit.Delete(member.FullStart, member.End));
                    result.RemovedEnums.Add(member.Name);
                    continue;
                }

                result.KeptMembers.Add(Describe(member));
            }

            if (result.KeptMembers.Count == 0)
            {
                result.Outcome = DefinitionRewriteOutcome.Delete;
                return result;
            }

            // What survives is precisely what the generator does *not* emit, so it stays in the
            // project's own namespace: moving it to XrmFramework would break every reference to it.
            result.Outcome = DefinitionRewriteOutcome.Rewrite;
            result.NewText = ApplyEdits(source, edits);
            return result;
        }

        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Reads the members of the scope holding the file's types: the body of a block namespace, or
        /// the file itself when the namespace is file-scoped or absent.
        /// </summary>
        /// <returns>The reason to leave the file alone, or <c>null</c> when it was read reliably.</returns>
        private static string ReadScope(string source, out CSharpMember namespaceMember,
                                        out IList<CSharpMember> scopeMembers)
        {
            namespaceMember = null;
            scopeMembers = null;

            var topLevel = CSharpMemberReader.ReadMembers(source, 0, source.Length);
            if (topLevel == null)
                return "unreadable C# source";

            foreach (var member in topLevel)
                if (string.Equals(member.Keyword, "namespace", StringComparison.Ordinal))
                {
                    namespaceMember = member;
                    break;
                }

            if (namespaceMember != null && namespaceMember.BodyStart >= 0)
            {
                scopeMembers = CSharpMemberReader.ReadMembers(source, namespaceMember.BodyStart, namespaceMember.BodyEnd);
                if (scopeMembers == null)
                    return "unreadable namespace body";
            }
            else
            {
                // File-scoped namespace or no namespace at all: the members sit at the top level.
                scopeMembers = topLevel;
            }

            return null;
        }

        /// <summary>
        /// Locates the class to migrate among <paramref name="scopeMembers"/>.
        /// </summary>
        /// <remarks>
        /// The expected name comes from the file name, which under 2.* the DefinitionManager derived
        /// from the class — but a file renamed since, or a table whose declared name differs in case
        /// from its file (<c>Systemuser.table</c> declares <c>SystemUser</c>), makes the two drift apart.
        /// The lookup therefore falls back to a case-insensitive match, which is enough to recognize the
        /// class while an exact match still wins whenever one exists.
        /// </remarks>
        private static CSharpMember FindDefinitionClass(IEnumerable<CSharpMember> scopeMembers, string definitionClassName)
        {
            CSharpMember fallback = null;

            foreach (var member in scopeMembers)
            {
                if (!string.Equals(member.Keyword, "class", StringComparison.Ordinal))
                    continue;

                if (string.Equals(member.Name, definitionClassName, StringComparison.Ordinal))
                    return member;

                if (fallback == null
                    && string.Equals(member.Name, definitionClassName, StringComparison.OrdinalIgnoreCase))
                {
                    fallback = member;
                }
            }

            return fallback;
        }

        private static bool IsGenerated(CSharpMember member)
        {
            if (string.Equals(member.Keyword, "class", StringComparison.Ordinal))
                return GeneratedNestedClasses.Contains(member.Name);

            // Field or constant: no type keyword in the header.
            return member.Keyword.Length == 0 && GeneratedConstants.Contains(member.Name);
        }

        private static string Describe(CSharpMember member)
            => member.Keyword.Length == 0 ? member.Name : $"{member.Keyword} {member.Name}";

        /// <summary>
        /// Drops from <paramref name="attributeList"/> the attributes the generator re-emits: the whole
        /// list if none survives, otherwise a rebuilt list holding only the kept entries.
        /// </summary>
        private static void AddAttributeEdit(ICollection<Edit> edits, CSharpAttributeList attributeList)
        {
            var kept = new List<string>();

            foreach (var entry in attributeList.Entries)
                if (!GeneratedClassAttributes.Contains(entry.Name))
                    kept.Add(entry.Text);

            if (kept.Count == attributeList.Entries.Count)
                return;

            if (kept.Count == 0)
            {
                edits.Add(Edit.Delete(attributeList.FullStart, attributeList.End));
                return;
            }

            edits.Add(Edit.Replace(attributeList.Start, attributeList.End, "[" + string.Join(", ", kept.ToArray()) + "]"));
        }

        private static DefinitionRewriteResult Skip(DefinitionRewriteResult result, string reason)
        {
            result.Outcome = DefinitionRewriteOutcome.Skipped;
            result.Reason = reason;
            return result;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Edits
        // ──────────────────────────────────────────────────────────────────────────

        private struct Edit
        {
            public int Start;
            public int End;
            public string Text;

            public static Edit Delete(int start, int end) => new Edit { Start = start, End = end, Text = string.Empty };
            public static Edit Insert(int at, string text) => new Edit { Start = at, End = at, Text = text };
            public static Edit Replace(int start, int end, string text) => new Edit { Start = start, End = end, Text = text };
        }

        private static string ApplyEdits(string source, List<Edit> edits)
        {
            edits.Sort((a, b) => a.Start != b.Start ? a.Start.CompareTo(b.Start) : a.End.CompareTo(b.End));

            var sb = new StringBuilder(source.Length);
            var position = 0;

            foreach (var edit in edits)
            {
                // Edits are produced over disjoint spans; an overlap would mean a scanning bug,
                // and silently dropping the edit is safer than corrupting the file.
                if (edit.Start < position)
                    continue;

                sb.Append(source, position, edit.Start - position);
                sb.Append(edit.Text);
                position = edit.End;
            }

            sb.Append(source, position, source.Length - position);
            return sb.ToString();
        }
    }
}
