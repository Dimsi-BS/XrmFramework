// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

namespace XrmFramework.RemoteDebugger.Generator
{
    /// <summary>
    /// Générateur de source Roslyn qui lit les fichiers <c>.pluginsession.json</c>
    /// enregistrés lors des sessions de débogage distant et génère automatiquement
    /// des méthodes de tests unitaires xUnit avec Verify.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Pour activer ce générateur dans un projet de test, ajoutez :
    /// <code>
    /// &lt;ItemGroup&gt;
    ///   &lt;!-- Référencer le générateur comme analyzer --&gt;
    ///   &lt;ProjectReference Include="..\XrmFramework.RemoteDebugger.Generator\..."
    ///                     OutputItemType="Analyzer"
    ///                     ReferenceOutputAssembly="false" /&gt;
    ///
    ///   &lt;!-- Fournir les fichiers de session au générateur --&gt;
    ///   &lt;AdditionalFiles Include="PluginTestSessions\*.pluginsession.json" /&gt;
    /// &lt;/ItemGroup&gt;
    /// </code>
    /// </para>
    /// <para>
    /// Le projet de test doit également référencer :
    /// <list type="bullet">
    ///   <item><description><c>XrmFramework.RemoteDebugger.Client</c> (pour <c>PluginTestRunner</c>)</description></item>
    ///   <item><description><c>Verify.Xunit</c> ou <c>Verify.MSTest</c></description></item>
    ///   <item><description>L'assembly du plugin à tester</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Generator]
    public class PluginTestSourceGenerator : IIncrementalGenerator
    {
        /// <summary>Extension des fichiers de session de test plugin.</summary>
        private const string SessionFileExtension = ".pluginsession.json";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Sélectionner tous les AdditionalFiles dont l'extension est .pluginsession.json
            var sessionFiles = context.AdditionalTextsProvider
                .Where(text => text.Path.EndsWith(
                    SessionFileExtension,
                    StringComparison.OrdinalIgnoreCase));

            // Lire le contenu de chaque fichier
            var sessionContents = sessionFiles
                .Select(static (text, ct) => new SessionFileInfo(
                    path: text.Path,
                    fileName: Path.GetFileName(text.Path),
                    content: text.GetText(ct)?.ToString() ?? string.Empty))
                .Where(static s => !string.IsNullOrWhiteSpace(s.Content))
                .Collect();

            // Générer le code source pour tous les fichiers de session
            context.RegisterSourceOutput(sessionContents, GenerateTestClasses);
        }

