# Migration de la pipeline Azure DevOps vers GitHub Actions

Ce document décrit la bascule de `NugetPackages-Azure.yml` (Azure Pipelines, projet
`dimsi/XrmFramework`, définition 41) vers GitHub Actions, le code vivant déjà sur
GitHub (`Dimsi-BS/XrmFramework`) avec un flux de PR.

## Pourquoi

- Le code, les PR et les revues sont déjà sur GitHub ; la CI est le dernier aller-retour
  vers Azure DevOps. Les checks obligatoires de PR deviennent natifs.
- Un seul endroit pour les secrets, les permissions et l'historique des builds.
- `pull_request` est déclenché nativement sur les forks, ce qui n'est pas le cas de la
  pipeline Azure actuelle (aucune validation automatique des contributions externes).
- Les runners GitHub hébergés sont gratuits pour un dépôt public (minutes illimitées),
  là où le pool Azure consomme un parallel job.

## Ce qui est livré

| Fichier | Rôle |
| --- | --- |
| `.github/workflows/ci.yml` | Build + tests + pack, portage 1:1 de la pipeline Azure |
| `.github/workflows/release.yml` | Publication sur nuget.org, manuelle et validée par un reviewer |
| `.github/dependabot.yml` | Mise à jour mensuelle des versions d'actions |

## Correspondance étape par étape

| Azure Pipelines | GitHub Actions | Note |
| --- | --- | --- |
| `trigger.branches` (`main`, `v2.*`, `v3.*`) | `on.push.branches` identiques | — |
| `trigger.paths: src` | `on.push.paths` : `src/**` **+** `Directory.Build.props`, `Directory.Packages.props`, `version.json`, le workflow | Correction : un bump de `version.json` (racine) ne déclenchait aucun build |
| *(idem)* | **aucun filtre sur `on.pull_request`** | Voir « Filtres de chemins et checks obligatoires » |
| *(aucun)* | `on.pull_request` | Nouveau : validation des PR, forks inclus |
| `pool.vmImage: windows-latest` | `runs-on: windows-latest` | Conservé, voir « Décisions » |
| `checkout: self` + `fetchDepth: 0` | `actions/checkout` + `fetch-depth: 0` | Indispensable à Nerdbank.GitVersioning |
| 2 × `UseDotNet@2` (8.0.x, 10.0.x) | un seul `actions/setup-dotnet` multi-versions | `includePreviewVersions` inutile : .NET 10 est sorti |
| *(aucun)* | `actions/cache` sur `~/.nuget/packages` | Nouveau : restore mis en cache via `Directory.Packages.props` |
| `powershell:` validation beta sur main | step `pwsh` conditionnée par `if: github.ref == 'refs/heads/main'` | Le test de branche passe de l'intérieur du script à la condition du step |
| `script: dotnet build ... ` | `run:` + `working-directory: src` | Identique |
| `DotNetCoreCLI@2 command: test` + `publishTestResults: true` | `dotnet test` + `actions/upload-artifact` | **Seule perte fonctionnelle**, voir ci-dessous |
| `CopyFiles@2` + `PublishPipelineArtifact@1` | un seul `actions/upload-artifact` (glob direct) | L'étape de copie intermédiaire disparaît |
| *(publication nuget.org hors pipeline)* | `release.yml`, `workflow_dispatch` + environnement `nuget` | Nouveau : la publication devient tracée et reproductible |

## Décisions prises

**`windows-latest` conservé.** Les TFM `net462`/`net48` et le collecteur Microsoft
« Code coverage » restent plus sûrs sous Windows. Un leg Linux (`ubuntu-latest`,
compilation via `Microsoft.NETFramework.ReferenceAssemblies`) pourrait être ajouté plus
tard en matrice pour détecter les régressions de portabilité ; ce n'est pas le sujet de
cette migration.

**Pas de `PublicRelease` forcé.** `publicReleaseRefSpec` dans `version.json` couvre déjà
`refs/heads/main` et `refs/heads/vX.Y`, et NBGV le résout pareil dans les deux steps,
donc un seul `NuGetPackageVersion` par package — exactement l'invariant rétabli par #54.

**Versions produites sur les PR.** Sur un évènement `pull_request`, GitHub positionne
`GITHUB_REF` sur `refs/pull/N/merge` : NBGV n'y reconnaît pas une public release et
suffixe donc la version avec le hash du commit (`3.2.x-beta-g<sha>`). C'est le
comportement souhaité — ces packages ne sont pas publiables — mais il faut le savoir en
comparant un artefact de PR à un artefact de `main`.

**Publication séparée du build.** `release.yml` ne reconstruit rien : il télécharge
l'artefact `Packages` d'un run CI vert et le pousse. Ce qui atterrit sur nuget.org est
donc à l'octet près ce qui a été testé.

Deux entrées :

