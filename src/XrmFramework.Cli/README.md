# XrmFramework.Cli

CLI XrmFramework distribué comme **.NET tool**. Il regroupe les utilitaires de
développement et de déploiement Dynamics 365 / Dataverse derrière une seule
commande : **`xrmframework`**.

La logique métier vit dans la librairie [`XrmFramework.DeployUtils`](../XrmFramework.DeployUtils) ;
ce projet n'en est que la façade ligne de commande (basée sur
[Spectre.Console.Cli](https://spectreconsole.net/cli/)).

---

## Installation

### En tant que tool global

```bash
dotnet tool install --global XrmFramework.Cli
xrmframework --help
```

### En tant que tool local (recommandé par dépôt/solution)

```bash
# à la racine du dépôt consommateur
dotnet new tool-manifest          # si .config/dotnet-tools.json n'existe pas encore
dotnet tool install XrmFramework.Cli
dotnet xrmframework --help        # ou : dotnet tool run xrmframework -- --help
```

Le tool local est figé dans `.config/dotnet-tools.json` (versionné), ce qui garantit
que toute l'équipe et la CI utilisent la même version.

### Depuis les sources (développement)

```bash
dotnet run --project src/XrmFramework.Cli -- <commande> [options]
```

---

## Configuration de l'environnement

Les commandes **connectées** (`deploy`, `tables list`, `tables pull`) ciblent
l'environnement *sélectionné* dans la configuration du projet, via deux fichiers
(mécanisme XrmFramework existant) lus dans le dossier **`Config/`** de la racine du projet :

| Fichier | Rôle |
|---|---|
| `Config/xrmFramework.config` | Déclare les projets et la connexion active (`selectedConnection`). |
| `Config/connectionStrings.config` | Définit les chaînes de connexion nommées (Dataverse / On-Premises). |

`selectedConnection` pointe vers une entrée de `connectionStrings.config` : c'est
**l'environnement cible**. La commande `tables sync`, elle, n'a **pas** besoin de connexion
(elle travaille uniquement à partir d'un assembly local).

### Découverte automatique de la configuration

`tables list` et `tables pull` **remontent l'arborescence** depuis le dossier courant jusqu'à
trouver un `Config/xrmFramework.config` : le CLI peut donc être lancé depuis n'importe quel
sous-répertoire de la solution (y compris un `bin/Debug`). `--project-root` court-circuite
cette recherche.

Dans la racine ainsi trouvée, le CLI lit `Directory.Build.props` pour en extraire
`XrmFrameworkCoreProjectName`, ce qui lui donne le répertoire `.table` par défaut :
`<racine>/<ProjetCore>/Definitions`. C'est la même résolution que celle injectée par MSBuild
au DefinitionManager. À défaut, `--tables-dir` devient obligatoire.

> La découverte ne teste que `xrmFramework.config` : `connectionStrings.config` porte des
> secrets et est gitignoré dans les solutions générées, donc absent d'un clone frais. Son
> absence est signalée précisément au moment de la connexion, et non déguisée en
> « configuration introuvable ».

> Le CLI charge ces deux fichiers explicitement (sans dépendre d'un `App.config`
> applicatif) — cf. [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs)
> et [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs).

---

## Commandes

### `xrmframework tables sync` ✅ *(disponible)*

Synchronise les fichiers `.table` d'un répertoire à partir des classes
`[EntityDefinition]` trouvées dans un assembly compilé.

```bash
xrmframework tables sync --dll <chemin.dll> --tables-dir <répertoire> [--clean]
```

| Option | Requis | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Assembly à analyser (contient des classes `*Definition` décorées `[EntityDefinition]` exposant un champ statique `EntityName`). |
| `--tables-dir <DIRECTORY>` | ✅ | Répertoire des fichiers `.table` à créer / mettre à jour. |
| `--clean` | ❌ | Met `Select=false` sur les colonnes orphelines et supprime les `.table` entièrement générés par l'outil sans donnée CRM. |

**Exemple**

```bash
xrmframework tables sync --dll bin/Release/net8.0/MyProject.Plugins.dll \
                         --tables-dir ../MyProject.Core/Definitions \
                         --clean
```

#### Tables livrées par le framework

Les `.table` du package XrmFramework (`SystemUser`, `Role`, `Team`, `SdkMessage`, …) sont
compilés dans le projet consommateur : leurs `*Definition` apparaissent donc dans le DLL
analysé, au même titre que celles du projet. La commande **ne les crée pas** dans le
répertoire cible — ce serait un doublon d'un fichier déjà fourni par le package — et signale
simplement combien ont été ignorées.

En revanche, si le projet **suit déjà sa propre copie** d'une de ces tables (fichier présent
dans le répertoire cible, typiquement pour y déclarer des colonnes supplémentaires à côté de
celles du framework marquées `"Locked": true`), elle est synchronisée comme n'importe quelle
autre : ajout des colonnes manquantes, activation de celles que le code référence, et
de-sélection des orphelines sous `--clean`. Le marqueur `Locked` n'est jamais modifié.

L'inventaire vit dans
[`FrameworkTableCatalog`](../XrmFramework.DeployUtils/TableSync/FrameworkTableCatalog.cs) ; un
test vérifie qu'il correspond exactement aux `.table` de `src/XrmFramework/Definitions`.

**Codes de sortie**

| Code | Signification |
|:---:|---|
| `0` | Succès (y compris « aucune définition trouvée »). |
| `2` | DLL ou répertoire introuvable. |
| `3` | Erreur inattendue (la stack trace est affichée). |
| `1` / `-1` | Erreur de parsing / validation des arguments (Spectre). |

Implémentation : [`TableSyncHelper.Sync`](../XrmFramework.DeployUtils/TableSyncHelper.cs)
→ [`DefinitionAnalyzer`](../XrmFramework.DeployUtils/TableSync/DefinitionAnalyzer.cs)
+ [`TableFileSyncer`](../XrmFramework.DeployUtils/TableSync/TableFileSyncer.cs).

### `xrmframework tables list` ✅ *(disponible)*

Liste les tables de l'environnement sélectionné. La colonne `.table` indique celles déjà
suivies dans le projet — l'information qui manque le plus au moment de choisir quoi récupérer.

```bash
xrmframework tables list [--prefix <préfixe>] [--filter <texte>] [--custom-only] [--project-root <dir>]
```

| Option | Requis | Description |
|---|:---:|---|
| `--prefix <PREFIX>` | ❌ | Ne retient que les tables dont le **nom logique** commence par ce préfixe (ex. `ftp_`). |
| `--filter <TEXT>` | ❌ | Ne retient que les tables dont le nom logique **ou le libellé** contient ce texte. |
| `--custom-only` | ❌ | Ne retient que les tables personnalisées. |
| `--project-root <DIR>` | ❌ | Racine contenant `Config/` (défaut : recherche en remontant depuis le dossier courant). |

**Exemple**

```bash
xrmframework tables list --prefix ftp_
```

Les métadonnées sont récupérées sans les attributs (`EntityFilters.Entity`), ce qui rend la
commande nettement plus rapide qu'une récupération complète.

### `xrmframework tables pull` ✅ *(disponible)*

Génère ou met à jour des fichiers `.table` depuis les métadonnées de l'environnement : types,
libellés localisés, capacités, bornes, relations, clés alternatives et option sets. C'est
l'équivalent headless du **DefinitionManager** (WinForms `net462`), utilisable en CI.

```bash
xrmframework tables pull [--table <noms>] [--prefix <préfixe>] [--tables-dir <dir>] [--project-root <dir>] [-n]
```

| Option | Requis | Description |
|---|:---:|---|
| `-t`, `--table <NAME>` | ❌ | Nom logique d'une table. Option **répétable** et acceptant une liste séparée par des virgules. |
| `--prefix <PREFIX>` | ❌ | Récupère en outre toutes les tables dont le nom logique commence par ce préfixe. |
| `--tables-dir <DIRECTORY>` | ❌ | Répertoire cible (défaut : le `Definitions` du projet Core, déduit de la configuration). |
| `--project-root <DIR>` | ❌ | Racine contenant `Config/` (défaut : recherche en remontant depuis le dossier courant). |
| `-n`, `--noprompt` | ❌ | Mode silencieux : ignore la confirmation (CI/CD). |

#### Sélection par défaut : les tables déjà suivies

Sans `--table` ni `--prefix`, `pull` rafraîchit **toutes les tables déjà décrites par un
`.table`** du répertoire cible — la mise à jour de masse après une évolution du modèle, sans
avoir à réénumérer les tables du projet.

- La sélection est lue dans les fichiers, par leur `LogName` : un `.table` renommé reste suivi.
- `OptionSet.table` (option sets globaux) est exclu — il ne correspond à aucune entité du CRM,
  mais reste alimenté par les tables récupérées.
- Un `.table` dont l'entité n'existe plus dans l'environnement est **signalé et ignoré**, sans
  interrompre les autres ; le fichier n'est pas supprimé.
- Si le répertoire ne contient aucun `.table`, la commande s'arrête avec le code `1` **avant de
  se connecter** : rien à récupérer, autant ne pas authentifier pour rien.

**Exemples**

```bash
xrmframework tables pull --noprompt
```

```bash
xrmframework tables pull --table account,ftp_contrat --noprompt
```

#### Sélection des colonnes

À la **création** d'un `.table`, seules les colonnes directement exploitables sont activées
(`Select: true`) :

- la clé primaire, la colonne de nom et la colonne d'image (`PrimaryType`) ;
- les colonnes participant à une clé alternative ;
- `createdon`, `modifiedon`, `statecode`, `statuscode`.

Toutes les autres colonnes sont bien **écrites avec l'intégralité de leurs métadonnées**, mais
restent inactives : c'est `tables sync` qui les active au fur et à mesure que le code les
référence. Cela évite de générer des milliers de constantes inutiles.

#### Règles de fusion sur un fichier existant

> **Ce qui devient un identifiant C# appartient au fichier ; ce qui décrit la table appartient
> au CRM.**

| Élément | Source retenue |
|---|---|
| `Name` (table, colonne, clé, option set et ses membres) | **le fichier** — renommé à la main, le code compilé en dépend |
| `Select` | **le fichier** — jamais de rétrogradation |
| `Locked` | **le fichier** — marqueur local, absent du CRM |
| `Type`, `PrimaryType`, `Capa`, `Labels`, `StrLen`, `MinRange`, `MaxRange`, `DatBehav`, `IsMultiSelect`, `EnumName`, relations | **le CRM** |

Autres garanties :

- **Une colonne déjà sélectionnée le reste.** `pull` ne rétrograde jamais un `Select: true`, et ne
  réactive jamais une colonne délibérément désactivée — y compris `createdon` et consorts, pourtant
  activées d'office à la création. Cette garantie est vérifiée de bout en bout (métadonnées →
  fusion → écriture → relecture) par `TablePullPersistenceTests`.