        private static void GenerateTestClasses(
            SourceProductionContext context,
            ImmutableArray<SessionFileInfo> sessionFiles)
        {
            if (sessionFiles.IsDefaultOrEmpty)
                return;

            // Grouper les sessions par nom de plugin
            // Format du nom de fichier : {NomPlugin}_{yyyyMMdd_HHmmss}_{shortId}.pluginsession.json
            var groups = new Dictionary<string, List<SessionFileInfo>>(StringComparer.OrdinalIgnoreCase);

            foreach (var session in sessionFiles)
            {
                var pluginName = ExtractPluginName(session.FileName);

                if (!groups.TryGetValue(pluginName, out var list))
                {
                    list = new List<SessionFileInfo>();
                    groups[pluginName] = list;
                }

                list.Add(session);
            }

            // Générer une classe de test par plugin
            foreach (var kvp in groups)
            {
                var pluginName = kvp.Key;
                var sessions = kvp.Value;

                // Trier par nom de fichier (ordre chronologique)
                sessions.Sort((a, b) => string.Compare(a.FileName, b.FileName, StringComparison.OrdinalIgnoreCase));

                var source = GenerateTestClass(pluginName, sessions);
                context.AddSource(
                    $"{pluginName}_SessionTests.g.cs",
                    SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Extrait le nom du plugin depuis le nom du fichier de session.
        /// Format attendu : {NomPlugin}_{yyyyMMdd_HHmmss}_{shortId}.pluginsession.json
        /// </summary>
        private static string ExtractPluginName(string fileName)
        {
            // Retirer l'extension
            var nameWithoutExt = fileName.EndsWith(SessionFileExtension, StringComparison.OrdinalIgnoreCase)
                ? fileName.Substring(0, fileName.Length - SessionFileExtension.Length)
                : fileName;

            // Le nom du plugin est tout ce qui précède le premier '_' suivi d'un chiffre
            // (début du timestamp yyyyMMdd)
            var underscoreIndex = IndexOfTimestampSeparator(nameWithoutExt);

            return underscoreIndex > 0
                ? nameWithoutExt.Substring(0, underscoreIndex)
                : MakeValidIdentifier(nameWithoutExt);
        }

        /// <summary>
        /// Trouve l'index du '_' qui précède la partie timestamp (8 chiffres).
        /// </summary>
        private static int IndexOfTimestampSeparator(string name)
        {
            for (int i = 0; i < name.Length - 8; i++)
            {
                if (name[i] == '_' && i + 9 <= name.Length)
                {
                    // Vérifier que les 8 caractères suivants sont des chiffres (yyyyMMdd)
                    var isTimestamp = true;
                    for (int j = i + 1; j <= i + 8 && j < name.Length; j++)
                    {
                        if (!char.IsDigit(name[j]))
                        {
                            isTimestamp = false;
                            break;
                        }
                    }
                    if (isTimestamp) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Génère le nom de la méthode de test depuis le nom du fichier.
        /// Exemple : AccountPlugin_20241201_143022_a1b2c3d4 → Session_20241201_143022_a1b2c3d4
        /// </summary>
        private static string ExtractTestMethodName(string fileName)
        {
            var nameWithoutExt = fileName.EndsWith(SessionFileExtension, StringComparison.OrdinalIgnoreCase)
                ? fileName.Substring(0, fileName.Length - SessionFileExtension.Length)
                : fileName;

            var underscoreIndex = IndexOfTimestampSeparator(nameWithoutExt);
            var suffix = underscoreIndex > 0
                ? nameWithoutExt.Substring(underscoreIndex + 1)
                : nameWithoutExt;

            return "Session_" + MakeValidIdentifier(suffix);
        }

        /// <summary>
        /// Transforme une chaîne en identifiant C# valide.
        /// </summary>
        private static string MakeValidIdentifier(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                sb.Append(char.IsLetterOrDigit(c) ? c : '_');
            }
            return sb.ToString();
        }

        /// <summary>
        /// Échappe une chaîne pour l'utiliser dans un verbatim string C# (@"...").
        /// Les guillemets doubles sont doublés.
        /// </summary>
        private static string EscapeVerbatimString(string s)
            => s.Replace("\"", "\"\"");

        /// <summary>
        /// Génère le code source complet d'une classe de tests pour un plugin donné.
        /// </summary>
        private static string GenerateTestClass(
            string pluginName,
            List<SessionFileInfo> sessions)
        {
            var className = MakeValidIdentifier(pluginName) + "_SessionTests";

            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("// Généré par XrmFramework.RemoteDebugger.Generator");
            sb.AppendLine("// Ne pas modifier ce fichier manuellement.");
            sb.AppendLine("// Pour régénérer, relancez simplement la compilation après modification des fichiers .pluginsession.json.");
            sb.AppendLine();
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using VerifyXunit;");
            sb.AppendLine("using Xunit;");
            sb.AppendLine("using XrmFramework.RemoteDebugger.Common;");
            sb.AppendLine();
            sb.AppendLine("namespace XrmFramework.RemoteDebugger.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Tests unitaires générés automatiquement pour <c>{pluginName}</c>.");
            sb.AppendLine("    /// Chaque méthode rejoue une session de débogage distant enregistrée");
            sb.AppendLine("    /// et vérifie que le contexte de sortie correspond au snapshot Verify.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [UsesVerify]");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");

            foreach (var session in sessions)
            {
                var methodName = ExtractTestMethodName(session.FileName);
                var escapedJson = EscapeVerbatimString(session.Content);

                sb.AppendLine($"        /// <summary>");
                sb.AppendLine($"        /// Rejoue la session enregistrée depuis le fichier : {session.FileName}");
                sb.AppendLine($"        /// </summary>");
                sb.AppendLine("        [Fact]");
                sb.AppendLine($"        public async Task {methodName}()");
                sb.AppendLine("        {");
                sb.AppendLine("            // JSON de la session enregistrée lors du débogage distant");
                sb.AppendLine("            // (intégré directement pour éviter toute dépendance aux fichiers externes)");
                sb.AppendLine($"            const string sessionJson = @\"{escapedJson}\";");
                sb.AppendLine();
                sb.AppendLine("            // Exécuter le plugin avec le contexte d'entrée enregistré,");
                sb.AppendLine("            // en rejouant tous les appels CRM depuis les réponses enregistrées.");
                sb.AppendLine("            var outputContext = PluginTestRunner.RunFromJson(sessionJson);");
                sb.AppendLine();
                sb.AppendLine("            // Vérifier le contexte de sortie via un snapshot Verify.");
                sb.AppendLine("            // Lors du premier lancement, le fichier .verified.txt est créé.");
                sb.AppendLine("            // Les exécutions suivantes vérifient que la sortie n'a pas changé.");
                sb.AppendLine("            await Verifier.Verify(outputContext)");
                sb.AppendLine($"                .UseDirectory(\"TestData\")");
                sb.AppendLine($"                .UseFileName(\"{MakeValidIdentifier(session.FileName.Replace(SessionFileExtension, string.Empty))}\");");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>Représente les informations d'un fichier de session.</summary>
        private readonly struct SessionFileInfo(string path, string fileName, string content)
        {
            public string Path { get; } = path;
            public string FileName { get; } = fileName;
            public string Content { get; } = content;
        }
    }
}
