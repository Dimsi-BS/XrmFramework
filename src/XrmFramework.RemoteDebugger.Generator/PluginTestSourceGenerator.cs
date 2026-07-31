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
    /// Roslyn source generator that reads <c>.pluginsession.json</c> files
    /// recorded during remote debugging sessions and automatically generates
    /// xUnit unit test methods using Verify.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To enable this generator in a test project, add:
    /// <code>
    /// &lt;ItemGroup&gt;
    ///   &lt;!-- Reference the generator as an analyzer --&gt;
    ///   &lt;ProjectReference Include="..\XrmFramework.RemoteDebugger.Generator\..."
    ///                     OutputItemType="Analyzer"
    ///                     ReferenceOutputAssembly="false" /&gt;
    ///
    ///   &lt;!-- Provide the session files to the generator --&gt;
    ///   &lt;AdditionalFiles Include="PluginTestSessions\*.pluginsession.json" /&gt;
    /// &lt;/ItemGroup&gt;
    /// </code>
    /// </para>
    /// <para>
    /// The test project must also reference:
    /// <list type="bullet">
    ///   <item><description><c>XrmFramework.RemoteDebugger.Client</c> (for <c>PluginTestRunner</c>)</description></item>
    ///   <item><description><c>Verify.Xunit</c> or <c>Verify.MSTest</c></description></item>
    ///   <item><description>The assembly of the plugin under test</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    [Generator]
    public class PluginTestSourceGenerator : IIncrementalGenerator
    {
        /// <summary>Extension of plugin test session files.</summary>
        private const string SessionFileExtension = ".pluginsession.json";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Select all AdditionalFiles whose extension is .pluginsession.json
            var sessionFiles = context.AdditionalTextsProvider
                .Where(text => text.Path.EndsWith(
                    SessionFileExtension,
                    StringComparison.OrdinalIgnoreCase));

            // Read the content of each file
            var sessionContents = sessionFiles
                .Select(static (text, ct) => new SessionFileInfo(
                    path: text.Path,
                    fileName: Path.GetFileName(text.Path),
                    content: text.GetText(ct)?.ToString() ?? string.Empty))
                .Where(static s => !string.IsNullOrWhiteSpace(s.Content))
                .Collect();

            // Generate the source code for all session files
            context.RegisterSourceOutput(sessionContents, GenerateTestClasses);
        }

        private static void GenerateTestClasses(
            SourceProductionContext context,
            ImmutableArray<SessionFileInfo> sessionFiles)
        {
            if (sessionFiles.IsDefaultOrEmpty)
                return;

            // Group sessions by plugin name
            // File name format: {PluginName}_{yyyyMMdd_HHmmss}_{shortId}.pluginsession.json
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

            // Generate one test class per plugin
            foreach (var kvp in groups)
            {
                var pluginName = kvp.Key;
                var sessions = kvp.Value;

                // Sort by file name (chronological order)
                sessions.Sort((a, b) => string.Compare(a.FileName, b.FileName, StringComparison.OrdinalIgnoreCase));

                var source = GenerateTestClass(pluginName, sessions);
                context.AddSource(
                    $"{pluginName}_SessionTests.g.cs",
                    SourceText.From(source, Encoding.UTF8));
            }
        }

        /// <summary>
        /// Extracts the plugin name from the session file name.
        /// Expected format: {PluginName}_{yyyyMMdd_HHmmss}_{shortId}.pluginsession.json
        /// </summary>
        private static string ExtractPluginName(string fileName)
        {
            // Strip the extension
            var nameWithoutExt = fileName.EndsWith(SessionFileExtension, StringComparison.OrdinalIgnoreCase)
                ? fileName.Substring(0, fileName.Length - SessionFileExtension.Length)
                : fileName;

            // The plugin name is everything before the first '_' followed by a digit
            // (start of the yyyyMMdd timestamp)
            var underscoreIndex = IndexOfTimestampSeparator(nameWithoutExt);

            return underscoreIndex > 0
                ? nameWithoutExt.Substring(0, underscoreIndex)
                : MakeValidIdentifier(nameWithoutExt);
        }

        /// <summary>
        /// Finds the index of the '_' that precedes the timestamp part (8 digits).
        /// </summary>
        private static int IndexOfTimestampSeparator(string name)
        {
            for (int i = 0; i < name.Length - 8; i++)
            {
                if (name[i] == '_' && i + 9 <= name.Length)
                {
                    // Check that the next 8 characters are digits (yyyyMMdd)
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
        /// Generates the test method name from the file name.
        /// Example: AccountPlugin_20241201_143022_a1b2c3d4 -> Session_20241201_143022_a1b2c3d4
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
        /// Turns a string into a valid C# identifier.
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
        /// Escapes a string for use inside a C# verbatim string (@"...").
        /// Double quotes are doubled.
        /// </summary>
        private static string EscapeVerbatimString(string s)
            => s.Replace("\"", "\"\"");

        /// <summary>
        /// Generates the complete source code of a test class for a given plugin.
        /// </summary>
        private static string GenerateTestClass(
            string pluginName,
            List<SessionFileInfo> sessions)
        {
            var className = MakeValidIdentifier(pluginName) + "_SessionTests";

            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("// Generated by XrmFramework.RemoteDebugger.Generator");
            sb.AppendLine("// Do not modify this file manually.");
            sb.AppendLine("// To regenerate, simply rebuild after modifying the .pluginsession.json files.");
            sb.AppendLine();
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using VerifyXunit;");
            sb.AppendLine("using Xunit;");
            sb.AppendLine("using XrmFramework.RemoteDebugger.Common;");
            sb.AppendLine();
            sb.AppendLine("namespace XrmFramework.RemoteDebugger.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// Automatically generated unit tests for <c>{pluginName}</c>.");
            sb.AppendLine("    /// Each method replays a recorded remote debugging session");
            sb.AppendLine("    /// and verifies that the output context matches the Verify snapshot.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    [UsesVerify]");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");

            foreach (var session in sessions)
            {
                var methodName = ExtractTestMethodName(session.FileName);
                var escapedJson = EscapeVerbatimString(session.Content);

                sb.AppendLine($"        /// <summary>");
                sb.AppendLine($"        /// Replays the recorded session from file: {session.FileName}");
                sb.AppendLine($"        /// </summary>");
                sb.AppendLine("        [Fact]");
                sb.AppendLine($"        public async Task {methodName}()");
                sb.AppendLine("        {");
                sb.AppendLine("            // JSON of the session recorded during remote debugging");
                sb.AppendLine("            // (embedded directly to avoid any dependency on external files)");
                sb.AppendLine($"            const string sessionJson = @\"{escapedJson}\";");
                sb.AppendLine();
                sb.AppendLine("            // Run the plugin with the recorded input context,");
                sb.AppendLine("            // replaying all CRM calls from the recorded responses.");
                sb.AppendLine("            var outputContext = PluginTestRunner.RunFromJson(sessionJson);");
                sb.AppendLine();
                sb.AppendLine("            // Verify the output context via a Verify snapshot.");
                sb.AppendLine("            // On first run, the .verified.txt file is created.");
                sb.AppendLine("            // Subsequent runs verify that the output hasn't changed.");
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

        /// <summary>Represents the information of a session file.</summary>
        private readonly struct SessionFileInfo(string path, string fileName, string content)
        {
            public string Path { get; } = path;
            public string FileName { get; } = fileName;
            public string Content { get; } = content;
        }
    }
}
