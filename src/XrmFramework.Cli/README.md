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
configuration du projet, via deux fichiers (mécanisme XrmFramework existant) lus dans
le dossier **`Config/`** de la racine du projet (`--project-root`, défaut : dossier courant) :

| Fichier | Rôle |
|---|---|
| `Config/xrmFramework.config` | Déclare les projets et la connexion active (`selectedConnection`). |
| `Config/connectionStrings.config` | Définit les chaînes de connexion nommées (Dataverse / On-Premises). |

`selectedConnection` pointe vers une entrée de `connectionStrings.config` : c'est
**l'environnement cible** des commandes `deploy`. La commande `tables sync`, elle,
n'a **pas** besoin de connexion (elle travaille uniquement à partir d'un assembly local).

> Le CLI charge ces deux fichiers explicitement (sans dépendre d'un `App.config`
> applicatif) — cf. [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs).

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

### `xrmframework deploy plugins` ✅ *(disponible)*

Déploie une assembly XrmFramework — **plugins, custom APIs et workflows** — vers
l'environnement sélectionné dans `Config/xrmFramework.config`.

```bash
xrmframework deploy plugins --dll <chemin.dll> --project <nom> [--project-root <dir>] [--on-premise] [--noprompt]
```

| Option | Requis | Description |
|---|:---:|---|
| `--dll <PATH>` | ✅ | Assembly **net8.0** du projet plugin à déployer. |
| `--project <NAME>` | ✅ | Nom du projet tel que déclaré dans `xrmFramework.config` (ex. `Plugins`). |
| `--project-root <DIR>` | ❌ | Racine contenant le dossier `Config/` (défaut : dossier courant). |
| `--on-premise` | ❌ | Cible un CRM On-Premises (défaut : Dataverse Online). |
| `-n`, `--noprompt` | ❌ | Mode silencieux : ignore la confirmation de connexion (CI/CD). |

> ⚠️ **`--dll` doit pointer l'asset net8.0** du projet plugin. Les projets plugin sont
> aujourd'hui `net462` ; leur multi-ciblage `net8.0` (pour le chargement par ce tool net8.0)
> est en cours (cf. [Roadmap](#roadmap)). L'asset net8.0 sert uniquement à l'outillage —
> Dataverse exécute toujours le build net462.

**Exemple**

```bash
xrmframework deploy plugins --dll bin/Release/net8.0/MyProject.Plugins.dll \
                            --project MyProject.Plugins \
                            --noprompt
```

**Codes de sortie**

| Code | Signification |
|:---:|---|
| `0` | Succès (ou annulation à la confirmation). |
| `1` | Projet absent de `xrmFramework.config`. |
| `3` | Erreur inattendue (connexion, déploiement…). |
| `255` | Erreur de validation des arguments (Spectre). |

Implémentation : [`RegistrationHelper.RegisterPluginsAndWorkflows`](../XrmFramework.DeployUtils/RegistrationHelper.cs)
+ [`ConfigHelper.UseProjectConfig`](../XrmFramework.DeployUtils/Configuration/ConfigHelper.cs).

---

## Roadmap

Arborescence de commandes cible (les ✅ existent, les 🚧 sont à venir) :

```
xrmframework
├── tables
│   ├── sync           ✅  synchronise les .table depuis un assembly
│   └── columns        🚧  ajoute / modifie des colonnes d'une ou plusieurs tables
└── deploy
    ├── plugins        ✅  déploie une assembly plugins / custom API / workflow
    └── webresources   🚧  déploie les webresources
```

> Pré-requis 🚧 pour `deploy plugins` : multi-ciblage **net8.0** des projets/packages
> plugin (aujourd'hui `net462`), pour que ce tool net8.0 charge nativement l'assembly.
> Lot de travail séparé, en cours.

### 🚧 `tables columns` — ajouter / modifier des colonnes

Édition des fichiers `.table` pour ajouter ou mettre à jour des colonnes sur une
ou plusieurs tables (sans repasser par la génération complète). Verbes pressentis
(à figer) : `tables columns add` / `tables columns set`. Réutilisera la couche
d'écriture de [`TableFileSyncer`](../XrmFramework.DeployUtils/TableSync/TableFileSyncer.cs).


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
