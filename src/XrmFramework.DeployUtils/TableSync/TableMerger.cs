// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using XrmFramework.Core;
using CoreTable = XrmFramework.Core.Table;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// Fusionne les métadonnées fraîchement lues dans le CRM avec le contenu d'un <c>.table</c>
    /// déjà versionné.
    /// </summary>
    /// <remarks>
    /// Principe directeur : <b>tout ce qui devient un identifiant C# appartient au fichier, tout
    /// ce qui décrit la table appartient au CRM.</b>
    ///
    /// Les noms (<c>Name</c>) alimentent le code généré et sont fréquemment ajustés à la main
    /// (<c>ftp_numeroContrat</c> renommé <c>NumeroContrat</c>). Les écraser avec le
    /// <c>SchemaName</c> du CRM casserait la compilation du projet consommateur à chaque
    /// récupération. À l'inverse, types, libellés, capacités et bornes sont précisément ce que
    /// l'on vient rafraîchir.
    ///
    /// Cette fusion n'utilise volontairement ni <see cref="CoreTable.MergeTo" /> ni
    /// <see cref="ColumnCollection" />.Add : leurs règles de rapprochement, pensées pour la
    /// sélection dans l'ancien outil graphique, écrasent des métadonnées et écartent
    /// silencieusement certaines colonnes.
    /// </remarks>
    public static class TableMerger
    {
        /// <summary>
        /// Produit la table à écrire à partir de l'existant et des métadonnées CRM.
        /// </summary>
        /// <param name="existing">
        /// Contenu du <c>.table</c> versionné, ou <see langword="null" /> à la première récupération.
        /// </param>
        /// <param name="fresh">Table construite depuis les métadonnées Dataverse.</param>
        public static CoreTable Merge(CoreTable existing, CoreTable fresh)
        {
            if (fresh == null)
                throw new ArgumentNullException(nameof(fresh));

            if (existing == null)
                return fresh;

            var merged = new CoreTable
            {
                LogicalName = fresh.LogicalName,
                CollectionName = fresh.CollectionName,

                // Nom de type C# et marqueur local : propriété du fichier.
                Name = string.IsNullOrEmpty(existing.Name) ? fresh.Name : existing.Name,
                IsLocked = existing.IsLocked
            };

            MergeColumns(existing, fresh, merged);
            MergeKeys(existing, fresh, merged);
            MergeEnums(existing, fresh, merged);

            // Les relations sont identifiées par leur propre nom : un renommage manuel les rendrait
            // impossibles à rapprocher. Elles sont donc reprises telles quelles depuis le CRM.
            merged.OneToManyRelationships.AddRange(fresh.OneToManyRelationships);
            merged.ManyToOneRelationships.AddRange(fresh.ManyToOneRelationships);
            merged.ManyToManyRelationships.AddRange(fresh.ManyToManyRelationships);

            return merged;
        }

        // ──────────────────────────────────────────────────────────────────────
        // Colonnes
        // ──────────────────────────────────────────────────────────────────────

        private static void MergeColumns(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingColumns = existing.Columns
                .Where(c => c.LogicalName != null)
                .GroupBy(c => c.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var freshLogicalNames = new HashSet<string>(
                fresh.Columns.Select(c => c.LogicalName), StringComparer.OrdinalIgnoreCase);

            foreach (var freshColumn in fresh.Columns)
            {
                if (existingColumns.TryGetValue(freshColumn.LogicalName, out var existingColumn))
                {
                    // Nom C#, sélection et verrou proviennent du fichier ; tout le reste du CRM.
                    freshColumn.Name = existingColumn.Name;
                    freshColumn.Selected = existingColumn.Selected;
                    freshColumn.IsLocked = existingColumn.IsLocked;
                }

                merged.Columns.Add(freshColumn);
            }

            // Une colonne supprimée dans l'environnement est conservée : « pull » rafraîchit, il ne
            // détruit pas. C'est « tables sync --clean » qui gère la désélection des orphelines.
            foreach (var orphan in existing.Columns.Where(c => !freshLogicalNames.Contains(c.LogicalName)))
                merged.Columns.Add(orphan);
        }

        /// <summary>
        /// Colonnes présentes dans le fichier mais absentes des métadonnées CRM.
        /// </summary>
        public static IReadOnlyList<Column> GetColumnsMissingFromCrm(CoreTable existing, CoreTable fresh)
        {
            if (existing == null || fresh == null)
                return new List<Column>();

            var freshLogicalNames = new HashSet<string>(
                fresh.Columns.Select(c => c.LogicalName), StringComparer.OrdinalIgnoreCase);

            return existing.Columns
                           .Where(c => !freshLogicalNames.Contains(c.LogicalName))
                           .ToList();
        }

        // ──────────────────────────────────────────────────────────────────────
        // Clés alternatives
        // ──────────────────────────────────────────────────────────────────────

        private static void MergeKeys(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingKeys = existing.Keys
                .Where(k => k.LogicalName != null)
                .GroupBy(k => k.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var freshKey in fresh.Keys)
            {
                if (freshKey.LogicalName != null
                    && existingKeys.TryGetValue(freshKey.LogicalName, out var existingKey)
                    && !string.IsNullOrEmpty(existingKey.Name))
                {
                    // Le nom de la clé devient une constante C# : il reste au fichier.
                    freshKey.Name = existingKey.Name;
                }

                merged.Keys.Add(freshKey);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Option sets
        // ──────────────────────────────────────────────────────────────────────

        private static void MergeEnums(CoreTable existing, CoreTable fresh, CoreTable merged)
        {
            var existingEnums = existing.Enums
                .Where(e => e.LogicalName != null)
                .GroupBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var freshLogicalNames = new HashSet<string>(
                fresh.Enums.Select(e => e.LogicalName), StringComparer.OrdinalIgnoreCase);

            foreach (var freshEnum in fresh.Enums)
            {
                if (freshEnum.LogicalName != null
                    && existingEnums.TryGetValue(freshEnum.LogicalName, out var existingEnum))
                {
                    MergeEnum(existingEnum, freshEnum);
                }

                merged.Enums.Add(freshEnum);
            }

            foreach (var orphan in existing.Enums.Where(e => !freshLogicalNames.Contains(e.LogicalName)))
                merged.Enums.Add(orphan);
        }

        /// <summary>
        /// Reporte sur un option set fraîchement lu les éléments appartenant au fichier : son nom de
        /// type C#, son verrou, et le nom de chacun de ses membres (rapprochés par valeur numérique,
        /// seule donnée stable — le libellé CRM peut changer sans que le code doive suivre).
        /// </summary>
        internal static void MergeEnum(OptionSetEnum existingEnum, OptionSetEnum freshEnum)
        {
            if (!string.IsNullOrEmpty(existingEnum.Name))
                freshEnum.Name = existingEnum.Name;

            freshEnum.IsLocked = existingEnum.IsLocked;

            var existingValues = existingEnum.Values
                .GroupBy(v => v.Value)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var freshValue in freshEnum.Values)
            {
                if (existingValues.TryGetValue(freshValue.Value, out var existingValue)
                    && !string.IsNullOrEmpty(existingValue.Name))
                {
                    freshValue.Name = existingValue.Name;
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        // Option sets globaux
        // ──────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Fusionne des option sets globaux dans le pseudo-table <c>OptionSet.table</c>.
        /// </summary>
        /// <remarks>
        /// Les option sets globaux sont partagés par toutes les tables : récupérer une seule entité
        /// ne doit jamais retirer ceux que les autres référencent. La fusion est donc purement
        /// additive.
        /// </remarks>
        public static CoreTable MergeGlobalOptionSets(
            CoreTable existing, IEnumerable<OptionSetEnum> freshEnums)
        {
            var merged = new CoreTable
            {
                LogicalName = TableFileStore.GlobalOptionSetLogicalName,
                Name = TableFileStore.GlobalOptionSetFileName
            };

            if (existing != null)
            {
                merged.LogicalName = existing.LogicalName ?? merged.LogicalName;
                merged.Name = string.IsNullOrEmpty(existing.Name) ? merged.Name : existing.Name;
                merged.IsLocked = existing.IsLocked;
            }

            var existingEnums = (existing?.Enums ?? new List<OptionSetEnum>())
                .Where(e => e.LogicalName != null)
                .GroupBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var refreshed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var freshEnum in freshEnums ?? Enumerable.Empty<OptionSetEnum>())
            {
                if (freshEnum.LogicalName == null || !refreshed.Add(freshEnum.LogicalName))
                    continue;

                if (existingEnums.TryGetValue(freshEnum.LogicalName, out var existingEnum))
                    MergeEnum(existingEnum, freshEnum);

                merged.Enums.Add(freshEnum);
            }

            foreach (var untouched in existingEnums.Values.Where(e => !refreshed.Contains(e.LogicalName)))
                merged.Enums.Add(untouched);

            // Ordre stable : le fichier est versionné, un ordre dépendant de la table récupérée
            // produirait des diffs parasites à chaque exécution.
            var ordered = merged.Enums.OrderBy(e => e.LogicalName, StringComparer.OrdinalIgnoreCase).ToList();
            merged.Enums.Clear();
            merged.Enums.AddRange(ordered);

            return merged;
        }
    }
}
