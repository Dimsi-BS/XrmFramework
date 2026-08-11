// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XrmFramework.DeployUtils.TableSync
{
    /// <summary>
    /// One table described by several definitions that disagree on the name it should carry.
    /// </summary>
    public sealed class DefinitionNameConflict
    {
        public DefinitionNameConflict(string logicalName, IReadOnlyList<string> names)
        {
            LogicalName = logicalName;
            Names = names ?? new List<string>();
        }

        /// <summary>Logical name of the CRM table, the identity the definitions do agree on.</summary>
        public string LogicalName { get; }

        /// <summary>The distinct names proposed for it, ordered.</summary>
        public IReadOnlyList<string> Names { get; }
    }

    /// <summary>
    /// Raised when the analyzed assembly describes a table under several names, leaving no way to
    /// tell which one its <c>.table</c> — and therefore the generated definition class — should
    /// carry.
    /// </summary>
    /// <remarks>
    /// Not an internal error: the assembly says two things at once and only the project can settle
    /// it. The message therefore names every table concerned and what to do about it, and the CLI
    /// prints it as-is rather than as a stack trace.
    /// </remarks>
    public sealed class DefinitionNameConflictException : Exception
    {
        public DefinitionNameConflictException(IReadOnlyList<DefinitionNameConflict> conflicts)
            : base(BuildMessage(conflicts))
        {
            Conflicts = conflicts ?? new List<DefinitionNameConflict>();
        }

        public IReadOnlyList<DefinitionNameConflict> Conflicts { get; }

        private static string BuildMessage(IReadOnlyList<DefinitionNameConflict> conflicts)
        {
            var ordered = (conflicts ?? new List<DefinitionNameConflict>())
                          .OrderBy(c => c.LogicalName, StringComparer.OrdinalIgnoreCase)
                          .ToList();

            var sb = new StringBuilder();

            sb.Append(ordered.Count == 1
                          ? "1 table is described under several names in the DLL"
                          : $"{ordered.Count} tables are described under several names in the DLL");
            sb.AppendLine(", so the name its .table should carry cannot be settled here:");
            sb.AppendLine();

            foreach (var conflict in ordered)
            {
                sb.AppendLine($"  {conflict.LogicalName}");
                foreach (var name in conflict.Names)
                    sb.AppendLine($"      {name}Definition");
            }

            sb.AppendLine();
            sb.AppendLine("This is the 2.* definition class and the one the generator emits from the .table");
            sb.AppendLine("disagreeing. Keep the name your own code refers to: either delete the versioned");
            sb.AppendLine("*Definition.cs the generator now replaces, or set \"Name\" in the .table to that name.");
            sb.Append("Nothing was modified.");

            return sb.ToString();
        }
    }
}
