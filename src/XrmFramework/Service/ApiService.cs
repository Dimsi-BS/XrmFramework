using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace XrmFramework
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract partial class ApiService<TSettings> : DefaultServiceWithSettings<TSettings>
        where TSettings : CrmSettings, new()
    {
        private readonly Lazy<HttpClient> _httpClient;
        private const string Patch = "Patch";
        
        protected HttpClient Client => _httpClient.Value;

        protected virtual ICredentials GetCredentials() => null;

        protected virtual void AddAuthenticationHeader(HttpRequestHeaders requestMessageHeaders)
        {
        }

        protected TResponse HttpPostData<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true,
            Action<HttpRequestHeaders> addHeaders = null)
            => HttpPostDataAsync<TResponse>(url, requestContent, useAuthenticationHeaders, addHeaders).GetAwaiter().GetResult();

        protected TResponse HttpPostData<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true,
            Action<HttpRequestHeaders> addHeaders = null)
            => HttpPostDataAsync<TResponse>(url, requestContent,
                useAuthenticationHeaders,
                addHeaders)
                .GetAwaiter().GetResult();

        protected TResponse HttpPostData<TRequest, TResponse>(string url, TRequest request,
            bool useAuthenticationHeaders = true,
            Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TRequest, TResponse>(
                url,
                request,
                HttpMethod.Post,
                useAuthenticationHeaders,
                addHeaders)
                .GetAwaiter().GetResult();
  
        protected Task<TResponse> HttpPostDataAsync<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TRequest, TResponse>(
                url, 
                request, 
                HttpMethod.Post,
                useAuthenticationHeaders,
                addHeaders);
        
        protected Task<TResponse> HttpPostDataAsync<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(url, requestContent, HttpMethod.Post, useAuthenticationHeaders, addSpecificHeaders);

        protected Task<TResponse> HttpPostDataAsync<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url, 
                requestContent, 
                HttpMethod.Post,
                useAuthenticationHeaders,
                addSpecificHeaders);
        
        protected TResponse HttpPatchData<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url, 
                requestContent, 
                new HttpMethod(Patch), 
                useAuthenticationHeaders,
                addSpecificHeaders)
                .GetAwaiter().GetResult();

        protected TResponse HttpPatchData<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                    url, 
                    requestContent, 
                    new HttpMethod(Patch), 
                    useAuthenticationHeaders,
                    addSpecificHeaders)
                .GetAwaiter().GetResult();

        protected TResponse HttpPatchData<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpPatchDataAsync<TRequest, TResponse>(url, request, useAuthenticationHeaders, addSpecificHeaders).GetAwaiter().GetResult();
        
        protected Task<TResponse> HttpPatchDataAsync<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TRequest, TResponse>(url, request, new HttpMethod(Patch), useAuthenticationHeaders, addSpecificHeaders);
        
        protected Task<TResponse> HttpPatchDataAsync<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                    url, 
                    requestContent, 
                    new HttpMethod(Patch), 
                    useAuthenticationHeaders,
                    addSpecificHeaders);

        protected Task<TResponse> HttpPatchDataAsync<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url, 
                requestContent, 
                new HttpMethod(Patch), 
                useAuthenticationHeaders,
                addSpecificHeaders);

        protected Task<TResponse> HttpPutDataAsync<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TRequest, TResponse>(url, request, HttpMethod.Put, useAuthenticationHeaders, addSpecificHeaders);
        
        protected Task<TResponse> HttpPutDataAsync<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(url, requestContent, HttpMethod.Put, useAuthenticationHeaders, addSpecificHeaders);

        protected Task<TResponse> HttpPutDataAsync<TResponse>(string url, string requestContent,
            bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url,
                requestContent, 
                HttpMethod.Put,
                useAuthenticationHeaders,
                addSpecificHeaders);
        
        protected TResponse HttpPutData<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpPutDataAsync<TResponse>(url,requestContent, useAuthenticationHeaders, addSpecificHeaders).GetAwaiter().GetResult();

        protected TResponse HttpPutData<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpPutDataAsync<TResponse>(url,requestContent, useAuthenticationHeaders, addSpecificHeaders).GetAwaiter().GetResult();

        protected TResponse HttpPutData<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpPutDataAsync<TRequest, TResponse>(url, request, useAuthenticationHeaders, addSpecificHeaders).GetAwaiter().GetResult();
        
        private TResponse HttpSendData<TResponse>(string url, HttpContent requestContent, HttpMethod method,
            bool useAuthenticationHeaders = true, Action<HttpRequestHeaders> addSpecificHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url, 
                requestContent, 
                method, 
                useAuthenticationHeaders,
                addSpecificHeaders
                ).GetAwaiter().GetResult();

        
        private Task<TResponse> HttpSendDataAsync<TRequest, TResponse>(string url, TRequest request, HttpMethod method,
            bool useDefaultAuthenticationHeaders = false, Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url,
                JsonConvert.SerializeObject(request),
                method,
                useDefaultAuthenticationHeaders,
                addHeaders
            );
        
        private Task<TResponse> HttpSendDataAsync<TResponse>(string url, string requestContent, HttpMethod method,
            bool useDefaultAuthenticationHeaders = false, Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TResponse>(
                url,
                new StringContent(requestContent, System.Text.Encoding.UTF8, "application/json"),
                method,
                useDefaultAuthenticationHeaders,
                addHeaders
            );
        
        private async Task<TResponse> HttpSendDataAsync<TResponse>(string url, HttpContent requestContent, HttpMethod method,
            bool useDefaultAuthenticationHeaders = false, Action<HttpRequestHeaders> addHeaders = null)
        {
            Log(nameof(HttpSendData), "Url : {0}", url);

            var isSuccessfull = false;

            #region Log Info

            string replyContent;
            var status = HttpStatusCode.Unused;

            #endregion

            try
            {
                var requestMessage = new HttpRequestMessage(method, url);
                
                if (requestContent != null)
                {
                    requestMessage.Content = requestContent;
                }

                (useDefaultAuthenticationHeaders ? AddAuthenticationHeader : addHeaders)?.Invoke(requestMessage.Headers);

                var serviceResponse = await Client.SendAsync(requestMessage);

                replyContent = await serviceResponse.Content.ReadAsStringAsync();
                status = serviceResponse.StatusCode;
                isSuccessfull = serviceResponse.IsSuccessStatusCode;
            }
            catch (HttpRequestException e)
            {
                replyContent = e.Message;
            }

            if (isSuccessfull)
            {
                if (typeof(TResponse) == typeof(string))
                {
                    return (TResponse)(object)replyContent;
                }

                var response = JsonConvert.DeserializeObject<TResponse>(replyContent);

                return response;
            }
            
            var exceptionContent = string.IsNullOrEmpty(replyContent) ? status.ToString() : replyContent;
            
            throw new InvalidPluginExecutionException(exceptionContent);
        }
        
        protected TResponse HttpGetData<TResponse>(string url, bool useDefaultAuthenticationHeaders = true, Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TResponse>(url, (HttpContent) null, HttpMethod.Get, useDefaultAuthenticationHeaders, addHeaders).GetAwaiter().GetResult();
        
        protected Task<TResponse> HttpGetDataAsync<TResponse>(string url, bool useDefaultAuthenticationHeaders = true, Action<HttpRequestHeaders> addHeaders = null)
            => HttpSendDataAsync<TResponse>(url, (HttpContent) null, HttpMethod.Get, useDefaultAuthenticationHeaders, addHeaders);

        protected ApiService(IServiceContext context) : base(context)
        {
            _httpClient = new Lazy<HttpClient>(() =>
            {
                var handler = new HttpClientHandler();

                var credentials = GetCredentials();

                if (credentials != null)
                {
                    handler.Credentials = credentials;
                }

                var client = new HttpClient(handler)
                {
                    Timeout = new TimeSpan(0, 2, 0)
                };
                // Add HTTP headers
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.ConnectionClose = false;

                return client;
            });
            
        }
    }
}
