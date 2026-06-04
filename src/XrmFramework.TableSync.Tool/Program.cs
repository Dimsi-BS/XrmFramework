// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using XrmFramework.DeployUtils;

// Délègue entièrement le traitement à TableSyncHelper, qui gère :
//   - le parsing des arguments (--dll, --tables-dir, --clean)
//   - l'extraction des classes [EntityDefinition] depuis le DLL
//   - la synchronisation des fichiers .table
//   - les messages de progression / erreurs via AnsiConsole
TableSyncHelper.SyncTables(args);
