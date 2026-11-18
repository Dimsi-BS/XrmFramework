using Microsoft.Azure.Relay;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XrmFramework.DeployUtils.Configuration;
using XrmFramework.RemoteDebugger.Common;

namespace XrmFramework.RemoteDebugger.Client
{
    public class AzureRelayHybridConnectionMessageManager : IRemoteDebuggerMessageManager
    {
        private static readonly ConcurrentDictionary<Guid, RemoteDebuggerMessage> MessageSendCache = new ConcurrentDictionary<Guid, RemoteDebuggerMessage>();
        private static readonly ConcurrentDictionary<Guid, RelayedHttpListenerResponse> CurrentResponseCache = new ConcurrentDictionary<Guid, RelayedHttpListenerResponse>();
        private static readonly ConcurrentDictionary<Guid, RemoteDebuggerMessage> MessageReceiveCache = new ConcurrentDictionary<Guid, RemoteDebuggerMessage>();

        public event Action<RemoteDebugExecutionContext> ContextReceived;

        /// <summary>
        ///     Environment variable holding the Azure Relay connection string, consulted before any
        ///     configuration file.
        /// </summary>
        /// <remarks>
        ///     The way a runner on .NET gets configured without an App.config. A classic .NET
        ///     Framework runner keeps working off its own, unchanged, through
        ///     <see cref="ConfigHelper" />; this gives every other host - and anyone who would rather
        ///     not keep a SAS key in a file - a way in that needs no XML at all.
        /// </remarks>
        public const string RelayConnectionStringVariable = "XRMFRAMEWORK_DEBUG_RELAY";

        private const string RelayConnectionStringName = "DebugConnectionString";

        public AzureRelayHybridConnectionMessageManager()
        {
            // create a connection string with the listener profile
            Listener = new HybridConnectionListener(ResolveRelayConnectionString());

          //  Listener.Connecting += (_, _) => { Console.WriteLine("Listener is connecting to Azure…"); };
          //  Listener.Offline += (_, _) => { Console.WriteLine("Listener is about to go offline…"); };
          //  Listener.Online += (_, _) => { Console.WriteLine("Listener is online…"); };

            Listener.RequestHandler = RequestHandler;
        }

        /// <summary>
        ///     The Azure Relay connection string: the environment variable if set, otherwise the
        ///     <c>DebugConnectionString</c> connection string from configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException">When neither is available.</exception>
        private static string ResolveRelayConnectionString()
        {
            var fromEnvironment = Environment.GetEnvironmentVariable(RelayConnectionStringVariable);
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
                return fromEnvironment;

            var fromConfiguration = ConfigHelper.GetConnectionString(RelayConnectionStringName);
            if (!string.IsNullOrWhiteSpace(fromConfiguration))
                return fromConfiguration;

            // Both options named, because which one applies depends on how the runner was started,
            // and the old message ("the connectionString is not defined") sent people looking for an
            // App.config a .NET runner may not even have.
            throw new InvalidOperationException(
                $"No Azure Relay connection string. Set the {RelayConnectionStringVariable} environment "
                + $"variable, or declare a \"{RelayConnectionStringName}\" connection string in the "
                + "runner's configuration - App.config for a .NET Framework runner, or the project's "
                + "Config/connectionStrings.config with ConfigHelper.UseProjectConfig() pointed at it.");
        }

        private HybridConnectionListener Listener { get; }

        private void RequestHandler(RelayedHttpListenerContext context)
        {
            var streamReader = new StreamReader(context.Request.InputStream);
            // reading the body
            var requestContent = streamReader.ReadToEnd();


            var message = JsonConvert.DeserializeObject<RemoteDebuggerMessage>(requestContent);

            CurrentResponseCache.AddOrUpdate(message.PluginExecutionId, context.Response, (_, _) => context.Response);

            if (message.MessageType == RemoteDebuggerMessageType.Context)
            {
                var remoteContext = message.GetContext<RemoteDebugExecutionContext>();

                OnContextReceived(remoteContext);

                MessageSendCache.TryAdd(remoteContext.Id, new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
            }

            if (message.MessageType == RemoteDebuggerMessageType.Response)
            {
                MessageReceiveCache.TryAdd(message.PluginExecutionId, message);

            }

            if (message.MessageType == RemoteDebuggerMessageType.Ping)
            {
                MessageSendCache.TryAdd(message.PluginExecutionId, message);
            }

            RemoteDebuggerMessage response;
            while (!MessageSendCache.TryRemove(message.PluginExecutionId, out response))
            {
                // Waiting for the response to come
                Thread.Sleep(50);
            }
            
            SendMessage(response);
        }

        public Task SendMessage(RemoteDebuggerMessage message)
        {

            if (CurrentResponseCache.TryGetValue(message.PluginExecutionId, out var currentResponse))
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                try
                {
                    currentResponse.OutputStream.Write(bytes, 0, bytes.Length);
                    currentResponse.Close();
                }
                catch (Exception)
                {
                    // error ignored
                }

            }

            return Task.CompletedTask;
        }

        public Task<RemoteDebuggerMessage> SendMessageWithResponse(RemoteDebuggerMessage message)
        {
            SendMessage(message);

            RemoteDebuggerMessage response;

            while (!MessageReceiveCache.TryRemove(message.PluginExecutionId, out response))
            {
                // Waiting for the response to come
                Thread.Sleep(50);
            }

            return Task.FromResult(response);
        }

        public void RunAndBlock()
        {
            Listener.OpenAsync().GetAwaiter().GetResult();
            Console.In.ReadLineAsync().GetAwaiter().GetResult();
            Listener.CloseAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public Task OpenAsync() => Listener.OpenAsync();

        /// <inheritdoc />
        public Task CloseAsync() => Listener.CloseAsync();

        protected virtual void OnContextReceived(RemoteDebugExecutionContext obj)
        {
            ContextReceived?.Invoke(obj);
        }
    }
}
