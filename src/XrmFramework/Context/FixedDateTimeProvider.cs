// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace XrmFramework
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeProvider"/> that returns a fixed date.
    /// Used when replaying test sessions to guarantee the reproducibility of
    /// relative date calculations (e.g. "add 3 days to now").
    /// </summary>
    public class FixedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTime _utcNow;

        /// <summary>
        /// Initializes the provider with the recorded execution date.
        /// </summary>
        /// <param name="executionDate">
        /// Original execution date. Can be local or UTC;
        /// it is normalized to UTC internally.
        /// </param>
        public FixedDateTimeProvider(DateTime executionDate)
        {
            _utcNow = executionDate.Kind == DateTimeKind.Utc
                ? executionDate
                : executionDate.ToUniversalTime();
        }

        /// <inheritdoc />
        /// <remarks>Returns the local time corresponding to the recorded execution date.</remarks>
        public DateTime Now => _utcNow.ToLocalTime();

        /// <inheritdoc />
        /// <remarks>Returns the recorded UTC execution date.</remarks>
        public DateTime UtcNow => _utcNow;

        /// <inheritdoc />
        /// <remarks>Returns the local day of the recorded execution date, at midnight.</remarks>
        public DateTime Today => _utcNow.ToLocalTime().Date;
    }
}
