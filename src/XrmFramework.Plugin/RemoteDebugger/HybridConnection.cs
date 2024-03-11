using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XrmFramework.RemoteDebugger.TokenProviders;

namespace XrmFramework.RemoteDebugger
{
    public sealed class HybridConnection : IDisposable
    {
        private readonly string _uri;
        private readonly HttpClient _client;

        public HybridConnection(string keyName, string sharedAccessKey, string uri)
        {
            _uri = uri;
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                keyName, sharedAccessKey);
            
            var token =tokenProvider.GetTokenAsync(uri, TimeSpan.FromHours(1)).GetAwaiter().GetResult();

            _client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(2)
            };
            _client.DefaultRequestHeaders.Add("ServiceBusAuthorization", token.TokenString);
        }

        public async Task<RemoteDebuggerMessage?> SendMessage(RemoteDebuggerMessage message)
        {
            var serializedContext = JsonConvert.SerializeObject(message);

            var request = new HttpRequestMessage(HttpMethod.Post, _uri)
            {
                Content = new StringContent(serializedContext, Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                var responseMessage = JsonConvert.DeserializeObject<RemoteDebuggerMessage>(responseString);

                return responseMessage;
            }

            response.EnsureSuccessStatusCode();
            return null;
        }

        #region IDisposable

        public void Dispose()
        {
            _client?.Dispose();
        }

        #endregion

        public static bool TryPingDebugSession(DebugSession debugSession)
        {
            var uri = new Uri($"{debugSession.RelayUrl.TrimEnd('/')}/{debugSession.HybridConnectionName}");

            var isOnline = false;

            try
            {
                using var hybridConnection = new HybridConnection(debugSession.SasKeyName, debugSession.SasConnectionKey, uri.AbsoluteUri);

                var message = new RemoteDebuggerMessage(RemoteDebuggerMessageType.Ping, null, Guid.NewGuid());

                var response = hybridConnection.SendMessage(message).GetAwaiter().GetResult();

                isOnline = response.MessageType == RemoteDebuggerMessageType.Ping;
            }
            catch (HttpRequestException)
            {
                //isOnline is false and will stay that way
            }
            return isOnline;

        }
    }
}
