# CI GitHub Actions — ligne de maintenance v2.2

Portage de la pipeline Azure de cette branche (`NugetPackages-Azure.yml`) vers GitHub
Actions. Le document complet de la migration, avec le mapping détaillé et le plan de
bascule, est sur `main` ; celui-ci ne couvre que ce qui est **spécifique à v2.2**.

Les workflows vivent sur chaque branche construite : pour un push sur `v2.2`, GitHub lit
`ci.yml` **depuis `v2.2`**, jamais depuis `main`. D'où cette copie.

## Ce n'est pas une copie de `main`

La pipeline Azure de cette branche diffère de celle de `main` sur trois points, et
chacun casserait quelque chose s'il était porté en aveugle.

### 1. `/p:PublicRelease=true` est obligatoire ici

C'est le point important. Le `version.json` de cette branche déclare :

```json
"publicReleaseRefSpec": [ "^refs/heads/master$", "^refs/tags/v\\d+\\.\\d+" ]
```

Il ne couvre donc **pas** `refs/heads/v2.2`. Sans le commutateur, NBGV considère un build
de cette branche comme une préversion privée et estampille chaque paquet
`2.2.x-g<sha>` au lieu de `2.2.x`. La pipeline Azure le forçait déjà ; le workflow le
garde.

Le workflow de `main` omet ce commutateur **volontairement** — son propre refspec couvre
`^refs/heads/v\d+\.\d+`, donc NBGV y résout `PublicRelease` tout seul. Copier le fichier
de `main` sur cette branche changerait silencieusement la version de tous les paquets.

### 2. Aucun test

La pipeline Azure de cette branche n'a jamais exécuté les tests. Une ligne de
maintenance n'est pas l'endroit pour les activer à l'aveugle : si vous voulez les
lancer ici, c'est un changement à faire et valider séparément, pas un effet de bord du
portage de la CI.

### 3. Aucune couverture

`src/coverage.runsettings` n'existe pas sur cette branche — et sans run de test, il n'y
aurait rien à collecter. Par conséquent `Write-TestSummary.ps1` n'est pas copié ici non
plus : il n'aurait rien à résumer.

## Autres écarts, mineurs

- Le workflow s'appelle **`Build`** et non « Build & Tests » : il ne teste rien, le nom
  doit le dire. Le nom de fichier reste `ci.yml`, et le check obligatoire à cocher dans
  la protection de branche est donc `Build & pack` (le nom du job).

  À savoir, parce que c'est déroutant : **GitHub indexe les workflows par chemin de
  fichier, pas par branche.** `.github/workflows/ci.yml` est donc une seule entité pour
  tout le dépôt, et son nom affiché dans la barre latérale Actions vient de la branche
  par défaut. Les runs de `v2.2` apparaissent par conséquent regroupés sous « Build &
  Tests », alors que chaque run porte bien le nom `Build` sur sa propre page. Rien n'est
  cassé : c'est le même fichier `ci.yml` de cette branche qui s'exécute. Corollaire
  utile : pour distinguer les checks obligatoires entre branches, fiez-vous au **nom du
  job**, pas à celui du workflow.
- Les triggers ne listent que `v2.*`. La pipeline Azure listait aussi `master`, ce qui
  n'a aucun effet dans un fichier vivant sur `v2.2` : pour un push sur `master`, GitHub
  lit les workflows de `master`.
- Les SDK .NET 8 et 10 sont installés explicitement. Cette branche cible
  `net462;net48;net8.0`, et le format `.slnx` exige un SDK ≥ 9.0.200 pour être lu. La
  pipeline Azure se reposait sur ce que l'image embarquait ; épingler rend le build
  déterministe.
- `dependabot.yml` n'est pas copié : Dependabot ne lit sa configuration que sur la
  branche par défaut. Pour couvrir `v2.2`, il faut une entrée `target-branch: v2.2` dans
  le fichier de `main`.

## `release.yml`

Identique à celui de `main`, y compris le Trusted Publishing. Il se lance à la main
(`workflow_dispatch`) et republie l'artefact `Packages` d'un build vert de **cette**
branche. La policy nuget.org porte sur le fichier `release.yml`, donc elle couvre déjà
cette copie — vérifiez simplement que son éventuel filtre de branche n'est pas limité à
`main`.
