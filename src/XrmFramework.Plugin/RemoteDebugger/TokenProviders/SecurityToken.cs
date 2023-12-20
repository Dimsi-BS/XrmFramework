// ReSharper disable once CheckNamespace

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace XrmFramework.RemoteDebugger.TokenProviders;

public class SecurityToken
  {
    private static readonly Func<string, string> Decoder = new Func<string, string>(WebUtility.UrlDecode);
    private static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private readonly string token;
    private readonly DateTime expiresAtUtc;
    private readonly string audience;
    private readonly string audienceFieldName;
    private readonly string expiresOnFieldName;
    private readonly string keyValueSeparator;
    private readonly string pairSeparator;

    /// <summary>
    /// Creates a new instance of the <see cref="T:Microsoft.Azure.Relay.SecurityToken" /> class.
    /// </summary>
    /// <param name="tokenString">A token in string format.</param>
    /// <param name="audienceFieldName">The key name for the audience field.</param>
    /// <param name="expiresOnFieldName">The key name for the expires on field.</param>
    /// <param name="keyValueSeparator">The separator between keys and values.</param>
    /// <param name="pairSeparator">The separator between different key/value pairs.</param>
    protected SecurityToken(
      string tokenString,
      string audienceFieldName,
      string expiresOnFieldName,
      string keyValueSeparator,
      string pairSeparator)
    {
      this.token = tokenString != null ? tokenString : throw new ArgumentNullException(nameof (tokenString));
      this.audienceFieldName = audienceFieldName;
      this.expiresOnFieldName = expiresOnFieldName;
      this.keyValueSeparator = keyValueSeparator;
      this.pairSeparator = pairSeparator;
      this.GetExpirationDateAndAudienceFromToken(tokenString, out this.expiresAtUtc, out this.audience);
    }

    /// <summary>Gets the audience of this token.</summary>
    public string Audience => this.audience;

    /// <summary>Gets the expiration time of this token.</summary>
    public DateTime ExpiresAtUtc => this.expiresAtUtc;

    /// <summary>Gets the actual token as a string.</summary>
    public string TokenString => this.token;

    private void GetExpirationDateAndAudienceFromToken(
      string tokenString,
      out DateTime expiresOn,
      out string audience)
    {
      IDictionary<string, string> dictionary = SecurityToken.Decode(tokenString, SecurityToken.Decoder, SecurityToken.Decoder, this.keyValueSeparator, this.pairSeparator);
      string s;
      if (!dictionary.TryGetValue(this.expiresOnFieldName, out s))
        throw new ArgumentException(nameof (tokenString), "TokenMissingExpiresOn");
      if (!dictionary.TryGetValue(this.audienceFieldName, out audience))
        throw new ArgumentException(nameof (tokenString), "TokenMissingAudience");
      expiresOn = SecurityToken.EpochTime + TimeSpan.FromSeconds(double.Parse(s, CultureInfo.InvariantCulture));
    }

    private static IDictionary<string, string> Decode(
      string tokenString,
      Func<string, string> keyDecoder,
      Func<string, string> valueDecoder,
      string keyValueSeparator,
      string pairSeparator)
    {
      IDictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string str in tokenString.Split(new[]
               {
                 pairSeparator
               }, StringSplitOptions.None))
      {
        string[] strArray = str.Split(new[]
        {
          keyValueSeparator
        }, StringSplitOptions.None);
        if (strArray.Length != 2)
          throw new ArgumentException(nameof (tokenString), @"Invalid encoding");
        dictionary.Add(keyDecoder(strArray[0]), valueDecoder(strArray[1]));
      }
      return dictionary;
    }
  }