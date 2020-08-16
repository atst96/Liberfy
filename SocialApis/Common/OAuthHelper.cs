using SocialApis.Common;
using SocialApis.Core;
using SocialApis.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http;
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
        private static readonly ICollection<byte> _urlEncodingReservedCharBytes;

        static OAuthHelper()
        {
            _urlEncodingReservedCharBytes = GetUrlEncodingIgnoreChars();
        }

        public static IEnumerable<QueryElement> EnumerateOAuthParameters(string consumerKey, string apiToken, string timestamp, string nonce)
        {
            var items = new List<QueryElement>(10)
            {
                new QueryElement(OAuthParameters.ConsumerKey, consumerKey),
                new QueryElement(OAuthParameters.Nonce, nonce),
                new QueryElement(OAuthParameters.SignatureMethod, HMACSHA1SignatureType),
                new QueryElement(OAuthParameters.Timestamp, timestamp),
            };

            if (!string.IsNullOrEmpty(apiToken))
            {
                items.Add(new QueryElement(OAuthParameters.Token, apiToken));
            }

            items.Add(new QueryElement(OAuthParameters.Version, OAuthVersion));

            return items;
        }

        private class KeyValueComparer : IComparer<KeyValuePair<string, string>>
        {
            private static readonly IComparer<string> _stringComparer = StringComparer.Ordinal;

            private static KeyValueComparer _instance;
            public static KeyValueComparer Instance => _instance ??= new KeyValueComparer();

            public int Compare(KeyValuePair<string, string> x, KeyValuePair<string, string> y)
            {
                int res = _stringComparer.Compare(x.Key, y.Key);
                if (res != 0)
                {
                    return res;
                }

                return _stringComparer.Compare(x.Value, y.Value);
            }
        }

        public static string GenerateAuthenticationString(HttpMethod method, Uri endpoint, IApi tokens, IEnumerable<QueryElement> query, IEnumerable<QueryElement> oauthParameters)
        {
            var parameters = oauthParameters
                .Concat(query ?? Enumerable.Empty<QueryElement>())
                .ToStringNameValuePairs()
                .UrlEncode();

            var signatureBase = OAuthHelper.GenerateSignatureBase(method, endpoint, parameters);
            var signature = OAuthHelper.GenerateSignature(tokens.ConsumerSecret, tokens.ApiTokenSecret, signatureBase);

            var authHeaderString = GeneareteAuthenticationString(parameters, signature);
            return authHeaderString;
        }

        private static string GeneareteAuthenticationString(IEnumerable<KeyValuePair<string, string>> parameters, string signature)
        {
            var authenticationParameters = parameters
                .Append(new KeyValuePair<string, string>(UrlEncode(OAuthParameters.Signature), UrlEncode(signature)))
                .OrderBy(item => item, KeyValueComparer.Instance)
                .Select(kvp => JoinParameter(kvp));

            return string.Join(",", authenticationParameters);
        }

        public static string GenerateAuthenticationString(HttpMethod method, Uri endpoint, IApi tokens, IEnumerable<QueryElement> parameters = null)
        {
            var (nonce, timestamp) = OAuthHelper.GenerateUniqueValues();

            var oauthParameters = OAuthHelper.EnumerateOAuthParameters(tokens.ConsumerKey, tokens.ApiToken, timestamp, nonce);
            return OAuthHelper.GenerateAuthenticationString(method, endpoint, tokens, parameters, oauthParameters);
        }

        public static (string nonce, string timestamp) GenerateUniqueValues() => (OAuthHelper.GenerateNonce(), OAuthHelper.GenerateTimestamp());

        public static string GenerateTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        public static string GenerateNonce()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }

        private static string JoinParameter(KeyValuePair<string, string> kvp, string separator = "=")
            => string.Concat(kvp.Key, separator, kvp.Value);

        internal static string GenerateSignatureBase(HttpMethod method, Uri endpoint, IEnumerable<KeyValuePair<string, string>> urlEncodedParmaeters)
        {
            var parameters = urlEncodedParmaeters
                .OrderBy(item => item, KeyValueComparer.Instance)
                .Select(kvp => JoinParameter(kvp));
            var joinedParameters = string.Join("&", parameters);

            return string.Join("&", method.Method, UrlEncode(endpoint.AbsoluteUri), UrlEncode(joinedParameters));
        }

        public static string GenerateSignature(string consumerSecret, string apiTokenSecret, string signatureBase)
        {
            if (string.IsNullOrEmpty(signatureBase))
            {
                throw new ArgumentNullException(nameof(signatureBase));
            }

            var hashKey = EncodingUtil.UTF8.GetBytes(consumerSecret + "&" + apiTokenSecret);
            using var algorythm = new HMACSHA1(hashKey);
            var data = EncodingUtil.ASCII.GetBytes(signatureBase);
            var hash = algorythm.ComputeHash(data);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// URLエンコード時に処理しない文字列
        /// </summary>
        /// <returns></returns>
        private static ICollection<byte> GetUrlEncodingIgnoreChars()
        {
            var ignoreSymbolChars = new byte[]
            {
                // -
                0x2D,
                // .
                0x2E,
                // _
                0x5F,
                // ~
                0x7E,
                //// !
                //0x21,
                //// *
                //0x2A,
                //// (
                //0x28,
                //// )
                //0x29
            };

            var ignoreChars = ignoreSymbolChars
                // 0 ~ 9
                .Concat(RangeUtil.Enumerate(0x30, 0x39))
                // a ~ z
                .Concat(RangeUtil.Enumerate(0x41, 0x5A))
                // A ~ Z
                .Concat(RangeUtil.Enumerate(0x61, 0x7A));

            var chars = new HashSet<byte>(100);
            chars.UnionWith(ignoreChars);

            return chars;
        }

        /// <summary>
        /// 文字列をパーセントエンコードする。
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> UrlEncode(this in KeyValuePair<string, string> kvp)
            => KeyValuePair.Create(UrlEncode(kvp.Key), UrlEncode(kvp.Value));

        /// <summary>
        /// 文字列をパーセントエンコードする。
        /// </summary>
        /// <param name="parameters"><see cref="KeyValuePair"/></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> UrlEncode(this IEnumerable<KeyValuePair<string, string>> parameters)
            => parameters.Select(kvp => KeyValuePair.Create(UrlEncode(kvp.Key), UrlEncode(kvp.Value)));

        /// <summary>
        /// 文字列をパーセントエンコードする。
        /// </summary>
        /// <param name="content">パーセントエンコードする文字列</param>
        /// <returns>パーセントエンコード済み文字列</returns>
        public static string UrlEncode(string content)
        {
            if (content.Length == 0)
            {
                return content;
            }

            byte[] data = EncodingUtil.UTF8.GetBytes(content);
            byte[] bytes = new byte[data.Length * 3];
            int pos = 0;

            for (int index = 0; index < data.Length; ++index)
            {
                byte c = data[index];

                if (_urlEncodingReservedCharBytes.Contains(c))
                {
                    // エンコードを無視
                    bytes[pos++] = c;
                }
                else
                {
                    // その他の文字列
                    bytes[pos++] = (byte)'%';
                    bytes[pos++] = ToHexByte(c >> 4);
                    bytes[pos++] = ToHexByte(c & 0x0F);
                }
            }

            var result = Encoding.ASCII.GetString(bytes[0..pos]);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte ToHexByte(int value)
        {
            int c = value < 10 ? (value + '0') : (value - 10 + 'A');
            return unchecked((byte)c);
        }
    }
}
