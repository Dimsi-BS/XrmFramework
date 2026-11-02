namespace XrmFramework.Core
{
    /// <summary>
    /// The rule the three kinds of declaration a <c>.table</c> carries — the table itself, its
    /// columns and its option sets — share when the framework's copy of a table and the consuming
    /// project's copy of it are folded together.
    /// </summary>
    internal static class LockArbitration
    {
        /// <summary>
        /// The C# name the locked copy imposes on the other one, or <see langword="null" /> when
        /// neither does and the caller's ordinary rule decides.
        /// </summary>
        /// <remarks>
        /// A locked name is shipped by the framework along with the code that refers to it —
        /// <c>TeamDefinition.Columns.Type</c>, <c>TeamType.Owner</c> — so a copy the consuming
        /// project keeps only in order to enrich the table must not rename it, whichever of the two
        /// the fold happens to keep. Two copies agreeing on the lock have nothing to impose on each
        /// other. A locked copy declaring no name imposes nothing either: the other copy's name is
        /// still better than none.
        /// </remarks>
        internal static string LockedName(bool existingIsLocked, string existingName,
                                          bool incomingIsLocked, string incomingName)
        {
            if (existingIsLocked == incomingIsLocked)
            {
                return null;
            }

            var name = existingIsLocked ? existingName : incomingName;

            return string.IsNullOrEmpty(name) ? null : name;
        }
    }
}
