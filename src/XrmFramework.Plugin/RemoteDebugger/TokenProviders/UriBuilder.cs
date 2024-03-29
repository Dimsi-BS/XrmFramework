using System;

// ReSharper disable once CheckNamespace
namespace XrmFramework.RemoteDebugger.TokenProviders;

internal static class UriHelper
{
    public static Uri NormalizeUri(
        string uri,
        string scheme,
        bool stripQueryParameters = true,
        bool stripPath = false,
        bool ensureTrailingSlash = false)
    {
        UriBuilder uriBuilder = new UriBuilder(uri)
        {
            Scheme = scheme,
            Port = -1,
            Fragment = string.Empty,
            Password = string.Empty,
            UserName = string.Empty
        };
        if (stripPath)
            uriBuilder.Path = string.Empty;
        if (stripQueryParameters)
            uriBuilder.Query = string.Empty;
        if (ensureTrailingSlash && !uriBuilder.Path.EndsWith("/", StringComparison.Ordinal))
            uriBuilder.Path += "/";
        return uriBuilder.Uri;
    }
}