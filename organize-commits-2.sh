#!/bin/bash
# =============================================================================
# Commits restants — refactoring par librairie
# Lancer depuis la RACINE du projet XrmFramework
# =============================================================================
set -e

REPO_ROOT="$(git rev-parse --show-toplevel)"
cd "$REPO_ROOT"
echo "Dossier racine : $REPO_ROOT"
echo ""

# =============================================================================
echo "==> Commit A : chore — Build, CI et configuration"
# =============================================================================
git add .gitignore
git add .vscode/launch.json
git add .vscode/tasks.json
git add Directory.Build.props
git add LICENSE.txt
git add NugetPackages-Azure.yml
git add version.json
git add src/.gitignore
git add src/Build/Versioning.targets
git add src/Directory.Build.props
git add src/Directory.Build.targets
git add src/LICENSE.txt
git add src/NuGet.config
git commit -m "chore: update build configuration, CI pipeline and .gitignore"

# =============================================================================
echo "==> Commit B : docs — README et CODE_OF_CONDUCT"
# =============================================================================
git add README.md
git add CODE_OF_CONDUCT.md
git commit -m "docs: update README and CODE_OF_CONDUCT"

# =============================================================================
echo "==> Commit C : refactor — XrmFramework.Core"
# =============================================================================
git add src/XrmFramework.Core/
git commit -m "refactor: update XrmFramework.Core"

# =============================================================================
echo "==> Commit D : refactor — XrmFramework (librairie principale)"
# =============================================================================
git add src/XrmFramework/
git commit -m "refactor: update main XrmFramework library"

# =============================================================================
echo "==> Commit E : refactor — XrmFramework.Analyzers"
# =============================================================================
git add src/XrmFramework.Analyzers/
git commit -m "refactor: update XrmFramework.Analyzers"

# =============================================================================
echo "==> Commit F : refactor — XrmFramework.MSBuild.Generation"
# =============================================================================
git add src/XrmFramework.MSBuild.Generation/
git commit -m "refactor: update XrmFramework.MSBuild.Generation"

# =============================================================================
echo "==> Commit G : refactor — XrmFramework.DeployUtils"
# =============================================================================
git add src/XrmFramework.DeployUtils/
git commit -m "refactor: update XrmFramework.DeployUtils"

# =============================================================================
echo "==> Commit H : refactor — XrmFramework.Plugin"
# =============================================================================
git add src/XrmFramework.Plugin/
git commit -m "refactor: update XrmFramework.Plugin"

# =============================================================================
echo "==> Commit I : refactor — XrmFramework.RemoteDebugger.Client"
# =============================================================================
git add src/XrmFramework.RemoteDebugger.Client/
git commit -m "refactor: update XrmFramework.RemoteDebugger.Client"

# =============================================================================
echo "==> Commit J : refactor — XrmFramework.DefinitionManager"
# =============================================================================
git add src/XrmFramework.DefinitionManager/
git commit -m "refactor: update XrmFramework.DefinitionManager"

# =============================================================================
echo "==> Commit K : refactor — XrmFramework.CoreProject et OnPremise"
# =============================================================================
git add src/XrmFramework.CoreProject/
git add src/XrmFramework.CoreProject.OnPremise/
git commit -m "refactor: update XrmFramework.CoreProject and OnPremise variant"

# =============================================================================
echo "==> Commit L : refactor — XrmFramework.Templates"
# =============================================================================
git add src/XrmFramework.Templates/
git commit -m "refactor: update XrmFramework.Templates"

# =============================================================================
echo "==> Commit M : refactor — XrmFramework.TypeScript"
# =============================================================================
git add src/XrmFramework.TypeScript/
git commit -m "refactor: update XrmFramework.TypeScript"

# =============================================================================
echo "==> Commit N : test — Mise à jour des projets de tests existants"
# =============================================================================
git add src/Tests/
git commit -m "test: update existing test projects"

# =============================================================================
echo ""
echo "Tous les commits ont ete crees !"
echo ""
git log --oneline -20
