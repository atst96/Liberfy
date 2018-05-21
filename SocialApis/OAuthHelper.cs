using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialApis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    internal static class OAuthHelper
    {
        public const string OAuthVersion = "1.0";

        internal static class OAuthParameterKeys
        {
            public const string ConsumerKey = "oauth_consumer_key";
            public const string Callback = "oauth_callback";
            public const string Version = "oauth_version";
            public const string SignatureMethod = "oauth_signature_method";
            public const string Signature = "oauth_signature";
            public const string Timestamp = "oauth_timestamp";
            public const string Nonce = "oauth_nonce";
            public const string Token = "oauth_token";
            public const string TokenSecret = "oauth_token_secret";
            public const string Verifier = "oauth_verifier";
        }

        public const string HMACSHA1SignatureType = "HMAC-SHA1";

        private static Random random = new Random();

        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException(nameof(hashAlgorithm));

            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            var dataBuffer = Encoding.ASCII.GetBytes(data);
            var hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        private static string GenerateSignatureBase(ITokensBase tokens, string endpoint, Query query, string httpMethod, string timeStamp, string nonce, bool cloneQuery = true)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var parameters = cloneQuery
                ? new Query(query)
                : (query ?? new Query());

            parameters[OAuthParameterKeys.Version] = OAuthVersion;
            parameters[OAuthParameterKeys.Nonce] = nonce;
            parameters[OAuthParameterKeys.Timestamp] = timeStamp;
            parameters[OAuthParameterKeys.SignatureMethod] = HMACSHA1SignatureType;
            parameters[OAuthParameterKeys.ConsumerKey] = tokens.ConsumerKey;

            if (!string.IsNullOrEmpty(tokens.ApiToken))
                parameters[OAuthParameterKeys.Token] = tokens.ApiToken;

            var normalizedRequestParameters = Query.GetOrderedQueryString(parameters);

            var signatureBase = new[]
            {
                httpMethod.ToUpper(),
                UrlEncode(endpoint),
                UrlEncode(normalizedRequestParameters)
            };

            return string.Join("&", signatureBase);
        }

        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hashAlgorithm)
        {
            return ComputeHash(hashAlgorithm, signatureBase);
        }

        public static string GenerateAuthenticationHeader(string endpoint, ITokensBase tokens, IQuery query, string httpMethod, string timeStamp, string nonce)
        {
            var urlQuery = new Query(query);

            var signatureBase = GenerateSignatureBase(tokens, endpoint, urlQuery, httpMethod, timeStamp, nonce, false);
            urlQuery[OAuthParameterKeys.Signature] = HashHMACSHA1(tokens, signatureBase);

            return string.Concat("OAuth ", string.Join(",", Query.Sort(urlQuery).Select(Query.GetParameterPairDq)));
        }

        private static DateTime _utcBasedDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static string GenerateTimeStamp()
        {
            var ts = DateTime.UtcNow - _utcBasedDateTime;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        public static string HashHMACSHA1(ITokensBase token, string signatureBase)
        {
            var algorythm = new HMACSHA1();
            var key = string.Concat(token.ConsumerSecret, "&", token.ApiTokenSecret);
            algorythm.Key = Encoding.UTF8.GetBytes(key);

            return GenerateSignatureUsingHash(signatureBase, algorythm);
        }

        public static string UrlEncode(string value)
        {
            // https://nyahoon.com/blog/1291

            const byte _0 = 0x30, _9 = 0x39;
            const byte _A = 0x41, _Z = 0x5A;
            const byte _a = 0x61, _z = 0x7A;
            const byte _hyphen = 0x2D;
            const byte _dot = 0x2E;
            const byte _unserscore = 0x5F;
            const byte _dash = 0x7E;

            var utf8str = Encoding.UTF8.GetBytes(value);
            var sb = new StringBuilder();
            foreach (byte c in utf8str)
            {
                if ((_A <= c && c <= _Z) || (_a <= c && c <= _z) // [A-Z] | [a-z]
                    || (_0 <= c && c <= _9) // [0-9]
                    || c == _hyphen || c == _dot || c == _unserscore || c == _dash) // [-._~]
                {
                    sb.Append((char)c);
                }
                else
                {
                    sb.AppendFormat("%{0:X2}", (int)c);
                }
            }
            return sb.ToString();
        }
    }
}
