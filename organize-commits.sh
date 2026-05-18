#!/bin/bash
# =============================================================================
# Script d'organisation des commits par thème/fonctionnalité
# À exécuter depuis la racine du projet XrmFramework
# =============================================================================
set -e

echo "==> Commit 1 : Migration solution .sln → .slnx (déjà stagé)"
git commit -m "chore: migrate solution from .sln to .slnx format"

# =============================================================================
echo "==> Commit 2 : Support .NET 10.0"
# =============================================================================
git add \
  "src/XrmFramework.CoreProject/build/net10.0/" \
  "src/XrmFramework.CoreProject/lib/net10.0/" \
  "src/XrmFramework.DeployUtils/build/net10.0/" \
  "src/XrmFramework/build/net10.0/" \
  "src/XrmFramework/lib/net10.0/" \
  "src/XrmFramework/build/net8.0/XrmFramework.props" \
  "src/Directory.Build.props" \
  "src/Directory.Build.targets" \
  "src/Build/Versioning.targets" \
  "Directory.Build.props"
git commit -m "feat: add .NET 10.0 target framework support"

# =============================================================================
echo "==> Commit 3 : Nouveau module XrmFramework.TypeScript"
# =============================================================================
git add \
  "src/XrmFramework.TypeScript/GenerateTsFilesTask.cs" \
  "src/XrmFramework.TypeScript/Models.cs" \
  "src/XrmFramework.TypeScript/TsGenerator.cs" \
  "src/XrmFramework.TypeScript/XrmFramework.TypeScript.csproj" \
  "src/XrmFramework.TypeScript/XrmFramework.TypeScript.nuspec" \
  "src/XrmFramework.TypeScript/build/" \
  "src/XrmFramework.TypeScript/lib/" \
  "src/XrmFramework.TypeScript/tsconfig.json"
git commit -m "feat: add XrmFramework.TypeScript code generation module"

# =============================================================================
echo "==> Commit 4 : Utilitaires de mapping BindingModel"
# =============================================================================
git add \
  "src/XrmFramework/BindingModel/Utils/BindingModelQueryBuilder.cs" \
  "src/XrmFramework/BindingModel/Utils/BindingModelToEntityMapper.cs" \
  "src/XrmFramework/BindingModel/Utils/BindingModelUpsertExecutor.cs" \
  "src/XrmFramework/BindingModel/Utils/DtoBindingModelMapper.cs" \
  "src/XrmFramework/BindingModel/Utils/EntityToBindingModelMapper.cs" \
  "src/XrmFramework/BindingModel/Utils/XmlBindingModelMapper.cs"
git commit -m "feat: add BindingModel mapper and upsert utilities"

# =============================================================================
echo "==> Commit 5 : Abstraction IDateTimeProvider"
# =============================================================================
git add \
  "src/XrmFramework/Context/FixedDateTimeProvider.cs" \
  "src/XrmFramework/Context/IDateTimeProvider.cs" \
  "src/XrmFramework/Context/IXrmFrameworkService.cs" \
  "src/XrmFramework/Context/InternalDependencyProvider.DateTimeProvider.cs" \
  "src/XrmFramework/Context/SystemDateTimeProvider.cs"
git commit -m "feat: add IDateTimeProvider abstraction for testable time handling"

# =============================================================================
echo "==> Commit 6 : Gestion des sessions de test de plugins"
# =============================================================================
git add \
  "src/XrmFramework.Plugin/RemoteDebugger/PluginTestSession.cs" \
  "src/XrmFramework.RemoteDebugger.Client/ConsoleUI/" \
  "src/XrmFramework.RemoteDebugger.Client/PluginTestRunner.cs" \
  "src/XrmFramework.RemoteDebugger.Client/PluginTestSessionRecorder.cs" \
  "src/XrmFramework.RemoteDebugger.Generator/"
git commit -m "feat: add plugin test session management, recorder and code generator"

# =============================================================================
echo "==> Commit 7 : Nouveau module XrmFramework.LogicApp"
# =============================================================================
git add "src/XrmFramework.LogicApp/"
git commit -m "feat: add XrmFramework.LogicApp module for Logic App building"

