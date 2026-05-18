// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace XrmFramework.RemoteDebugger.Common
{
    public interface IRemoteDebuggerMessageManager
    {
        /// <summary>
        /// Événement déclenché à chaque réception d'un contexte d'exécution depuis le cloud.
        /// </summary>
        event Action<RemoteDebugExecutionContext> ContextReceived;

        /// <summary>Envoie un message sans attendre de réponse.</summary>
        Task SendMessage(RemoteDebuggerMessage message);

        /// <summary>Envoie un message et attend la réponse correspondante.</summary>
        Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message);

        /// <summary>
        /// Ouvre la connexion au relay Azure et bloque jusqu'à ce que l'utilisateur
        /// appuie sur Entrée dans la console. Méthode historique pour usage sans TUI.
        /// </summary>
        void RunAndBlock();

        /// <summary>
        /// Ouvre la connexion au relay Azure sans bloquer.
        /// À utiliser avec <see cref="CloseAsync"/> pour gérer le cycle de vie manuellement
        /// (par exemple lors de l'utilisation du TUI interactif).
        /// </summary>
        Task OpenAsync();

        /// <summary>
        /// Ferme la connexion au relay Azure.
        /// </summary>
        Task CloseAsync();
    }
}
