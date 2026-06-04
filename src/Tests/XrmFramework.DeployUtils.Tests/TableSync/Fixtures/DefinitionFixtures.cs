// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.CodeDom.Compiler;

namespace XrmFramework.DeployUtils.Tests.TableSync.Fixtures
{
    // ──────────────────────────────────────────────────────────────────────────
    // Le DefinitionAnalyzer matche les attributs par SIMPLE NAME. Le projet de
    // tests référence déjà la vraie [EntityDefinition] (via les sources liées
    // de XrmFramework.Tests), donc on l'utilise directement.
    //
    // Toutes les fixtures sont préfixées par "TableSyncTest" pour éviter les
    // collisions avec les Definition fixtures déjà présentes dans les sources
    // de tests partagées (Contact, Account, Dummy, Foo, etc.).
    // ──────────────────────────────────────────────────────────────────────────

    // ──────────────────────────────────────────────────────────────────────────
    // Fixture "classique" : Definition C# écrite à la main par l'utilisateur.
    // TableName attendu : "TableSyncTestContact".
    // ──────────────────────────────────────────────────────────────────────────

    [EntityDefinition]
    public static class TableSyncTestContactDefinition
    {
        public const string EntityName = "tabsync_contact";
        public const string EntityCollectionName = "tabsync_contacts";

        public static class Columns
        {
            public const string Id = "tabsync_contactid";
            public const string FirstName = "tabsync_firstname";
            public const string LastName = "tabsync_lastname";
            public const string Email = "tabsync_email";
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Entièrement générée par le TableSourceFileGenerator Roslyn.
    // TableName attendu : "TableSyncTestAccount".
    // ──────────────────────────────────────────────────────────────────────────

    [GeneratedCode("XrmFramework", "2.0")]
    [EntityDefinition]
    public static class TableSyncTestAccountDefinition
    {
        public const string EntityName = "tabsync_account";
        public const string EntityCollectionName = "tabsync_accounts";

        public static class Columns
        {
            public const string Id = "tabsync_accountid";
            public const string Name = "tabsync_name";
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Pas de suffixe "Definition" → le nom de la classe est conservé tel quel
    // comme TableName ("TableSyncTestNoSuffix").
    // ──────────────────────────────────────────────────────────────────────────

    [EntityDefinition]
    public static class TableSyncTestNoSuffix
    {
        public const string EntityName = "tabsync_nosuffix";
        public const string EntityCollectionName = "tabsync_nosuffixes";

        public static class Columns
        {
            public const string Id = "tabsync_nosuffixid";
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Sans champ EntityName → DOIT être ignorée par l'analyzer.
    // ──────────────────────────────────────────────────────────────────────────

    [EntityDefinition]
    public static class TableSyncTestIncompleteDefinition
    {
        public static class Columns
        {
            public const string Id = "tabsync_incompleteid";
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Sans [EntityDefinition] → DOIT être ignorée par l'analyzer.
    // ──────────────────────────────────────────────────────────────────────────

    public static class TableSyncTestNotADefinition
    {
        public const string EntityName = "tabsync_ghost";

        public static class Columns
        {
            public const string Id = "tabsync_ghostid";
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Avec EntityName mais Columns vide → 0 colonnes, cas légitime.
    // ──────────────────────────────────────────────────────────────────────────

    [EntityDefinition]
    public static class TableSyncTestEmptyDefinition
    {
        public const string EntityName = "tabsync_empty";
        public const string EntityCollectionName = "tabsync_empties";

        public static class Columns
        {
            // intentionnellement vide
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Sans EntityCollectionName → CollectionName doit être null.
    // ──────────────────────────────────────────────────────────────────────────

    [EntityDefinition]
    public static class TableSyncTestNoCollectionDefinition
    {
        public const string EntityName = "tabsync_nocoll";

        public static class Columns
        {
            public const string Id = "tabsync_nocollid";
        }
    }
}
