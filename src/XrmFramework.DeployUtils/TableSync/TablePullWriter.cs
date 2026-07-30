// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Résultat de l'écriture d'une table récupérée.
    /// </summary>
    public sealed class TablePullOutcome
    {
        internal TablePullOutcome(
            string filePath, bool created, CoreTable table, IReadOnlyList<Column> columnsMissingFromCrm)
        {
            FilePath = filePath;
            Created = created;
            Table = table;
            ColumnsMissingFromCrm = columnsMissingFromCrm;
        }

        /// <summary>Chemin du fichier écrit.</summary>
        public string FilePath { get; }

        /// <summary>Vrai si aucun fichier ne décrivait cette entité auparavant.</summary>
        public bool Created { get; }

        /// <summary>Table telle qu'écrite sur disque.</summary>
        public CoreTable Table { get; }

        /// <summary>
        /// Colonnes présentes dans le fichier mais absentes de l'environnement. Elles sont
        /// conservées : une récupération rafraîchit, elle ne détruit pas.
        /// </summary>
        public IReadOnlyList<Column> ColumnsMissingFromCrm { get; }
    }

    /// <summary>
    /// Réconcilie une table fraîchement lue dans le CRM avec le fichier <c>.table</c> versionné,
    /// puis l'écrit.
    /// </summary>
    /// <remarks>
    /// Extrait de la commande afin que celle-ci et les tests exercent exactement le même chemin :
    /// localisation du fichier, fusion, écriture. Une orchestration recopiée dans les tests serait
    /// aveugle aux dérives de la commande réelle.
    /// </remarks>
    public static class TablePullWriter
    {
        /// <summary>
        /// Écrit <paramref name="freshTable" /> dans <paramref name="tablesDirectory" /> en
        /// préservant ce qui appartient au fichier existant (noms C#, sélection, verrous).
        /// </summary>
        public static TablePullOutcome Write(string tablesDirectory, CoreTable freshTable)
        {
            if (string.IsNullOrWhiteSpace(tablesDirectory))
                throw new ArgumentException("Le répertoire des .table est obligatoire.", nameof(tablesDirectory));

            if (freshTable == null)
                throw new ArgumentNullException(nameof(freshTable));

            // Le fichier est retrouvé par son nom logique : son nom de fichier suit le nom C# de la
            // table, que les équipes renomment librement.
            var path = TableFileStore.FindTableFile(tablesDirectory, freshTable.LogicalName);
            var existing = path == null ? null : TableFileStore.Load(path);

            var merged = TableMerger.Merge(existing, freshTable);
            var missing = TableMerger.GetColumnsMissingFromCrm(existing, freshTable);

            path = path ?? TableFileStore.BuildTableFilePath(tablesDirectory, merged.Name);
            TableFileStore.Save(path, merged);

            return new TablePullOutcome(path, existing == null, merged, missing);
        }
    }
}
