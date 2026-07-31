// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using XrmFramework.DeployUtils.TableSync;

namespace XrmFramework.DeployUtils.Tests.TableSync;

/// <summary>
/// Rewriting a <c>*Definition.cs</c> inherited from XrmFramework 2.*: what the 3.1 source generator
/// re-emits must go, everything the project wrote by hand must survive.
/// </summary>
[TestFixture]
public class DefinitionSourceRewriterTests
{
    private static readonly ICollection<string> NoEnums = new HashSet<string>();

    // ══════════════════════════════════════════════════════════════════════════
    // Fixtures — the exact shape the 2.* DefinitionManager produced
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Definition entirely generated under 2.*: nothing in it survives the migration.
    /// </summary>
    private const string FullyGeneratedSource = """
                                                using System;
                                                using System.CodeDom.Compiler;
                                                using System.Diagnostics.CodeAnalysis;

                                                namespace MyProject.Core
                                                {
                                                    [GeneratedCode("XrmFramework", "2.0")]
                                                    [EntityDefinition]
                                                    [ExcludeFromCodeCoverage]
                                                    public static class ContactDefinition
                                                    {
                                                        public const string EntityName = "contact";
                                                        public const string EntityCollectionName = "contacts";

                                                        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
                                                        public static class Columns
                                                        {
                                                            /// <summary>
                                                            /// Type : String
                                                            /// </summary>
                                                            [AttributeMetadata(AttributeTypeCode.String)]
                                                            [PrimaryAttribute(PrimaryAttributeType.Name)]
                                                            public const string FullName = "fullname";
                                                        }

                                                        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
                                                        public static class AlternateKeyNames
                                                        {
                                                            public const string EmailKey = "contact_email_key";
                                                        }

                                                        public static class ManyToOneRelationships
                                                        {
                                                            [Relationship("account", EntityRole.Referencing, "parentcustomerid", "parentcustomerid")]
                                                            public const string contact_customer_accounts = "contact_customer_accounts";
                                                        }

                                                        public static class OneToManyRelationships
                                                        {
                                                        }

                                                        public static class ManyToManyRelationships
                                                        {
                                                        }
                                                    }
                                                }
                                                """;

    /// <summary>
    /// Same file, plus a constant the project added by hand — the case that survives as a partial.
    /// </summary>
    private const string HandEditedSource = """
                                            using System;
                                            using System.CodeDom.Compiler;
                                            using System.Diagnostics.CodeAnalysis;
                                            using XrmFramework.Definitions.Internal;

                                            namespace MyProject.Core
                                            {
                                                [GeneratedCode("XrmFramework", "2.0")]
                                                [EntityDefinition]
                                                [ExcludeFromCodeCoverage]
                                                [DefinitionManagerIgnore]
                                                public static class SystemUserDefinition
                                                {
                                                    public const string EntityName = "systemuser";
                                                    public const string EntityCollectionName = "systemusers";

                                                    /// <summary>Name of the membership relation, added by hand.</summary>
                                                    public const String TeamMembershipRelationName = "teammembership";

                                                    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
                                                    public static class Columns
                                                    {
                                                        [AttributeMetadata(AttributeTypeCode.Uniqueidentifier)]
                                                        public const string Id = "systemuserid";
                                                    }
                                                }
                                            }
                                            """;

    // ══════════════════════════════════════════════════════════════════════════
    // Removing what the generator re-emits
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Rewrite_DeletesFile_WhenEverythingIsRegenerated()
    {
        var result = DefinitionSourceRewriter.Rewrite(FullyGeneratedSource, "ContactDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Delete, result.Outcome,
            "A Definition holding nothing but generated members no longer has any reason to exist.");
        CollectionAssert.IsEmpty(result.KeptMembers);
    }

    [Test]
    public void Rewrite_RemovesEntityNameAndCollectionName()
    {
        var result = DefinitionSourceRewriter.Rewrite(FullyGeneratedSource, "ContactDefinition", NoEnums);

        CollectionAssert.Contains(result.RemovedMembers, "EntityName");
        CollectionAssert.Contains(result.RemovedMembers, "EntityCollectionName");
    }

    [Test]
    public void Rewrite_RemovesAllFiveGeneratedNestedClasses()
    {
        var result = DefinitionSourceRewriter.Rewrite(FullyGeneratedSource, "ContactDefinition", NoEnums);

        CollectionAssert.IsSupersetOf(result.RemovedMembers, new[]
        {
            "class Columns",
            "class AlternateKeyNames",
            "class ManyToOneRelationships",
            "class OneToManyRelationships",
            "class ManyToManyRelationships"
        });
    }

