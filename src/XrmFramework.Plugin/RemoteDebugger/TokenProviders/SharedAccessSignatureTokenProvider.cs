using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace XrmFramework.RemoteDebugger.TokenProviders;

/// <summary>
/// The SharedAccessSignatureTokenProvider generates tokens using a shared access key or existing signature.
/// </summary>
internal class SharedAccessSignatureTokenProvider : TokenProvider
{
  public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
  private readonly byte[] _encodedSharedAccessKey;
  private readonly string _keyName;
  private readonly string _sharedAccessSignature;

  internal SharedAccessSignatureTokenProvider(string sharedAccessSignature)
  {
    SharedAccessSignatureTokenProvider.SharedAccessSignatureToken.Validate(sharedAccessSignature);
    this._sharedAccessSignature = sharedAccessSignature;
  }

  internal SharedAccessSignatureTokenProvider(string keyName, string sharedAccessKey)
    : this(keyName, sharedAccessKey, TokenProvider.MessagingTokenProviderKeyEncoder)
  {
  }

  protected SharedAccessSignatureTokenProvider(
    string keyName,
    string sharedAccessKey,
    Func<string, byte[]> customKeyEncoder)
  {
    if (string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(sharedAccessKey))
      throw new ArgumentNullException(string.IsNullOrEmpty(keyName) ? nameof (keyName) : nameof (sharedAccessKey));
    if (keyName.Length > 256)
      throw new ArgumentOutOfRangeException(nameof (keyName));
    if (sharedAccessKey.Length > 256)
      throw new ArgumentOutOfRangeException(nameof (sharedAccessKey));
    this._keyName = keyName;
    this._encodedSharedAccessKey = customKeyEncoder != null ? customKeyEncoder(sharedAccessKey) : TokenProvider.MessagingTokenProviderKeyEncoder(sharedAccessKey);
  }

  protected override Task<SecurityToken> OnGetTokenAsync(string resource, TimeSpan validFor) => Task.FromResult<SecurityToken>(new SharedAccessSignatureTokenProvider.SharedAccessSignatureToken(this.BuildSignature(resource, validFor)));

  protected virtual string BuildSignature(string resource, TimeSpan validFor) => string.IsNullOrWhiteSpace(this._sharedAccessSignature) ? SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.BuildSignature(this._keyName, this._encodedSharedAccessKey, resource, validFor) : this._sharedAccessSignature;

  private static class SharedAccessSignatureBuilder
  {
    public static string BuildSignature(
      string keyName,
      byte[] encodedSharedAccessKey,
      string resource,
      TimeSpan timeToLive)
    {
      string str1 = SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.BuildExpiresOn(timeToLive);
      string str2 = WebUtility.UrlEncode(resource);
      string str3 = SharedAccessSignatureTokenProvider.SharedAccessSignatureBuilder.Sign(string.Join("\n", new List<string>()
      {
        str2,
        str1
      }), encodedSharedAccessKey);
      return string.Format(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}&{7}={8}", "SharedAccessSignature", "sr", str2, "sig", WebUtility.UrlEncode(str3), "se", WebUtility.UrlEncode(str1), "skn", WebUtility.UrlEncode(keyName));
    }

    private static string BuildExpiresOn(TimeSpan timeToLive) => Convert.ToString(Convert.ToInt64(DateTime.UtcNow.Add(timeToLive).Subtract(SharedAccessSignatureTokenProvider.EpochTime).TotalSeconds, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

    private static string Sign(string requestString, byte[] encodedSharedAccessKey)
    {
      using (HMACSHA256 hmacshA256 = new HMACSHA256(encodedSharedAccessKey))
        return Convert.ToBase64String(hmacshA256.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
    }
  }

  /// <summary>
  /// A WCF SecurityToken that wraps a Shared Access Signature
  /// </summary>
  private class SharedAccessSignatureToken : SecurityToken
  {
    public const int MaxKeyNameLength = 256;
    public const int MaxKeyLength = 256;
    public const string SharedAccessSignature = "SharedAccessSignature";
    public const string SignedResource = "sr";
    public const string Signature = "sig";
    public const string SignedKeyName = "skn";
    public const string SignedExpiry = "se";
    public const string SignedResourceFullFieldName = "SharedAccessSignature sr";
    public const string SasKeyValueSeparator = "=";
    public const string SasPairSeparator = "&";

    public SharedAccessSignatureToken(string tokenString)
      : base(tokenString, "SharedAccessSignature sr", "se", "=", "&")
    {
    }

    internal static void Validate(string sharedAccessSignature)
    {
      IDictionary<string, string> dictionary = !string.IsNullOrEmpty(sharedAccessSignature) ? SharedAccessSignatureTokenProvider.SharedAccessSignatureToken.ExtractFieldValues(sharedAccessSignature) : throw new ArgumentNullException(nameof (sharedAccessSignature));
      if (!dictionary.TryGetValue("sig", out string _))
        throw new ArgumentNullException("sig");
      if (!dictionary.TryGetValue("se", out string _))
        throw new ArgumentNullException("se");
      if (!dictionary.TryGetValue("skn", out string _))
        throw new ArgumentNullException("skn");
      if (!dictionary.TryGetValue("sr", out string _))
        throw new ArgumentNullException("sr");
    }

    private static IDictionary<string, string> ExtractFieldValues(string sharedAccessSignature)
    {
      string[] strArray1 = sharedAccessSignature.Split();
      if (!string.Equals(strArray1[0].Trim(), "SharedAccessSignature", StringComparison.OrdinalIgnoreCase) || strArray1.Length != 2)
        throw new ArgumentNullException(nameof (sharedAccessSignature));
      IDictionary<string, string> fieldValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      string str1 = strArray1[1].Trim();
      string[] separator = new string[1]{ "&" };
      foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
      {
        if (str2 != string.Empty)
        {
          string[] strArray2 = str2.Split(new string[1]
          {
            "="
          }, StringSplitOptions.None);
          if (string.Equals(strArray2[0], "sr", StringComparison.OrdinalIgnoreCase))
            fieldValues.Add(strArray2[0], strArray2[1]);
          else
            fieldValues.Add(strArray2[0], WebUtility.UrlDecode(strArray2[1]));
        }
      }
      return fieldValues;
    }
  }
}