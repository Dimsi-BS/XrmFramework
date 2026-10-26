# CI GitHub Actions — ligne stable v3.1

Le document complet de la migration — mapping Azure → Actions, décisions, pièges,
publication nuget.org, plan de bascule — vit sur **`main`**, dans ce même fichier.
Celui-ci ne couvre que ce qui est **spécifique à `v3.1`**, et reste volontairement court :
une copie intégrale du document de `main` dérive dès que `main` évolue, ce qui est
exactement ce qui s'est produit entre la mise en place et le premier réalignement.

Les workflows, eux, vivent sur chaque branche construite : pour un push sur `v3.1`,
GitHub lit `ci.yml` **depuis `v3.1`**, jamais depuis `main`. D'où leur présence ici.

## Fichiers identiques à ceux de `main`

- `.github/workflows/ci.yml`
- `.github/workflows/release.yml`
- `.github/scripts/Write-TestSummary.ps1`

Au bit près, volontairement : `git diff origin/main -- .github/workflows .github/scripts`
doit rester vide. Un portage ultérieur se résume alors à copier les fichiers de `main`.
Deux conséquences à connaître :

- la liste de branches des triggers mentionne encore `main` et `v2.*`, sans effet ici —
  seul `v3.*` peut correspondre à un évènement sur cette branche ;
- le step « Validate beta version on main » est conditionné à
  `github.ref == 'refs/heads/main'` : il est donc toujours *skipped* sur `v3.1`, ce qui
  est correct puisque cette branche porte une version stable (`v3.1`, sans `-beta`).

## Versionnage

`publicReleaseRefSpec` couvre `^refs/heads/v\d+\.\d+`, donc NBGV résout `PublicRelease`
à `true` sur cette branche sans qu'on ait à le forcer — même situation que sur `main`.
Vérifié : un push sur `v3.1` produit bien `3.1.144` et non `3.1.144-g<sha>`.

## Publication automatique : pilotée depuis `main`

`workflow_run` ne se déclenche que depuis la copie du fichier présente sur la **branche
par défaut**, comme `schedule`. C'est donc le `release.yml` de `main` qui publie
automatiquement cette ligne quand son « Build & Tests » réussit ; le bloc `workflow_run`
de la copie ci-présente est **inerte**, et elle ne sert que son propre
`workflow_dispatch` (republier un run précis, ou répéter à blanc).

Ne « corrigez » pas cette inertie : c'est le comportement de GitHub, pas une omission.

## `dependabot.yml` absent

Volontaire : Dependabot ne lit sa configuration que sur la branche par défaut. Pour
couvrir `v3.1`, il faut une entrée supplémentaire avec `target-branch: v3.1` dans le
fichier de `main`.