    [Test]
    public void Rewrite_KeepsHandWrittenMember()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Rewrite, result.Outcome);
        CollectionAssert.AreEqual(new[] { "TeamMembershipRelationName" }, result.KeptMembers);
    }

    [Test]
    public void Rewrite_KeepsDocCommentOfKeptMember()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.Contains("<summary>Name of the membership relation, added by hand.</summary>",
            result.NewText, "The XML doc of a kept member must not be swallowed with its neighbours.");
    }

    [Test]
    public void Rewrite_RemovesGeneratedMembersAndTheirAttributes()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.DoesNotContain("EntityName", result.NewText);
        StringAssert.DoesNotContain("EntityCollectionName", result.NewText);
        StringAssert.DoesNotContain("class Columns", result.NewText);
        StringAssert.DoesNotContain("CA1034", result.NewText,
            "The [SuppressMessage] carried by a removed nested class must go with it.");
        StringAssert.DoesNotContain("AttributeMetadata", result.NewText);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Realigning the surviving partial on the generated part
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Rewrite_MakesSurvivingClassPartial()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.Contains("public static partial class SystemUserDefinition", result.NewText,
            "Without 'partial' the file declares a second type instead of merging with the generated one.");
    }

    [Test]
    public void Rewrite_DoesNotDuplicatePartial_WhenAlreadyPresent()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static partial class ContactDefinition
                                  {
                                      public const string EntityName = "contact";
                                      public const string Custom = "custom";
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        StringAssert.Contains("public static partial class ContactDefinition", result.NewText);
        StringAssert.DoesNotContain("partial partial", result.NewText);
    }

    [Test]
    public void Rewrite_DropsClassAttributesTheGeneratorReemits()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.DoesNotContain("GeneratedCode", result.NewText);
        StringAssert.DoesNotContain("[EntityDefinition]", result.NewText);
        StringAssert.DoesNotContain("ExcludeFromCodeCoverage", result.NewText);
    }

    [Test]
    public void Rewrite_KeepsClassAttributesTheGeneratorDoesNotEmit()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.Contains("[DefinitionManagerIgnore]", result.NewText,
            "An attribute absent from the generated part is not a duplicate: it must survive.");
    }

    [Test]
    public void Rewrite_KeepsSurvivingAttributes_InAMixedAttributeList()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  [EntityDefinition, DefinitionManagerIgnore]
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";
                                      public const string Custom = "custom";
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        StringAssert.Contains("[DefinitionManagerIgnore]", result.NewText);
        StringAssert.DoesNotContain("EntityDefinition", result.NewText);
    }

    [Test]
    public void Rewrite_MovesSurvivingPartialToTheGeneratedNamespace()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "SystemUserDefinition", NoEnums);

        StringAssert.Contains("namespace XrmFramework", result.NewText,
            "The generator emits into XrmFramework: a partial declared elsewhere would not merge.");
        StringAssert.DoesNotContain("namespace MyProject.Core", result.NewText);
    }

    [Test]
    public void Rewrite_HandlesFileScopedNamespace()
    {
        const string source = """
                              namespace MyProject.Core;

                              [EntityDefinition]
                              public static class ContactDefinition
                              {
                                  public const string EntityName = "contact";
                                  public const string Custom = "custom";
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Rewrite, result.Outcome);
        StringAssert.Contains("namespace XrmFramework;", result.NewText);
        StringAssert.Contains("public static partial class ContactDefinition", result.NewText);
        StringAssert.DoesNotContain("EntityName", result.NewText);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Namespace-level option set enums
    // ══════════════════════════════════════════════════════════════════════════

    private const string SourceWithEnums = """
                                           namespace MyProject.Core
                                           {
                                               [EntityDefinition]
                                               public static class ContactDefinition
                                               {
                                                   public const string EntityName = "contact";
                                               }

                                               [OptionSetDefinition(ContactDefinition.EntityName, ContactDefinition.Columns.StatusCode)]
                                               public enum ContactStatus
                                               {
                                                   [Description("Active")]
                                                   Active = 0,
                                               }

                                               public enum HandWrittenFlags
                                               {
                                                   None = 0,
                                               }
                                           }
                                           """;

    [Test]
    public void Rewrite_RemovesEnumsTheGeneratorReemits()
    {
        var generated = new HashSet<string> { "ContactStatus" };

        var result = DefinitionSourceRewriter.Rewrite(SourceWithEnums, "ContactDefinition", generated);

        CollectionAssert.AreEqual(new[] { "ContactStatus" }, result.RemovedEnums);
        StringAssert.DoesNotContain("ContactStatus", result.NewText);
    }

    [Test]
    public void Rewrite_KeepsEnumsAbsentFromTheTable()
    {
        var generated = new HashSet<string> { "ContactStatus" };

        var result = DefinitionSourceRewriter.Rewrite(SourceWithEnums, "ContactDefinition", generated);

        StringAssert.Contains("enum HandWrittenFlags", result.NewText,
            "An enum the generator will not produce is hand-written code: deleting it would lose it.");
    }

    [Test]
    public void Rewrite_KeepsFileAndDropsEmptyClass_WhenOnlyHandWrittenEnumsRemain()
    {
        var generated = new HashSet<string> { "ContactStatus" };

        var result = DefinitionSourceRewriter.Rewrite(SourceWithEnums, "ContactDefinition", generated);

        Assert.AreEqual(DefinitionRewriteOutcome.Rewrite, result.Outcome,
            "Deleting the file would take the hand-written enum with it.");
        StringAssert.DoesNotContain("class ContactDefinition", result.NewText,
            "An emptied Definition class is entirely covered by the generated part.");
    }

    [Test]
    public void Rewrite_DeletesFile_WhenAllEnumsAreRegenerated()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  [EntityDefinition]
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";
                                  }

                                  public enum ContactStatus
                                  {
                                      Active = 0,
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", new HashSet<string> { "ContactStatus" });

        Assert.AreEqual(DefinitionRewriteOutcome.Delete, result.Outcome);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Refusing to act rather than acting wrongly
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Rewrite_Skips_WhenTheExpectedClassIsAbsent()
    {
        var result = DefinitionSourceRewriter.Rewrite(FullyGeneratedSource, "AccountDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Skipped, result.Outcome);
        StringAssert.Contains("AccountDefinition", result.Reason);
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Locating the class: the expected name comes from the file name, which may
    // have drifted from the class in casing
    // ══════════════════════════════════════════════════════════════════════════

    [Test]
    public void Rewrite_FindsTheClass_WhenCasingDiffersFromTheExpectedName()
    {
        var result = DefinitionSourceRewriter.Rewrite(FullyGeneratedSource, "CONTACTDEFINITION", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Delete, result.Outcome,
            "A file named with a different casing still designates the same class.");
    }

    [Test]
    public void Rewrite_KeepsTheCasingOfTheDeclaredClass_InTheRewrittenFile()
    {
        var result = DefinitionSourceRewriter.Rewrite(HandEditedSource, "systemuserdefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Rewrite, result.Outcome);
        StringAssert.Contains("public static partial class SystemUserDefinition", result.NewText,
            "Matching loosely must not rewrite the declaration: C# is case-sensitive.");
    }

    [Test]
    public void Rewrite_PrefersTheExactMatch_WhenBothCasingsAreDeclared()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static class contactdefinition
                                  {
                                      public const string Decoy = "decoy";
                                  }

                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";
                                      public const string Custom = "custom";
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        CollectionAssert.AreEqual(new[] { "Custom" }, result.KeptMembers,
            "An exact match must win over the one that only differs in casing.");
        StringAssert.Contains("Decoy", result.NewText, "The other class is none of our business.");
    }

    [Test]
    public void Rewrite_Skips_WhenBracesAreUnbalanced()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Skipped, result.Outcome,
            "A file that cannot be read reliably must be left untouched, never guessed at.");
    }

    [Test]
    public void Rewrite_IsNotFooledByBracesInsideLiteralsAndComments()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";

                                      // a stray brace } in a comment
                                      /* and another { in a block comment */
                                      public const string Pattern = "{ not a block }";
                                      public const string Verbatim = @"c:\temp\{x}";
                                      public const char Brace = '}';
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        Assert.AreEqual(DefinitionRewriteOutcome.Rewrite, result.Outcome);
        CollectionAssert.AreEqual(new[] { "Pattern", "Verbatim", "Brace" }, result.KeptMembers);
    }

    [Test]
    public void Rewrite_KeepsMethodsAndProperties()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";

                                      public static string Label { get; } = "Contact";

                                      public static string Describe(string suffix)
                                      {
                                          return EntityName + suffix;
                                      }
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        CollectionAssert.AreEqual(new[] { "Label", "Describe" }, result.KeptMembers);
    }

    [Test]
    public void Rewrite_KeepsNestedClassesTheGeneratorDoesNotProduce()
    {
        const string source = """
                              namespace MyProject.Core
                              {
                                  public static class ContactDefinition
                                  {
                                      public const string EntityName = "contact";

                                      public static class Queries
                                      {
                                          public const string Active = "active";
                                      }
                                  }
                              }
                              """;

        var result = DefinitionSourceRewriter.Rewrite(source, "ContactDefinition", NoEnums);

        CollectionAssert.AreEqual(new[] { "class Queries" }, result.KeptMembers);
    }
}