- Le fichier cible est retrouvé par son **`LogName`**, pas par son nom de fichier : une table
  dont le `Name` a été renommé à la main (`Contrat.table` → `ContratLocation.table`) est bien
  mise à jour au lieu d'être dupliquée — la sélection survit également à ce renommage.
- Une colonne présente dans le fichier mais **absente de l'environnement** est conservée et
  signalée. `pull` rafraîchit, il ne détruit pas ; la désélection des orphelines relève de
  `tables sync --clean`.
- Les option sets **globaux** sont fusionnés de façon purement additive dans `OptionSet.table` :
  récupérer une seule table ne retire jamais ceux que les autres référencent.
- L'opération est **idempotente** : un second `pull` sur la même table produit une diff vide.

**Codes de sortie** (communs à `list` et `pull`)

| Code | Signification |
|:---:|---|
| `0` | Succès (y compris annulation à la confirmation). |
| `1` | Aucune table ne correspond aux critères. |
| `2` | Configuration ou répertoire introuvable. |
| `3` | Erreur inattendue, ou au moins une table en échec. |
| `-1` | Erreur de validation des arguments (Spectre). |

Implémentation : [`CrmTableHelper`](../XrmFramework.DeployUtils/TableSync/CrmTableHelper.cs)
→ [`ProjectConfigLocator`](../XrmFramework.DeployUtils/TableSync/ProjectConfigLocator.cs)
+ [`MetadataTableFactory`](../XrmFramework.DeployUtils/TableSync/MetadataTableFactory.cs)
+ [`TableMerger`](../XrmFramework.DeployUtils/TableSync/TableMerger.cs)
+ [`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs).

### `xrmframework deploy plugins` ✅ *(disponible)*

Déploie une assembly XrmFramework — **plugins, custom APIs et workflows** — vers
l'environnement sélectionné dans `Config/xrmFramework.config`.

```bash
xrmframework deploy plugins --dll <chemin.dll> --project <nom> [--project-root <dir>] [--on-premise] [--noprompt]
```

| Option | Requis | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Assembly du projet plugin (`net462`, celle enregistrée dans Dataverse). |
| `--project <NAME>` | ✅ | Nom du projet tel que déclaré dans `xrmFramework.config` (ex. `Plugins`). |
| `--project-root <DIR>` | ❌ | Racine contenant le dossier `Config/` (défaut : dossier courant). |
| `--on-premise` | ❌ | Cible un CRM On-Premises (défaut : Dataverse Online). |
| `-n`, `--noprompt` | ❌ | Mode silencieux : ignore la confirmation de connexion (CI/CD). |

> **Comment ça marche — inventaire par exécution réelle du code.** Un plugin est `net462`,
> ce tool est `net10.0` : il ne peut donc pas instancier les types du plugin lui-même. Il délègue
> à l'outil **`XrmFramework.PluginInventory`** (exe `net462`, embarqué sous `inventory/`), qui
> charge l'assembly, **exécute les constructeurs (`AddSteps`)** et reflète les types, puis renvoie
> le manifeste JSON (plugins / steps / workflows / custom APIs) sur sa sortie standard.
>
> Conséquences :
> - L'enregistrement des steps est **entièrement libre** : boucles, conditions, valeurs calculées,
>   configuration… puisque le vrai code s'exécute (aucune contrainte d'analyse statique).
> - Le déploiement requiert le **runtime .NET Framework** (Windows). En développement multiplateforme,
>   un lanceur peut être fourni via la variable `XRMFRAMEWORK_INVENTORY_LAUNCHER` (ex. `mono`) ;
>   `XRMFRAMEWORK_INVENTORY_EXE` permet de pointer un exécutable d'inventaire alternatif.

**Exemple**

```bash
xrmframework deploy plugins --dll bin/Release/net462/MyProject.Plugins.dll \
                            --project MyProject.Plugins \
                            --noprompt
```

**Codes de sortie**

| Code | Signification |
|:---:|---|
| `0` | Succès (ou annulation à la confirmation). |
| `1` | Projet absent de `xrmFramework.config`. |
| `3` | Erreur inattendue (inventaire, connexion, déploiement…). |
| `255` | Erreur de validation des arguments (Spectre). |

Implémentation : [`RegistrationHelper.RegisterPluginsAndWorkflows`](../XrmFramework.DeployUtils/RegistrationHelper.cs)
→ inventaire [`XrmFramework.PluginInventory`](../XrmFramework.PluginInventory/PluginInventoryEngine.cs)
→ [`PluginInventoryReader`](../XrmFramework.DeployUtils/Factories/PluginInventoryReader.cs)
+ [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs).

---

## Roadmap

Arborescence de commandes cible (les ✅ existent, les 🚧 sont à venir) :

```
xrmframework
├── tables
│   ├── sync           ✅  .table ← assembly compilée        (hors ligne)
│   ├── list           ✅  liste les tables de l'environnement (connecté)
│   ├── pull           ✅  .table ← métadonnées Dataverse     (connecté)
│   └── columns        🚧  ajoute / modifie des colonnes d'une ou plusieurs tables
└── deploy
    ├── plugins        ✅  déploie une assembly plugins / custom API / workflow
    └── webresources   🚧  déploie les webresources
```

Les deux directions sont complémentaires : `pull` apporte les métadonnées riches depuis
l'environnement, `sync` active les colonnes que le code référence réellement.

> `deploy plugins` inventorie l'assembly plugin `net462` en **exécutant son code d'enregistrement**
> via l'outil `XrmFramework.PluginInventory` (exe `net462` embarqué) — l'enregistrement des steps
> reste donc totalement libre (boucles, conditions…). Nécessite le runtime .NET Framework (Windows).

### 🚧 `tables columns` — ajouter / modifier des colonnes

Édition manuelle des fichiers `.table` pour activer ou ajuster des colonnes sans passer
ni par un assembly (`tables sync`) ni par l'environnement (`tables pull`). Verbes pressentis
(à figer) : `tables columns add` / `tables columns set`. Réutilisera
[`TableFileStore`](../XrmFramework.DeployUtils/TableSync/TableFileStore.cs) pour la lecture
et l'écriture.


### 🚧 `deploy webresources` — déployer les webresources

Déploie les webresources d'un dossier projet vers l'environnement `SelectedConnection`.
S'appuiera sur [`WebResourceHelper.SyncWebResources`](../XrmFramework.DeployUtils/WebResourceHelper.cs)
(options existantes : `-p/--path`, `-n/--noprompt`).

---

## Architecture & ajout d'une commande

Le CLI suit le modèle Spectre.Console.Cli :

- **`Program.cs`** configure le `CommandApp` et l'arborescence (branches `tables`,
  `deploy`, …).
- **`Commands/`** contient une classe par commande : `Command<TSettings>` avec une
  classe `Settings` (options `[CommandOption]` + `Validate()`), et un `Execute(...)`
  qui **délègue à un helper de `XrmFramework.DeployUtils`** (le CLI ne contient pas
  de logique métier).

Pour ajouter une commande :

1. Créer `Commands/MaCommande.cs` (`Command<Settings>`), `Execute` appelle le helper.
2. L'enregistrer dans `Program.cs` (`AddCommand` / `AddBranch`).
3. Si la logique n'existe pas encore dans `DeployUtils`, l'y ajouter sous forme
   d'**API paramétrée retournant un `int`** (code de sortie), à l'image de
   `TableSyncHelper.Sync(...)` — pas d'`Environment.Exit` dans les helpers.

> ⚠️ Spectre.Console.Cli 0.55 : `Command<T>.Execute` est
> `protected override int Execute(CommandContext, T, CancellationToken)`.
> Dans les textes d'aide/description, échapper les crochets littéraux (`[` → `[[`,
> `]` → `]]`) sinon ils sont interprétés comme du markup de style.

---

## Développement

```bash
# build
dotnet build src/XrmFramework.Cli -c Release

# exécuter sans packager
dotnet run --project src/XrmFramework.Cli -- tables sync --dll <dll> --tables-dir <dir>

# packager le tool localement et l'inspecter
dotnet pack src/XrmFramework.Cli -c Release -o ./nupkg
unzip -p ./nupkg/XrmFramework.Cli.*.nupkg "*.nuspec"     # <packageType name="DotnetTool" />
```

> Le package embarque toute la *closure* de dépendances sous `tools/net10.0/any/`
> (dont `XrmFramework.DeployUtils` et le client Dataverse) : c'est volumineux mais
> nécessaire pour un tool autonome.

La version provient de **Nerdbank.GitVersioning** (pas de numéro à maintenir à la main).
