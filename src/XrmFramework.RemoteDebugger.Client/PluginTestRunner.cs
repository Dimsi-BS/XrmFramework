// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger;

namespace XrmFramework.RemoteDebugger.Common
{
    /// <summary>
    /// Exécute un plugin Dynamics 365 à partir d'une session de test enregistrée,
    /// en rejouant tous les appels au service d'organisation CRM depuis les réponses enregistrées.
    /// Aucune connexion réseau n'est requise.
    /// </summary>
    /// <remarks>
    /// Cette classe est conçue pour être utilisée par les tests unitaires générés automatiquement
    /// par <c>XrmFramework.RemoteDebugger.Generator</c>.
    /// </remarks>
    public static class PluginTestRunner
    {
        /// <summary>
        /// Exécute le plugin décrit dans le JSON de session et retourne le contexte d'exécution
        /// modifié (avec les OutputParameters, SharedVariables, etc. mis à jour).
        /// Tous les appels au service d'organisation CRM sont rejoués depuis les réponses enregistrées.
        /// </summary>
        /// <param name="sessionJson">Contenu JSON d'une <see cref="PluginTestSession"/>.</param>
        /// <returns>
        /// Le contexte d'exécution après l'exécution du plugin.
        /// Ce contexte peut être comparé via Verify pour créer un test snapshot.
        /// </returns>
        /// <exception cref="ArgumentNullException">Si <paramref name="sessionJson"/> est null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Si le JSON est invalide, si le type du plugin ne peut pas être résolu,
        /// ou si le plugin effectue plus d'appels OrgService qu'il n'en a été enregistré.
        /// </exception>
        public static RemoteDebugExecutionContext RunFromJson(string sessionJson)
        {
            if (sessionJson == null) throw new ArgumentNullException(nameof(sessionJson));

            var session = JsonConvert.DeserializeObject<PluginTestSession>(
                sessionJson,
                RemoteDebuggerSettings.JsonSerializerSettings);

            if (session == null)
                throw new InvalidOperationException("Impossible de désérialiser la session de test plugin.");

            return Run(session);
        }

        /// <summary>
        /// Exécute le plugin décrit dans la session et retourne le contexte d'exécution modifié.
        /// </summary>
        /// <param name="session">La session de test à rejouer.</param>
        /// <returns>Le contexte d'exécution après l'exécution du plugin.</returns>
        public static RemoteDebugExecutionContext Run(PluginTestSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            var callIndex = 0;
            var inputContext = session.InputContext;

            // Injecter la date d'exécution originale dans le contexte.
            // Cela déclenche InitializeDateTimeProvider() dans LocalContext, qui substitue
            // SystemDateTimeProvider par FixedDateTimeProvider(session.ExecutionDate).
            // Résultat : clock.UtcNow retourne la même valeur qu'au moment de l'enregistrement,
            // rendant reproductibles les calculs de dates relatives (ex : clock.UtcNow.AddDays(30)).
            if (session.ExecutionDate != default)
            {
                inputContext.ExecutionDate = session.ExecutionDate;
            }

            // Configurer le provider de services avec un OrgService qui rejoue les réponses enregistrées
            var serviceProvider = new LocalServiceProvider(inputContext);

            serviceProvider.RequestSent += request =>
            {
                if (callIndex >= session.OrgServiceCalls.Count)
                {
                    throw new InvalidOperationException(
                        $"Appel OrgService inattendu #{callIndex + 1}. " +
                        $"Seulement {session.OrgServiceCalls.Count} appel(s) ont été enregistrés dans cette session. " +
                        $"Le comportement du plugin a peut-être changé depuis l'enregistrement.");
                }

                var recorded = session.OrgServiceCalls[callIndex++];

                // Retourner la réponse enregistrée comme RemoteDebuggerMessage
                return new RemoteDebuggerMessage
                {
                    MessageType = RemoteDebuggerMessageType.Response,
                    PluginExecutionId = inputContext.Id,
                    Content = recorded.ResponseJson
                };
            };

            // Résoudre le type du plugin (supprimer Version/PublicKeyToken pour la portabilité)
            var pluginType = ResolvePluginType(inputContext.TypeAssemblyQualifiedName);

            if (pluginType == null)
            {
                throw new InvalidOperationException(
                    $"Impossible de résoudre le type du plugin '{inputContext.TypeAssemblyQualifiedName}'. " +
                    "Assurez-vous que l'assembly du plugin est référencé par le projet de test.");
            }

            if (inputContext.IsWorkflowContext)
            {
                RunWorkflowActivity(pluginType, serviceProvider, inputContext);
            }
            else
            {
                RunPlugin(pluginType, serviceProvider, inputContext);
            }

            return inputContext;
        }

        private static Type ResolvePluginType(string assemblyQualifiedName)
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
                return null;

            // Supprimer les tokens Version et PublicKey pour permettre la portabilité entre versions
            var parts = assemblyQualifiedName
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !p.StartsWith("Version=") && !p.StartsWith("PublicKeyToken=") && !p.StartsWith("Culture="))
                .ToList();

            var typeName = string.Join(", ", parts);
            return Type.GetType(typeName);
        }

        private static void RunPlugin(
            Type pluginType,
            LocalServiceProvider serviceProvider,
            RemoteDebugExecutionContext context)
        {
            IPlugin plugin;

            if (pluginType.GetConstructor(new[] { typeof(string), typeof(string) }) != null)
            {
                plugin = (IPlugin)Activator.CreateInstance(
                    pluginType,
                    context.UnsecureConfig,
                    context.SecureConfig);
            }
            else
            {
                plugin = (IPlugin)Activator.CreateInstance(pluginType);
            }

            plugin.Execute(serviceProvider);
        }

        private static void RunWorkflowActivity(
            Type activityType,
            LocalServiceProvider serviceProvider,
            RemoteDebugExecutionContext context)
        {
            var codeActivity = (CodeActivity)Activator.CreateInstance(activityType);
            var invoker = new WorkflowInvoker(codeActivity);

            AddWorkflowExtension<IWorkflowContext>(serviceProvider, invoker);
            AddWorkflowExtension<IOrganizationServiceFactory>(serviceProvider, invoker);
            AddWorkflowExtension<IServiceEndpointNotificationService>(serviceProvider, invoker);
            AddWorkflowExtension<ITracingService>(serviceProvider, invoker);

            var inputs = context.Arguments.ToDictionary(k => k.Key, k => k.Value);
            var outputs = invoker.Invoke(inputs);

            context.Arguments.Clear();
            foreach (var output in outputs)
            {
                context.Arguments[output.Key] = output.Value;
            }
        }

        private static void AddWorkflowExtension<TService>(IServiceProvider provider, WorkflowInvoker invoker)
            where TService : class
        {
            var service = provider.GetService(typeof(TService));
            if (service != null)
            {
                invoker.Extensions.Add<TService>(() => (TService)service);
            }
        }
    }
}
