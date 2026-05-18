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
/// Service d'authentification MSAL pour le RemoteDebugger.
/// Reproduit fidèlement le flux de l'application Desktop :
/// <list type="number">
///   <item>Tentative silencieuse depuis le cache (<see cref="IPublicClientApplication.AcquireTokenSilent"/>).</item>
///   <item>Si le cache est vide ou expiré, ouverture d'un sélecteur de compte Azure (<see cref="Prompt.SelectAccount"/>).</item>
/// </list>
/// Le cache est persisté dans <c>%LOCALAPPDATA%\xrmFramework\debugger.msalcache.bin3</c>
/// (chiffré avec DPAPI sur Windows, fichier brut sur les autres plateformes).
/// </summary>
internal sealed class ManagerAuthService
{
    // ── Constantes ──────────────────────────────────────────────────────────

    /// <summary>Scope identique à celui utilisé par l'application Desktop.</summary>
    private static readonly string[] Scopes = { "api://xrmFramework-manager-api/desktop-connect" };

    /// <summary>Redirect URI — même valeur que le Desktop.</summary>
    private const string RedirectUri = "http://localhost";

    // ── État ────────────────────────────────────────────────────────────────

    private readonly IPublicClientApplication _clientApp;
    private readonly Action<string> _log;

    // ── Constructeur ────────────────────────────────────────────────────────

    public ManagerAuthService(ManagerHubSettings settings, Action<string> log = null)
    {
        _log = log ?? Console.WriteLine;

        _clientApp = PublicClientApplicationBuilder
            .Create(settings.ClientId)
            .WithRedirectUri(RedirectUri)
            .WithAuthority(AzureCloudInstance.AzurePublic, settings.Tenant)
            .Build();

        // Persistance du cache de tokens (même principe que TokenCacheHelper du Desktop)
        EnableTokenCacheSerialization(_clientApp.UserTokenCache);
    }

    // ── API publique ────────────────────────────────────────────────────────

    /// <summary>
    /// Obtient un token d'accès valide pour le Manager.
    /// Tente d'abord le cache silencieux ; affiche le sélecteur de compte si nécessaire.
    /// Retourne <c>null</c> si l'authentification échoue (erreur loggée).
    /// </summary>
    public async Task<string> AcquireTokenAsync()
    {
        var accounts = await _clientApp.GetAccountsAsync();
        var firstAccount = accounts.FirstOrDefault();

        // 1. Tentative silencieuse (cache)
        try
        {
            var result = await _clientApp
                .AcquireTokenSilent(Scopes, firstAccount)
                .ExecuteAsync();

            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // Cache vide ou expiré → login interactif
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Erreur d'authentification silencieuse : {ex.Message}");
            return null;
        }

        // 2. Login interactif — identique au Desktop (sélecteur de compte)
        try
        {
            _log("[ManagerHub] Authentification requise — ouverture du sélecteur de compte Azure…");

            var result = await _clientApp
                .AcquireTokenInteractive(Scopes)
                .WithAccount(firstAccount)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync();

            _log($"[ManagerHub] Authentifié en tant que {result.Account?.Username}.");
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _log($"[ManagerHub] Authentification interactive échouée : {ex.Message}");
            return null;
        }
    }

    // ── Cache de tokens ─────────────────────────────────────────────────────

    /// <summary>
    /// Chemin du fichier de cache MSAL du RemoteDebugger.
    /// Distinct du cache Desktop (assemblies différents), mais dans le même
    /// répertoire utilisateur pour rester cohérent.
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
                // Cache illisible ou corrompu : on repart de zéro
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
                // Échec de persistance non bloquant
            }
        }
    }

    // ── Chiffrement DPAPI (Windows) / identité (autres plateformes) ─────────

    private static byte[] Protect(byte[] data)
    {
#if WINDOWS || NET462
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return System.Security.Cryptography.ProtectedData.Protect(
                data, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
#endif
        return data; // Autres plateformes : fichier non chiffré
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
