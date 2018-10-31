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
    using QueryElement = KeyValuePair<string, object>;
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    internal static class OAuthHelper
    {
        public const string OAuthVersion = "1.0";

        static OAuthHelper()
        {
            var hashset = new HashSet<byte>();

            hashset.Add(0x2D);
            hashset.Add(0x2E);

            for (byte c = 0x30; c <= 0x39; ++c)
                hashset.Add(c);

            for (byte c = 0x41; c <= 0x5A; ++c)
                hashset.Add(c);

            hashset.Add(0x5F);

            for (byte c = 0x61; c <= 0x7A; ++c)
                hashset.Add(c);

            hashset.Add(0x7E);

            _ordinaryCharacters = hashset;
        }

        private static readonly HashSet<byte> _ordinaryCharacters;

        internal static class OAuthParameters
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

        private static IQuery UnionOAuthParameters(ITokensBase tokens, IQuery query, string timeStamp, string nonce)
        {
            if (query == null)
                query = Enumerable.Empty<QueryElement>();

            return query.Union(EnumerateOAuthParameters(tokens, timeStamp, nonce));
        }

        private static IQuery EnumerateOAuthParameters(ITokensBase tokens, string timeStamp, string nonce)
        {
            yield return new QueryElement(OAuthParameters.Version, OAuthVersion);
            yield return new QueryElement(OAuthParameters.Nonce, nonce);
            yield return new QueryElement(OAuthParameters.Timestamp, timeStamp);
            yield return new QueryElement(OAuthParameters.SignatureMethod, HMACSHA1SignatureType);
            yield return new QueryElement(OAuthParameters.ConsumerKey, tokens.ConsumerKey);

            if (!string.IsNullOrEmpty(tokens.ApiToken))
            {
                yield return new QueryElement(OAuthParameters.Token, tokens.ApiToken);
            }
        }

        private static string GenerateSignatureBase(ITokensBase tokens, string endpoint, SortedQuery query, string httpMethod)
        {
            if (tokens == null)
                throw new ArgumentNullException(nameof(tokens));

            var normalizedParameters = Query.GetQueryString(query);

            return string.Join("&", httpMethod.ToUpper(), UrlEncode(endpoint), UrlEncode(normalizedParameters));
        }

        public static string GenerateAuthenticationHeader(string endpoint, ITokensBase tokens, IQuery query, string httpMethod)
        {
            var timeStamp = GenerateTimeStamp();
            var nonce = GenerateNonce();

            var urlQuery = new SortedQuery(UnionOAuthParameters(tokens, query, timeStamp, nonce));
            var signatureBase = GenerateSignatureBase(tokens, endpoint, urlQuery, httpMethod);

            urlQuery[OAuthParameters.Signature] = GenerateSignature(tokens, signatureBase);

            return "OAuth " + string.Join(",", urlQuery.Select(Query.GetParameterPairDq));
        }

        public static string GenerateTimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        public static string GenerateSignature(ITokensBase token, string signatureBase)
        {
            if (string.IsNullOrEmpty(signatureBase))
                throw new ArgumentNullException(nameof(signatureBase));

            var hashKey = Encoding.UTF8.GetBytes(string.Concat(token.ConsumerSecret, "&", token.ApiTokenSecret));
            var data = Encoding.ASCII.GetBytes(signatureBase);

            var algorythm = new HMACSHA1(hashKey);

            return Convert.ToBase64String(algorythm.ComputeHash(data));
        }

        public static string UrlEncode(string value)
        {
            // 参考: https://nyahoon.com/blog/1291

            var data = Encoding.UTF8.GetBytes(value);
            var sb = new StringBuilder(data.Length * 3);

            foreach (byte c in data)
            {
                if (_ordinaryCharacters.Contains(c))
                {
                    sb.Append((char)c);
                }
                else
                {
                    sb.Append("%" + c.ToString("X2"));
                }
            }

            return sb.ToString();
        }
    }
}
