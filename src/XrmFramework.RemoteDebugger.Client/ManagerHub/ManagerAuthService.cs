// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace XrmFramework.RemoteDebugger.Client.ManagerHub;

/// <summary>
/// MSAL authentication service for the RemoteDebugger.
/// Faithfully reproduces the Desktop application's flow:
/// <list type="number">
///   <item>Silent attempt from the cache (<see cref="IPublicClientApplication.AcquireTokenSilent"/>).</item>
///   <item>If the cache is empty or expired, opens an Azure account picker (<see cref="Prompt.SelectAccount"/>).</item>
/// </list>
/// The cache is persisted in <c>%LOCALAPPDATA%\xrmFramework\debugger.msalcache.bin3</c>
/// (encrypted with DPAPI on Windows, plain file on other platforms).
/// </summary>
internal sealed class ManagerAuthService
{
    // ── Constants ─────────────────────────────────────────────────────────

    /// <summary>Scope identical to the one used by the Desktop application.</summary>
    private static readonly string[] Scopes = { "api://xrmFramework-manager-api/desktop-connect" };

    /// <summary>Redirect URI — same value as the Desktop.</summary>
    private const string RedirectUri = "http://localhost";

    // ── State ─────────────────────────────────────────────────────────────

    private readonly IPublicClientApplication _clientApp;
    private readonly Action<string> _log;

    // ── Constructor ───────────────────────────────────────────────────────

    public ManagerAuthService(ManagerHubSettings settings, Action<string> log = null)
    {
        _log = log ?? Console.WriteLine;

        _clientApp = PublicClientApplicationBuilder
            .Create(settings.ClientId)
            .WithRedirectUri(RedirectUri)
            .WithAuthority(AzureCloudInstance.AzurePublic, settings.Tenant)
            .Build();

        // Token cache persistence (same principle as the Desktop's TokenCacheHelper)
        EnableTokenCacheSerialization(_clientApp.UserTokenCache);
    }

    // ── Public API ────────────────────────────────────────────────────────

    /// <summary>
    /// Obtains a valid access token for the Manager.
    /// First tries the silent cache; shows the account picker if needed.
    /// Returns <c>null</c> if authentication fails (error logged).
    /// </summary>
    public async Task<string> AcquireTokenAsync()
    {
        var accounts = await _clientApp.GetAccountsAsync();
        var firstAccount = accounts.FirstOrDefault();

        // 1. Silent attempt (cache)
        try
        {
            var result = await _clientApp
                .AcquireTokenSilent(Scopes, firstAccount)
                .ExecuteAsync();

            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // Cache empty or expired → interactive login
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Silent authentication error: {ex.Message}");
            return null;
        }

        // 2. Interactive login — identical to the Desktop (account picker)
        try
        {
            _log("[ManagerHub] Authentication required — opening the Azure account picker…");

            var result = await _clientApp
                .AcquireTokenInteractive(Scopes)
                .WithAccount(firstAccount)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync();

            _log($"[ManagerHub] Authenticated as {result.Account?.Username}.");
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Interactive authentication failed: {ex.Message}");
            return null;
        }
    }

    // ── Token cache ───────────────────────────────────────────────────────

    /// <summary>
    /// Path of the RemoteDebugger's MSAL cache file.
    /// Distinct from the Desktop cache (different assemblies), but in the same
    /// user directory for consistency.
    /// </summary>
    internal static string CacheFilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "xrmFramework",
        "debugger.msalcache.bin3");

    private static readonly object CacheLock = new();

    private static void EnableTokenCacheSerialization(ITokenCache tokenCache)
    {
        tokenCache.SetBeforeAccess(BeforeAccess);
        tokenCache.SetAfterAccess(AfterAccess);
    }

    private static void BeforeAccess(TokenCacheNotificationArgs args)
    {
        lock (CacheLock)
        {
            if (!File.Exists(CacheFilePath))
            {
                args.TokenCache.DeserializeMsalV3(null);
                return;
            }

            try
            {
                var bytes = File.ReadAllBytes(CacheFilePath);
                args.TokenCache.DeserializeMsalV3(Unprotect(bytes));
            }
            catch
            {
                // Unreadable or corrupted cache: start fresh
                args.TokenCache.DeserializeMsalV3(null);
            }
        }
    }

    private static void AfterAccess(TokenCacheNotificationArgs args)
    {
        if (!args.HasStateChanged) return;

        lock (CacheLock)
        {
            try
            {
                var dir = Path.GetDirectoryName(CacheFilePath)!;
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllBytes(CacheFilePath, Protect(args.TokenCache.SerializeMsalV3()));
            }
            catch
            {
                // Non-blocking persistence failure
            }
        }
    }

    // ── DPAPI encryption (Windows) / identity (other platforms) ─────────────

    private static byte[] Protect(byte[] data)
    {
#if WINDOWS || NET462
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return System.Security.Cryptography.ProtectedData.Protect(
                data, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
#endif
        return data; // Other platforms: unencrypted file
    }

    private static byte[] Unprotect(byte[] data)
    {
#if WINDOWS || NET462
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return System.Security.Cryptography.ProtectedData.Unprotect(
                data, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
#endif
        return data;
    }
}
