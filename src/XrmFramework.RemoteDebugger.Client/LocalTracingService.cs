// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Xrm.Sdk;

namespace XrmFramework.RemoteDebugger.Common
{
    /// <summary>
    /// Local implementation of <see cref="ITracingService"/> that forwards each trace
    /// to a callback provided at construction time (typically <c>ExecutionRecord.AddTraceLog</c>).
    /// No writes to <c>Console</c> — logs are captured in the session.
    /// </summary>
    public class LocalTracingService : ITracingService
    {
        private readonly Action<string> _sink;

        /// <param name="sink">
        /// Callback invoked for each trace line.
        /// If <c>null</c>, traces are silently ignored.
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