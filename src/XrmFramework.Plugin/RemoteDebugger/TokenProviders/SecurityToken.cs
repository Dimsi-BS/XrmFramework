// ReSharper disable once CheckNamespace

using System.Globalization;
using System.Net;

namespace XrmFramework.RemoteDebugger.TokenProviders;

public class SecurityToken
  {
    private static readonly Func<string, string> Decoder = WebUtility.UrlDecode;
    private static readonly DateTime EpochTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private readonly string _token;
    private readonly DateTime _expiresAtUtc;
    private readonly string _audience;
    private readonly string _audienceFieldName;
    private readonly string _expiresOnFieldName;
    private readonly string _keyValueSeparator;
    private readonly string _pairSeparator;

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
      this._token = tokenString != null ? tokenString : throw new ArgumentNullException(nameof (tokenString));
      this._audienceFieldName = audienceFieldName;
      this._expiresOnFieldName = expiresOnFieldName;
      this._keyValueSeparator = keyValueSeparator;
      this._pairSeparator = pairSeparator;
      this.GetExpirationDateAndAudienceFromToken(tokenString, out this._expiresAtUtc, out this._audience);
    }

    /// <summary>Gets the audience of this token.</summary>
    public string Audience => this._audience;

    /// <summary>Gets the expiration time of this token.</summary>
    public DateTime ExpiresAtUtc => this._expiresAtUtc;

    /// <summary>Gets the actual token as a string.</summary>
    public string TokenString => this._token;

    private void GetExpirationDateAndAudienceFromToken(
      string tokenString,
      out DateTime expiresOn,
      out string audience)
    {
      IDictionary<string, string> dictionary = SecurityToken.Decode(tokenString, SecurityToken.Decoder, SecurityToken.Decoder, this._keyValueSeparator, this._pairSeparator);
      string s;
      if (!dictionary.TryGetValue(this._expiresOnFieldName, out s))
        throw new ArgumentException(@"TokenMissingExpiresOn", nameof (tokenString));
      if (!dictionary.TryGetValue(this._audienceFieldName, out audience))
        throw new ArgumentException(@"TokenMissingAudience", nameof (tokenString));
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
          throw new ArgumentException(@"Invalid encoding", nameof (tokenString));
        dictionary.Add(keyDecoder(strArray[0]), valueDecoder(strArray[1]));
      }
      return dictionary;
    }
  }