- **automatique**, quand « Build & Tests » réussit sur `main`, `v2.*` ou `v3.*` ;
- **à la main** (`workflow_dispatch`), pour republier un run plus ancien ou répéter à
  blanc. Sur cette voie, `dry_run` vaut `true` par défaut ; la voie automatique publie
  toujours, un `input` n'existant pas sur un évènement `workflow_run`.

Dans les deux cas l'environnement `nuget` impose son approbation : « automatique »
signifie que la publication est mise en attente d'un clic, jamais qu'elle est
silencieuse. Retirer `environment: nuget` la rendrait réellement non surveillée — à
peser, une version nuget.org ne se supprimant pas, seulement se dé-listant.

## Filtres de chemins et checks obligatoires

`on.pull_request` n'a **aucun filtre `paths`**, et ce n'est pas un oubli.

`Build, test & pack` est un check **obligatoire** sur `main` (ruleset du dépôt). Un
check obligatoire qui ne se déclenche pas est rapporté « Expected — waiting for status »
et **bloque définitivement** la pull request : il n'y a pas d'échec à corriger, juste un
état qui n'arrivera jamais. Avec un filtre `paths`, c'est le sort de toute PR ne touchant
que le README, `docs/`, `.gitignore` ou `release.yml`.

Le cas s'est produit en vrai sur la PR qui a introduit la publication automatique : elle
ne modifiait que `release.yml` et ce document, elle est sortie `BLOCKED` avec zéro check.

Construire ces PR coûte ~3,5 minutes de runner gratuit ; une PR en impasse coûte une
après-midi de perplexité. Le filtre reste en place sur `on.push` : aucun check
obligatoire ne garde un push, donc un merge de documentation seule ne déclenche toujours
ni build ni paquet.

Les deux alternatives, pour mémoire : rendre le check non obligatoire (on perd la
garantie), ou ajouter un second workflow « skip » qui rapporte le même nom de job sur les
chemins complémentaires (fragile, et il faut maintenir deux listes de chemins miroir).

### Pourquoi `workflow_run` et non `push`

Trois raisons, dont deux sont des bugs qu'un trigger `push` aurait introduits :

- sur un `push`, les deux workflows démarrent **en même temps** : le run CI de ce commit
  n'est pas terminé, donc résoudre « le dernier run vert de la branche » publierait les
  paquets du commit **précédent**. `workflow_run` fournit l'id exact du run qui vient de
  réussir ;
- `inputs.dry_run` n'existe pas sur un `push`, donc `!inputs.dry_run` y vaut *vrai* : la
  publication se ferait sans garde-fou ;
- un run CI de pull request a pour `head_branch` la branche source (`ci/...`), qui ne
  correspond à aucun filtre — les builds de PR ne publient donc rien, ce qui est voulu.

**Contrainte à connaître** : comme `schedule`, `workflow_run` ne se déclenche que depuis
la copie du fichier présente sur la **branche par défaut**. C'est donc le `release.yml`
de `main` qui pilote aussi la publication automatique de `v2.*` et `v3.*` ; les copies
vivant sur ces branches ont un bloc `workflow_run` inerte et ne servent que leur propre
`workflow_dispatch`.

Corollaire sur la policy nuget.org : **laissez son filtre de branche vide**. Sur un
évènement `workflow_run`, les claims OIDC décrivent la branche par défaut et non la
branche construite, donc un filtre épinglé sur `v2.2` ou `v3.1` rejetterait les
publications automatiques de ces lignes.

## Affichage des tests et de la couverture

Azure DevOps publiait le `.trx` et rendait l'onglet « Tests » ainsi que le rapport de
couverture (`.coverage`) automatiquement, côté serveur. GitHub Actions n'a aucun
équivalent de première partie : c'est le seul vrai recul fonctionnel de la migration.

**Couverture : traitée.** Le step `Report coverage` convertit les `.coverage` en
Cobertura avec `dotnet-coverage` (outil Microsoft), puis ReportGenerator produit :

- un tableau Markdown ajouté à `$GITHUB_STEP_SUMMARY`, donc le pourcentage est lisible
  directement sur la page du run, sans rien télécharger ;
- un site HTML navigable, embarqué dans l'artefact `TestResults`.

Deux pièges rencontrés en construisant ce step, à ne pas réintroduire :

