// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.ModelSync
{
    /// <summary>
    /// What must be done with a hand-written binding model's <c>.cs</c> file once rewritten.
    /// </summary>
    public enum ModelSourceRewriteOutcome
    {
        /// <summary>The file could not be read reliably, or names no matching class: left untouched.</summary>
        Skipped,

        /// <summary>Nothing survives the migration: the file must be deleted.</summary>
        Delete,

        /// <summary>Members remain: the file must be written as <c>*.partial.cs</c>.</summary>
        Rewrite
    }

    /// <summary>Result of rewriting one binding model class within a source file.</summary>
    public sealed class ModelSourceRewriteResult
    {
        public ModelSourceRewriteOutcome Outcome { get; set; }

        /// <summary>New content of the file. Only meaningful for <see cref="ModelSourceRewriteOutcome.Rewrite"/>.</summary>
        public string NewText { get; set; }

        /// <summary>Reason the file was skipped. Only meaningful for <see cref="ModelSourceRewriteOutcome.Skipped"/>.</summary>
        public string Reason { get; set; }

        /// <summary>Properties removed because <c>ModelSourceFileGenerator</c> now re-emits them from the <c>.model</c> file.</summary>
        public IList<string> RemovedMembers { get; } = new List<string>();

        /// <summary>Members that survived — hand-written logic the migration has no reason to touch.</summary>
        public IList<string> KeptMembers { get; } = new List<string>();
    }

    /// <summary>
    /// Strips from a hand-written binding model class the properties <c>ModelSourceFileGenerator</c>
    /// now re-emits from the <c>.model</c> file <c>migrate sync-models</c> produced for it, and
    /// reports what is left.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Once a <c>.model</c> file exists for a class, <c>ModelSourceFileGenerator</c> emits a
    /// <c>partial</c> class of the same name carrying the same <c>[CrmMapping]</c> /
    /// <c>[ChildRelationship]</c> / <c>[ExtendBindingModel]</c> properties. Left in the hand-written
    /// file, they become duplicate members — the project no longer compiles. Anything else the
    /// project added by hand (helper methods, computed properties, additional constructors) is not
    /// something the generator produces, so it is kept.
    /// </para>
    /// <para>
    /// The class-level attributes the generator also re-emits — <c>[CrmEntity]</c>, and, if present,
    /// <c>[GeneratedCode]</c> / <c>[ExcludeFromCodeCoverage]</c> / <c>[JsonObject]</c> — are dropped
    /// too: none of them allows multiple use, so leaving them on the surviving partial would break
    /// the build (CS0579). If anything survives, the class is marked <c>partial</c> so the two
    /// pieces merge instead of colliding.
    /// </para>
    /// <para>
    /// Reuses <see cref="CSharpMemberReader"/>, the lexical scanner <see cref="DefinitionSourceRewriter"/>
    /// already uses for the equivalent <c>*Definition.cs</c> cleanup: deliberately not a full parser —
    /// anything it cannot bracket reliably makes it skip the file rather than risk a wrong rewrite.
    /// </para>
    /// </remarks>
    public static class BindingModelSourceRewriter
    {
        /// <summary>Property attributes that mean "ModelSourceFileGenerator now re-emits this member".</summary>
        private static readonly HashSet<string> MappedAttributes = new HashSet<string>(StringComparer.Ordinal)
        {
            "CrmMapping",
            "ChildRelationship",
            "ExtendBindingModel"
        };

        /// <summary>
        /// Class-level attributes the generator re-emits. None of them allows multiple use, so leaving
        /// them on the surviving partial would break the build (CS0579).
        /// </summary>
        private static readonly HashSet<string> GeneratedClassAttributes = new HashSet<string>(StringComparer.Ordinal)
        {
            "CrmEntity",
            "GeneratedCode",
            "ExcludeFromCodeCoverage",
            "JsonObject"
        };

        /// <summary>
        /// Rewrites <paramref name="source"/>, stripping the mapped properties of the class named
        /// <paramref name="className"/> — the same class <see cref="ModelSync.ModelDefinitionAnalyzer"/>
        /// read to produce its <c>.model</c> file.
        /// </summary>
        /// <param name="source">Content of the <c>.cs</c> file.</param>
        /// <param name="className">Name of the class to migrate, matched case-sensitively — C# class names are.</param>
        public static ModelSourceRewriteResult Rewrite(string source, string className)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(className)) throw new ArgumentNullException(nameof(className));

            var result = new ModelSourceRewriteResult();

            CSharpMember namespaceMember;
            IList<CSharpMember> scopeMembers;

            var unreadable = ReadScope(source, out namespaceMember, out scopeMembers);
            if (unreadable != null)
                return Skip(result, unreadable);

            var target = FindClass(scopeMembers, className);

            if (target == null)
                return Skip(result, $"no class named {className}");

            if (target.BodyStart < 0)
                return Skip(result, $"class {className} has no body");

            var classMembers = CSharpMemberReader.ReadMembers(source, target.BodyStart, target.BodyEnd);
            if (classMembers == null)
                return Skip(result, $"unreadable body of class {className}");

            var edits = new List<Edit>();

            foreach (var member in classMembers)
            {
                if (IsMapped(member))
                {
                    edits.Add(Edit.Delete(member.FullStart, member.End));
                    result.RemovedMembers.Add(member.Name);
                }
                else
                {
                    result.KeptMembers.Add(Describe(member));
                }
            }

            if (result.KeptMembers.Count == 0)
            {
                // Nothing of the class survives: the .model-driven partial covers it entirely. The
                // class declaration itself is removed from the text — NewText is filled in either
                // way, since a file can hold more than this one class, and whether the *file* is
                // then empty too is the caller's call, not this one's.
                edits.Add(Edit.Delete(target.FullStart, target.End));
                result.Outcome = ModelSourceRewriteOutcome.Delete;
            }
            else
            {
                // The class survives: realign it on the generated part so the two merge.
                if (!target.HasModifier("partial") && target.KeywordStart >= 0)
                    edits.Add(Edit.Insert(target.KeywordStart, "partial "));

                foreach (var attributeList in target.Attributes)
                    AddAttributeEdit(edits, attributeList);

                result.Outcome = ModelSourceRewriteOutcome.Rewrite;
            }

            result.NewText = ApplyEdits(source, edits);
            return result;
        }

        /// <summary>
        /// True when <paramref name="source"/> holds nothing but boilerplate — usings and an empty
        /// namespace, no class/interface/struct/enum left. A file can declare several classes
        /// <see cref="Rewrite"/> is applied to one at a time; only once every one of them is gone (and
        /// nothing else was ever there) is the file itself redundant, rather than merely one class
        /// within it.
        /// </summary>
        public static bool IsEmptyOfMembers(string source)
        {
            CSharpMember namespaceMember;
            IList<CSharpMember> scopeMembers;

            // Unreadable source is conservatively assumed to still hold something.
            return ReadScope(source, out namespaceMember, out scopeMembers) == null && scopeMembers.Count == 0;
        }

        // ──────────────────────────────────────────────────────────────────────────

        private static string ReadScope(string source, out CSharpMember namespaceMember, out IList<CSharpMember> scopeMembers)
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

        /// <summary>Finds the class to migrate — matched case-sensitively, unlike a *Definition.cs lookup.</summary>
        private static CSharpMember FindClass(IEnumerable<CSharpMember> scopeMembers, string className)
        {
            foreach (var member in scopeMembers)
            {
                if (!string.Equals(member.Keyword, "class", StringComparison.Ordinal))
                    continue;

                if (string.Equals(member.Name, className, StringComparison.Ordinal))
                    return member;
            }

            return null;
        }

        private static bool IsMapped(CSharpMember member)
        {
            foreach (var attributeList in member.Attributes)
                foreach (var entry in attributeList.Entries)
                    if (MappedAttributes.Contains(entry.Name))
                        return true;

            return false;
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

        private static ModelSourceRewriteResult Skip(ModelSourceRewriteResult result, string reason)
        {
            result.Outcome = ModelSourceRewriteOutcome.Skipped;
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
