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

**Publication séparée et non automatique.** `release.yml` ne reconstruit rien : il
télécharge l'artefact `Packages` d'un run CI vert et le pousse. Ce qui atterrit sur
nuget.org est donc l'octet près ce qui a été testé. Le mode `dry_run` est activé par
défaut, et l'environnement `nuget` impose une approbation.

## Perte fonctionnelle à arbitrer : l'affichage des tests

Azure DevOps publiait le `.trx` et rendait l'onglet « Tests » ainsi que le rapport de
couverture (`.coverage`) automatiquement. GitHub Actions n'a pas d'équivalent de
première partie : `ci.yml` se contente donc d'uploader les `.trx` et les `.coverage` en
artefacts (les échecs restent lisibles dans le log du step `Test`). Trois options, par
ordre de coût croissant :

1. **Statu quo** (livré) : artefacts téléchargeables, zéro dépendance tierce.
2. **Annoter les PR** avec une action tierce lisant le `.trx`
   (`dorny/test-reporter`, `EnricoMi/publish-unit-test-result-action`). Épinglées par
   SHA, elles demandent `checks: write` — donc un droit d'écriture accordé à du code
   tiers sur le dépôt. À arbitrer.
3. **Passer la couverture à Cobertura** pour la rendre affichable dans le résumé de job
   et en commentaire de PR. `coverlet.collector` est déjà référencé par tous les projets
   de test, donc `--collect "XPlat Code Coverage"` fonctionne — mais les filtres de
   `src/coverage.runsettings` sont écrits pour le collecteur Microsoft et devront être
   retranscrits dans le bloc `XPlat code coverage` (filtres `[XrmFramework*]*` /
   `ExcludeByFile`). À valider sur une branche jetable avant de toucher au runsettings.

## Mise en place

1. **Secrets / environnement** (Settings → Environments) : créer l'environnement
   `nuget`, y ajouter `NUGET_API_KEY` (clé nuget.org limitée au glob `XrmFramework*`)
   et activer *Required reviewers*. À terme, le *Trusted Publishing* de nuget.org
   (OIDC, sans clé stockée) est l'évolution à viser — à vérifier dans la doc nuget.org
   avant de remplacer la clé.
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
