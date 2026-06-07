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

Les commandes de **déploiement** ciblent l'environnement *sélectionné* dans la
configuration du projet, via deux fichiers (mécanisme XrmFramework existant) :

| Fichier | Rôle |
|---|---|
| `xrmFramework.config` | Déclare les projets et la connexion active (`SelectedConnection`). |
| `connectionStrings.config` | Définit les chaînes de connexion nommées (Dataverse / On-Premises). |

`SelectedConnection` pointe vers une entrée de `connectionStrings.config` : c'est
**l'environnement cible** des commandes `deploy`. La commande `tables sync`, elle,
n'a **pas** besoin de connexion (elle travaille uniquement à partir d'un assembly local).

> Note : la résolution de configuration repose aujourd'hui sur `ConfigurationManager`.
> Pour un tool exécuté hors du dossier projet, prévoir le passage du dossier de config
> (cf. [Roadmap](#roadmap)).

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

---

## Roadmap

Arborescence de commandes cible (les ✅ existent, les 🚧 sont à venir) :

```
xrmframework
├── tables
│   ├── sync           ✅  synchronise les .table depuis un assembly
│   └── columns        🚧  ajoute / modifie des colonnes d'une ou plusieurs tables
└── deploy
    ├── plugins        🚧  déploie une assembly plugins / custom API / workflow
    └── webresources   🚧  déploie les webresources
```

### 🚧 `tables columns` — ajouter / modifier des colonnes

Édition des fichiers `.table` pour ajouter ou mettre à jour des colonnes sur une
ou plusieurs tables (sans repasser par la génération complète). Verbes pressentis
(à figer) : `tables columns add` / `tables columns set`. Réutilisera la couche
d'écriture de [`TableFileSyncer`](../XrmFramework.DeployUtils/TableSync/TableFileSyncer.cs).

### 🚧 `deploy plugins` — déployer une assembly plugins / custom API / workflow

Déploie/enregistre les steps de plugins, custom APIs et workflows d'une assembly
vers l'environnement `SelectedConnection`. S'appuiera sur
[`RegistrationHelper.RegisterPluginsAndWorkflows`](../XrmFramework.DeployUtils/RegistrationHelper.cs).

> Adaptation CLI nécessaire : la signature actuelle prend l'assembly via un
> paramètre générique `<TPlugin>` (résolution à la compilation). En CLI, l'assembly
> sera chargée **par chemin** (`--dll`, comme `tables sync`) + un nom de projet
> (`--project`, tel que déclaré dans `xrmFramework.config`). Prévoir aussi
> `-n/--noprompt` (déjà géré par `DeployCommandOptions`) et la localisation des
> fichiers de config (`--config` / dossier courant).

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

> Le package embarque toute la *closure* de dépendances sous `tools/net8.0/any/`
> (dont `XrmFramework.DeployUtils` et le client Dataverse) : c'est volumineux mais
> nécessaire pour un tool autonome.

La version provient de **Nerdbank.GitVersioning** (pas de numéro à maintenir à la main).
