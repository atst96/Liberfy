using SocialApis.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SocialApis
{
    using QueryElement = KeyValuePair<string, object>;

    internal static class OAuthHelper
    {
        public const string OAuthVersion = "1.0";

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

        public static IEnumerable<QueryElement> EnumerateOAuthParameters(string consumerKey, string apiToken, string timestamp, string nonce)
        {
            yield return new QueryElement(OAuthParameters.ConsumerKey, consumerKey);
            yield return new QueryElement(OAuthParameters.Nonce, nonce);
            yield return new QueryElement(OAuthParameters.SignatureMethod, HMACSHA1SignatureType);
            yield return new QueryElement(OAuthParameters.Timestamp, timestamp);

            if (!string.IsNullOrEmpty(apiToken))
                yield return new QueryElement(OAuthParameters.Token, apiToken);

            yield return new QueryElement(OAuthParameters.Version, OAuthVersion);
        }

        private static string BuildParameterPair(KeyValuePair<string, string> q)
        {
            return $@"{q.Key}=""{q.Value}""";
        }

        public static string GenerateAuthenticationHeader(string httpMethod, string endpoint, IApi tokens, IEnumerable<QueryElement> query, IEnumerable<QueryElement> oauthParameters)
        {
            var (nonce, timestamp) = OAuthHelper.GenerateUniqueValues();

            var sortedQuery = new SortedDictionary<string, object>(StringComparer.Ordinal);

            var parameters = (query?.Any() != true)
                ? oauthParameters
                : query.Union(oauthParameters);

            foreach (var kvp in parameters)
                sortedQuery.Add(kvp.Key, kvp.Value);

            var signatureBase = OAuthHelper.GenerateSignatureBase(httpMethod, endpoint, sortedQuery);

            sortedQuery[OAuthParameters.Signature] = OAuthHelper.GenerateSignature(tokens.ConsumerSecret, tokens.ApiTokenSecret, signatureBase);

            var queryParameters = new QueryParameterCollection(sortedQuery);
            var authHeaderParameters = queryParameters.Select(BuildParameterPair);
            var authHeaderString = string.Join(",", authHeaderParameters);

            return $"OAuth " + authHeaderString;
        }

        public static string GenerateAuthenticationHeader(string httpMethod, string endpoint, IApi tokens, IEnumerable<QueryElement> parameters)
        {
            var (nonce, timestamp) = OAuthHelper.GenerateUniqueValues();

            return GenerateAuthenticationHeader(httpMethod, endpoint, tokens, parameters, EnumerateOAuthParameters(tokens.ConsumerKey, tokens.ApiToken, timestamp, nonce));
        }

        public static (string nonce, string timestamp) GenerateUniqueValues() => (GenerateNonce(), GenerateTimestamp());

        public static string GenerateTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        private static string GenerateSignatureBase(string httpMethod, string endpoint, ICollection<QueryElement> query)
        {
            var normalizedParameters = Query.JoinParametersWithAmpersand(query);

            return string.Join("&", httpMethod, WebUtility.UrlEncode(endpoint), WebUtility.UrlEncode(normalizedParameters));
        }

        public static string GenerateSignature(string consumerSecret, string apiTokenSecret, string signatureBase)
        {
            if (string.IsNullOrEmpty(signatureBase))
                throw new ArgumentNullException(nameof(signatureBase));

            var hashKey = EncodingUtility.UTF8.GetBytes(consumerSecret + "&" + apiTokenSecret);
            using var algorythm = new HMACSHA1(hashKey);
            var data = EncodingUtility.ASCII.GetBytes(signatureBase);
            var hash = algorythm.ComputeHash(data);

            return Convert.ToBase64String(hash);
        }
    }
}
