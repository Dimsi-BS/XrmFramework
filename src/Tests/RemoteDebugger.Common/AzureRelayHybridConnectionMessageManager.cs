using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Relay;
using XrmFramework.Debugger;

#if INTERNAL_NEWTONSOFT
using Newtonsoft.Json.Xrm;
#else 
using Newtonsoft.Json;
#endif



namespace XrmFramework.RemoteDebugger.Common
{
    public class AzureRelayHybridConnectionMessageManager : IRemoteDebuggerMessageManager
    {
        private static readonly ConcurrentDictionary<Guid, RemoteDebuggerMessage> MessageSendCache = new ConcurrentDictionary<Guid, RemoteDebuggerMessage>();
        private static readonly ConcurrentDictionary<Guid, RemoteDebuggerMessage> MessageReceiveCache = new ConcurrentDictionary<Guid, RemoteDebuggerMessage>();

        public event Action<RemoteDebugExecutionContext> ContextReceived;

        private RelayedHttpListenerResponse CurrentResponse
        {
            get; set;
        }

        public AzureRelayHybridConnectionMessageManager()
        {
            // create a connection string with the listener profile
            Listener = new HybridConnectionListener(ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString);

            Listener.Connecting += (o, e) => { Console.WriteLine("Listener is connecting to Azure…"); };
            Listener.Offline += (o, e) => { Console.WriteLine("Listener is about to go offline…"); };
            Listener.Online += (o, e) => { Console.WriteLine("Listener is online…"); };

            Listener.RequestHandler = RequestHandler;
        }

        private HybridConnectionListener Listener { get; }

        private void RequestHandler(RelayedHttpListenerContext context)
        {
            var streamReader = new StreamReader(context.Request.InputStream);
            // reading the body
            var requestContent = streamReader.ReadToEnd();

            CurrentResponse = context.Response;

            var message = JsonConvert.DeserializeObject<RemoteDebuggerMessage>(requestContent);
            if (message.MessageType == RemoteDebuggerMessageType.Context)
            {
                var remoteContext = message.GetContext<RemoteDebugExecutionContext>();
                OnContextReceived(remoteContext);

                MessageSendCache.TryAdd(remoteContext.Id, new RemoteDebuggerMessage(RemoteDebuggerMessageType.Context, remoteContext, remoteContext.Id));
            }

            if (message.MessageType == RemoteDebuggerMessageType.Response)
            {
                MessageReceiveCache.TryAdd(message.PluginExecutionId, message);

                RemoteDebuggerMessage response;
                while (!MessageSendCache.TryRemove(message.PluginExecutionId, out response))
                {
                    // Waiting for the response to come
                    Thread.Sleep(50);
                }

                //Console.WriteLine("{0}", response);

                SendMessage(response);
            }
        }

        public Task SendMessage(RemoteDebuggerMessage message)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            CurrentResponse.OutputStream.Write(bytes, 0, bytes.Length);
            CurrentResponse.OutputStream.Close();

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

        protected virtual void OnContextReceived(RemoteDebugExecutionContext obj)
        {
            ContextReceived?.Invoke(obj);
        }
    }
}
