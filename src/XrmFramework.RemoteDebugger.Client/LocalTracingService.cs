// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.RemoteDebugger.Common
{
    /// <summary>
    /// Implémentation locale de <see cref="ITracingService"/> qui transmet chaque trace
    /// à un callback fourni à la construction (typiquement <c>ExecutionRecord.AddTraceLog</c>).
    /// Aucune écriture dans <c>Console</c> — les logs sont capturés dans la session.
    /// </summary>
    public class LocalTracingService : ITracingService
    {
        private readonly Action<string> _sink;

        /// <param name="sink">
        /// Callback appelé pour chaque ligne de trace.
        /// Si <c>null</c>, les traces sont silencieusement ignorées.
        /// </param>
        public LocalTracingService(Action<string> sink = null)
        {
            _sink = sink;
        }

        public void Trace(string format, params object[] args)
        {
            if (_sink == null) return;

            var message = args.Length > 0
                ? string.Format(format, args)
                : format;

            _sink(message);
        }
    }
}