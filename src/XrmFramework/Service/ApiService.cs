using System;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace XrmFramework;

public abstract class ApiService<TSettings> : DefaultServiceWithSettings<TSettings> where TSettings : CrmSettings, new()
{
    private HttpClient Client { get; set; }

    private void CheckOrInitConnection()
    {
        if (Client != null)
        {
            return;
        }

        lock (SyncRoot)
        {
            if (Client != null)
            {
                return;
            }

            var handler = new HttpClientHandler();

            var credentials = GetCredentials();

            if (credentials != null)
            {
                handler.Credentials = credentials;
            }

            Client = new HttpClient(handler)
            {
                Timeout = new TimeSpan(0, 2, 0)
            };
            // Add HTTP headers
            Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.ConnectionClose = false;
        }
    }

    protected virtual ICredentials GetCredentials() => null;

    protected virtual void AddAuthenticationHeader(HttpRequestHeaders requestMessageHeaders)
    {
    }

    protected TResponse HttpPostData<TResponse>(string url, string requestContent, bool useAuthenticationHeaders = true) =>
        HttpPostData<TResponse>(url,
            new StringContent(requestContent, System.Text.Encoding.UTF8, "application/json"), useAuthenticationHeaders);

    protected TResponse HttpPostData<TResponse>(string url, HttpContent requestContent, bool useAuthenticationHeaders = true)
    {
        CheckOrInitConnection();

        var isSuccessful = false;

        string replyContent;

        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = requestContent
            };

            if (useAuthenticationHeaders)
            {
                AddAuthenticationHeader(requestMessage.Headers);
            }

            var serviceResponse = Client.SendAsync(requestMessage).GetAwaiter().GetResult();

            replyContent = serviceResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            isSuccessful = serviceResponse.IsSuccessStatusCode;

        }
        catch (HttpRequestException e)
        {
            replyContent = e.Message;
        }

        if (!isSuccessful)
        {
            throw new InvalidPluginExecutionException(replyContent);
        }

        if (typeof(TResponse) == typeof(string))
        {
            return (TResponse)(object)replyContent;
        }

        var response = JsonConvert.DeserializeObject<TResponse>(replyContent);

        return response;
    }

    protected TResponse HttpPostData<TRequest, TResponse>(string url, TRequest request, bool useAuthenticationHeaders = true)
    {
        var requestContent = JsonConvert.SerializeObject(request);

        return HttpPostData<TResponse>(url, requestContent);
    }

    protected TResponse HttpGetData<TResponse>(string url, bool useAuthenticationHeaders = true)
    {
        CheckOrInitConnection();

        var isSuccessful = false;

        string replyContent;

        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            if (useAuthenticationHeaders)
            {
                AddAuthenticationHeader(requestMessage.Headers);
            }
            var serviceResponse = Client.SendAsync(requestMessage).GetAwaiter().GetResult();
            replyContent = serviceResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            isSuccessful = serviceResponse.IsSuccessStatusCode;
        }
        catch (HttpRequestException e)
        {
            replyContent = e.Message;
        }

        if (isSuccessful)
        {
            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)replyContent;
            }

            var response = JsonConvert.DeserializeObject<TResponse>(replyContent);

            return response;
        }

        throw new InvalidPluginExecutionException(replyContent);
    }

    protected ApiService(IServiceContext context) : base(context)
    {
    }
}