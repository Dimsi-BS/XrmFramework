// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace XrmFramework.RemoteDebugger.Common
{
    public interface IRemoteDebuggerMessageManager
    {
        /// <summary>
        /// Event raised each time an execution context is received from the cloud.
        /// </summary>
        event Action<RemoteDebugExecutionContext> ContextReceived;

        /// <summary>Sends a message without waiting for a response.</summary>
        Task SendMessage(RemoteDebuggerMessage message);

        /// <summary>Sends a message and waits for the corresponding response.</summary>
        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

        /// <summary>
        /// Opens the connection to the Azure relay and blocks until the user
        /// presses Enter in the console. Legacy method for use without the TUI.
        /// </summary>
        void RunAndBlock();

        /// <summary>
        /// Opens the connection to the Azure relay without blocking.
        /// To be used with <see cref="CloseAsync"/> to manage the lifecycle manually
        /// (for example when using the interactive TUI).
        /// </summary>
        Task OpenAsync();

        /// <summary>
        /// Closes the connection to the Azure relay.
        /// </summary>
        Task CloseAsync();
    }
}
