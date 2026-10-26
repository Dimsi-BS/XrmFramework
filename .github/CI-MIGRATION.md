# CI GitHub Actions — ligne de maintenance v2.2

Portage de la pipeline Azure de cette branche (`NugetPackages-Azure.yml`) vers GitHub
Actions. Le document complet de la migration, avec le mapping détaillé et le plan de
bascule, est sur `main` ; celui-ci ne couvre que ce qui est **spécifique à v2.2**.

Les workflows vivent sur chaque branche construite : pour un push sur `v2.2`, GitHub lit
`ci.yml` **depuis `v2.2`**, jamais depuis `main`. D'où cette copie.

## `ci.yml` est identique à celui de `main`

Au bit près, sans substitution : cette branche utilise déjà `src/XrmFramework.slnx`,
comme `main`, et n'a pas de `.sln`. Garder les deux fichiers identiques rend chaque
portage ultérieur trivial — un `git show origin/main:.github/workflows/ci.yml` suffit.

Deux changements ont été nécessaires sur la branche pour y arriver.

### 1. `publicReleaseRefSpec` couvre maintenant les branches `vX.Y`

C'est ce qui permet de supprimer le `/p:PublicRelease=true` que forçait la pipeline
Azure. L'entrée ajoutée :

```json
"publicReleaseRefSpec": [
  "^refs/heads/master$",
  "^refs/heads/v\\d+\\.\\d+",   // ← ajoutée
  "^refs/tags/v\\d+\\.\\d+"
]
```

Sans elle, NBGV traitait un build de `v2.2` comme une préversion privée et estampillait
les paquets `2.2.x-g<sha>` au lieu de `2.2.x` ; la pipeline Azure compensait avec le
commutateur. Le refspec le résout maintenant à la source, comme sur `main` et `v3.1`.

`^refs/heads/master$` et `^refs/tags/v\d+\.\d+` sont **conservés** volontairement : le
premier ne coûte rien, le second préserve le comportement des builds par tag, qui
produisent eux aussi une version publique sur cette ligne. `main` a abandonné l'entrée
de tag ; la retirer ici changerait la version des builds de tag sans raison.

**Conséquence à connaître** : un build de **pull request** produit désormais une version
suffixée `-g<sha>`, car `GITHUB_REF` vaut alors `refs/pull/N/merge`, qu'aucun refspec ne
couvre. C'est le comportement de `main` et de `v3.1`, et c'est voulu — un artefact de PR
n'a pas à porter un numéro publiable. La version propre `2.2.x` revient dès le push sur
`v2.2`.

### 2. Les tests tournent maintenant sur cette branche

La pipeline Azure de `v2.2` ne lançait **aucun test**. Comme `ci.yml` est désormais
identique à celui de `main`, les deux projets de test de la solution
(`XrmFramework.Analyzers.Tests` et `XrmFramework.Tests`) sont exécutés, avec la
couverture et le résumé dans le résumé de run.

C'est un **changement de comportement réel**, pas un simple portage : la CI de cette
branche peut désormais échouer sur un test qui n'était jamais exécuté. Deux fichiers ont
été ajoutés pour cela, copiés de `main` :

- `src/coverage.runsettings`, qui cantonne le collecteur aux assemblies `XrmFramework*`.
  Ses deux exclusions de sources portent sur `XrmFramework.Plugin/NewtonSoft.Json` et
  `XrmFramework/Utils/BoDi.cs` — vérifié, ces deux chemins existent bien sur `v2.2`,
  les règles sont donc actives et non inertes ;
- `.github/scripts/Write-TestSummary.ps1`, inchangé.

## Autres points

- **GitHub indexe les workflows par chemin de fichier, pas par branche.**
  `.github/workflows/ci.yml` est une seule entité pour tout le dépôt, et son nom affiché
  dans la barre latérale Actions vient de la branche par défaut. Les runs de `v2.2`
  apparaissent donc regroupés avec ceux de `main`. Rien n'est cassé : c'est bien le
  `ci.yml` de cette branche qui s'exécute. Corollaire : pour les checks obligatoires,
  fiez-vous au **nom du job**, pas à celui du workflow. Il est ici
  `Build, test & pack`, comme sur `main` et `v3.1`.
- Le step « Validate beta version on main » est conditionné à
  `github.ref == 'refs/heads/main'` : toujours *skipped* ici.
- **`on.pull_request` n'a aucun filtre `paths`**, et c'est ce qui rend cette branche
  sûre si vous étendez le ruleset du dépôt à `v2.*`. Un check obligatoire qui ne se
  déclenche pas n'échoue pas : il reste « Expected — waiting for status » et bloque la
  PR définitivement. Avec un filtre, ce serait le sort de toute PR ne touchant que de la
  documentation. Le filtre reste en place sur `on.push`.
- `dependabot.yml` n'est pas copié : Dependabot ne lit sa configuration que sur la
  branche par défaut. Pour couvrir `v2.2`, il faut une entrée `target-branch: v2.2` dans
  le fichier de `main`.

## `release.yml` : publication pilotée depuis `main`

Identique à celui de `main`, Trusted Publishing compris, et il doit le rester —
`git diff origin/main -- .github/workflows .github/scripts` doit être vide.

**Son bloc `workflow_run` est inerte ici.** Comme `schedule`, `workflow_run` ne se
déclenche que depuis la copie du fichier présente sur la **branche par défaut** : c'est
donc le `release.yml` de `main` qui publie automatiquement cette ligne quand son
« Build & Tests » réussit. La copie ci-présente ne sert que son propre
`workflow_dispatch` — republier un run précis, ou répéter à blanc. Ne « corrigez » pas
cette inertie, c'est le comportement de GitHub et non une omission.

Le workflow est découpé en deux jobs, `resolve` puis `publish`, pour que la version
apparaisse dans le nom du second (`Publish 2.2.110 to nuget.org`) et donc sur l'écran
d'approbation. Le détail du pourquoi est dans le document de `main`.

La policy nuget.org porte sur le fichier `release.yml`, elle couvre donc déjà cette
copie. Son filtre de branche doit rester **vide** : sur un `workflow_run`, les claims
OIDC décrivent la branche par défaut et non la branche construite.