# =============================================================================
echo "==> Commit 8 : Nouveaux projets de tests"
# =============================================================================
git add \
  "src/Tests/XrmFramework.BindingModel.Tests/" \
  "src/Tests/XrmFramework.DeployUtils.Tests/" \
  "src/Tests/XrmFramework.Generators.Tests/" \
  "src/Tests/XrmFramework.LogicApp.Tests/" \
  "src/Tests/XrmFramework.Plugin.Tests/"
git commit -m "test: add new test projects (BindingModel, DeployUtils, Generators, LogicApp, Plugin)"

# =============================================================================
echo "==> Commit 9 : Projet sample"
# =============================================================================
git add "samples/"
git commit -m "feat: add XrmFramework sample solution"

# =============================================================================
echo "==> Commit 10 : Documentation"
# =============================================================================
git add \
  "README.md" \
  "CODE_OF_CONDUCT.md" \
  "XrmFramework_Pitch.docx" \
  "docs/CustomApis.md" \
  "docs/IService-Architecture.md" \
  "docs/IService-Architecture.docx" \
  "docs/Plugins.md" \
  "docs/RemoteDebugger.md" \
  "docs/WorkingWithServices.md" \
  "docs/XrmFrameworkUtilities.md"
git commit -m "docs: update and add documentation (CustomApis, Plugins, architecture)"

# =============================================================================
echo "==> Commit 11 : Infrastructure build & CI"
# =============================================================================
git add \
  ".gitignore" \
  "src/.gitignore" \
  "NugetPackages-Azure.yml" \
  ".vscode/launch.json" \
  ".vscode/tasks.json" \
  "src/NuGet.config" \
  "LICENSE.txt" \
  "src/LICENSE.txt" \
  "version.json"
git commit -m "chore: update build configuration, CI pipeline and .gitignore"

# =============================================================================
echo "==> Commit 12 : Refactoring XrmFramework.Core"
# =============================================================================
git add "src/XrmFramework.Core/"
git commit -m "refactor: update XrmFramework.Core"

# =============================================================================
echo "==> Commit 13 : Refactoring librairie principale XrmFramework"
# =============================================================================
git add "src/XrmFramework/"
git commit -m "refactor: update main XrmFramework library"

# =============================================================================
echo "==> Commit 14 : Refactoring XrmFramework.Analyzers"
# =============================================================================
git add "src/XrmFramework.Analyzers/"
git commit -m "refactor: update XrmFramework.Analyzers"

# =============================================================================
echo "==> Commit 15 : Refactoring XrmFramework.MSBuild.Generation"
# =============================================================================
git add "src/XrmFramework.MSBuild.Generation/"
git commit -m "refactor: update XrmFramework.MSBuild.Generation"

# =============================================================================
echo "==> Commit 16 : Refactoring XrmFramework.DeployUtils"
# =============================================================================
git add "src/XrmFramework.DeployUtils/"
git commit -m "refactor: update XrmFramework.DeployUtils"

# =============================================================================
echo "==> Commit 17 : Refactoring XrmFramework.Plugin"
# =============================================================================
git add "src/XrmFramework.Plugin/"
git commit -m "refactor: update XrmFramework.Plugin"

# =============================================================================
echo "==> Commit 18 : Refactoring XrmFramework.RemoteDebugger.Client"
# =============================================================================
git add "src/XrmFramework.RemoteDebugger.Client/"
git commit -m "refactor: update XrmFramework.RemoteDebugger.Client"

# =============================================================================
echo "==> Commit 19 : Refactoring XrmFramework.DefinitionManager"
# =============================================================================
git add "src/XrmFramework.DefinitionManager/"
git commit -m "refactor: update XrmFramework.DefinitionManager"

# =============================================================================
echo "==> Commit 20 : Refactoring XrmFramework.CoreProject et OnPremise"
# =============================================================================
git add \
  "src/XrmFramework.CoreProject/" \
  "src/XrmFramework.CoreProject.OnPremise/"
git commit -m "refactor: update XrmFramework.CoreProject and OnPremise variant"

# =============================================================================
echo "==> Commit 21 : Refactoring XrmFramework.Templates"
# =============================================================================
git add "src/XrmFramework.Templates/"
git commit -m "refactor: update XrmFramework.Templates"

# =============================================================================
echo "==> Commit 22 : Mise à jour des projets de tests existants"
# =============================================================================
git add "src/Tests/"
git commit -m "test: update existing test projects"

# =============================================================================
echo ""
echo "✅ Tous les commits ont été créés avec succès !"
git log --oneline -25