- `<Format>Cobertura</Format>` dans le runsettings du collecteur Microsoft **n'est pas
  honoré** (vérifié avec le SDK 10.0.100 : le collecteur écrit un `.coverage` binaire
  quoi qu'il arrive). La conversion par `dotnet-coverage merge --output-format cobertura`
  est la route fiable.
- Les outils sont installés en `--tool-path` et non `--global` : le dossier des outils
  globaux n'est pas garanti dans le `PATH` du runner. Les versions sont épinglées, sinon
  la sortie de la CI dérive sans commit.

À noter : la couverture de méthode de ReportGenerator est réservée aux sponsors ; la
couverture de ligne et de branche, celle qui nous intéresse, est libre.

**Tests : traité aussi.** `.github/scripts/Write-TestSummary.ps1` agrège les `.trx`
(15 fichiers : un par projet de test × TFM) en un tableau Markdown dans
`$GITHUB_STEP_SUMMARY`, et déplie chaque test en échec avec son message et sa stack
trace. Les `.trx` restent uploadés en artefact pour le détail complet.

Trois partis pris du script, qui expliquent sa forme :

- **Deux chiffres annoncés, pas un** : les exécutions (5 726 sur le run de référence) et
  les tests distincts (1 468). Le multi-targeting compte un test une fois par TFM ;
  annoncer les seules exécutions laisserait croire à un volume de tests quatre fois
  supérieur à la réalité.
- **Échecs groupés par test, pas par fichier** : un test qui casse sur quatre TFM est un
  bug, pas quatre. Les TFM touchés sont listés dans le titre du bloc.
- **Détail plafonné** (`-MaxFailuresDetailed`, 20 par défaut) : un job summary est limité
  à 1 MiB, et une panne massive le dépasserait. Le tableau, lui, compte tous les échecs.

À savoir si vous reprenez ce script : `GITHUB_STEP_SUMMARY` désigne un **fichier par
step**, concaténés à l'affichage — un step ne peut pas relire ce qu'un autre a écrit.

Option non retenue : **annoter les PR** avec une action tierce (`dorny/test-reporter`,
reporter `dotnet-trx` natif). Épinglée par SHA, elle demande `checks: write` — donc un
droit d'écriture accordé à du code tiers sur le dépôt. À arbitrer si l'affichage dans le
résumé de run ne suffit pas.

Pour un historique, des tendances ou de la détection de tests instables, il n'existe
rien côté GitHub : il faut publier un rapport Allure/CTRF sur GitHub Pages ou brancher
un service externe (Codecov/Coveralls pour la couverture, Datadog Test Optimization ou
Trunk pour les tests).

## Mise en place

1. **Publication nuget.org — Trusted Publishing, aucune clé stockée.** La policy
   nuget.org (menu utilisateur → *Trusted Publishing*) porte le propriétaire du package,
   `Dimsi-BS/XrmFramework`, et le fichier de workflow `release.yml`. Côté GitHub, trois
   choses :

   - une **variable de dépôt `NUGET_USER`** (Settings → Variables) avec le compte
     nuget.org propriétaire de la policy. Une variable, pas un secret : rien de sensible,
     et une valeur erronée doit donner une erreur lisible ;
   - la permission `id-token: write` sur le job, déjà déclarée dans `release.yml` ;
   - l'environnement **`nuget` avec *Required reviewers*** : il ne porte plus de secret,
     il ne sert plus qu'à la validation humaine avant publication.

   Point d'attention sur la policy : son champ *Environment* est optionnel, mais la
   claim n'est vérifiée **que s'il est rempli**. Laissé vide, il n'impose rien ; rempli,
   il doit valoir exactement `nuget`, sinon le login est rejeté alors que le workflow
   tourne bien. La policy peut aussi filtrer la branche — à ne pas restreindre à `main`
   seule si vous publiez depuis les branches `vX.Y`.

   Le délai de 7 jours d'activation d'une nouvelle policy ne s'applique qu'aux dépôts
   privés : XrmFramework étant public, la policy est active immédiatement.
2. **Fusionner cette PR.** Les deux CI tournent alors en parallèle.
3. **Observer 2 à 3 PR et un push sur `main`** et comparer : nombre de `.nupkg`
   produits, versions NBGV identiques, durée de build (le cache NuGet ne sert qu'à
   partir du second run), tests tous exécutés.
4. **Basculer les checks obligatoires** (Settings → Branches → `main`, `v3.*`) sur
   `Build & Tests / Build, test & pack`.
5. **Désactiver la définition Azure 41**, puis supprimer `NugetPackages-Azure.yml` dans
   une PR dédiée et remplacer le badge du README :

   ```markdown
   [![Build & Tests](https://github.com/Dimsi-BS/XrmFramework/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Dimsi-BS/XrmFramework/actions/workflows/ci.yml)
   ```

6. **Porter sur `v3.1`** (et `v2.2` si la ligne est encore maintenue) : la pipeline
   Azure déclenche aussi ces branches, elles ont donc besoin du workflow pour rester
   construites. Branche de feature + worktree depuis `origin/v3.1`, comme d'habitude.

## Limites connues

- Le workflow n'est pas exécutable en local ; le premier run réel sur une branche de
  feature (via `workflow_dispatch`) est la seule validation possible. Les points les
  plus susceptibles de demander un ajustement : la résolution de `--settings` /
  `--results-directory` (chemins absolus `${{ github.workspace }}` sous Windows), et le
  nom exact du collecteur `"Code coverage"` hors contexte VSTest Azure.
- Les samples (`samples/`) ne sont pas construits, comme aujourd'hui.
- Les minutes `windows-latest` comptent ×2 sur un dépôt privé ; sans effet ici si le
  dépôt reste public.
