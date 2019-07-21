using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    internal static class WebUtility
    {
        public static readonly Encoding UTF8Encoding = new UTF8Encoding(false);
        private static readonly char[] _uriSpritCharacters = new[] { '?', '&', '#' };
        private static readonly HashSet<byte> _urlSafeCharacters;

        static WebUtility()
        {
            var urlSafeCharacters = new HashSet<byte>
            {
                // '-', '.', '_', '!', '*', '(', ')',
                0x2D, 0x2E, 0x5F, 0x21, 0x2A, 0x28, 0x29
            };

            // '0' ~ '9'
            for (byte c = 0x30; c <= 0x39; ++c)
            {
                urlSafeCharacters.Add(c);
            }

            // 'a' ~ 'z'
            for (byte c = 0x41; c <= 0x5A; ++c)
            {
                urlSafeCharacters.Add(c);
            }

            // 'A' ~ 'Z'
            for (byte c = 0x61; c <= 0x7A; ++c)
            {
                urlSafeCharacters.Add(c);
            }

            _urlSafeCharacters = urlSafeCharacters;
        }

        public static HttpWebRequest CreateWebRequest(string method, string requestUri, IQuery parameters, WebHeaderCollection headers = null)
        {
            requestUri = requestUri.Split(_uriSpritCharacters).First();
            method = method ?? HttpMethods.GET;

            string queryString = default;

            if (parameters?.Any() ?? false)
            {
                queryString = Query.JoinParametersWithAmpersand(parameters);

                if (method == HttpMethods.GET || method == HttpMethods.DELETE)
                {
                    requestUri = string.Concat(requestUri, "?", queryString);
                }
            }

            var request = CreateWebRequestSimple(method, requestUri, headers);

            if (method == HttpMethods.POST || method == HttpMethods.PUT)
            {
                request.ContentType = HttpContentTypes.FormUrlEncoded;

                if (queryString != null)
                {
                    using var stream = request.GetRequestStream();
                    var data = EncodingUtility.UTF8.GetBytes(queryString);

                    stream.Write(data, 0, data.Length);
                }
            }

            return request;
        }

        public static HttpWebRequest CreateWebRequestSimple(string method, string requestUri, WebHeaderCollection headers = null)
        {
            var request = WebRequest.CreateHttp(new Uri(requestUri, UriKind.Absolute));

            request.Method = method ?? HttpMethods.GET;

            if (headers != null)
            {
                request.Headers = headers;
            }

            return request;
        }

        public static HttpWebRequest CreateOAuthRequest(string method, string endpoint, IApi tokens, IQuery parameters)
        {
            var headers = new WebHeaderCollection
            {
                [HttpRequestHeader.Authorization] = OAuthHelper.GenerateAuthenticationHeader(method, endpoint, tokens, parameters),
            };

            return CreateWebRequest(method, endpoint, parameters, headers);
        }

        public static HttpWebRequest CreateOAuthRequestSimple(string method, string endpoint, IApi tokens, IQuery parameters)
        {
            var headers = new WebHeaderCollection
            {
                [HttpRequestHeader.Authorization] = OAuthHelper.GenerateAuthenticationHeader(method, endpoint, tokens, parameters),
            };

            return CreateWebRequestSimple(method, endpoint, headers);
        }

        public static async Task<string> SendRequestText(HttpWebRequest request)
        {
            using var response = await request.GetResponseAsync().ConfigureAwait(false);

            return await response.GetResponseStream().ReadToEndAsync().ConfigureAwait(false);
        }

        public static async Task SendRequest(HttpWebRequest request)
        {
            await request.GetRequestStreamAsync().ConfigureAwait(false);
        }

        public static async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            using var response = await request.GetResponseAsync().ConfigureAwait(false);

            return await JsonUtility.DeserializeAsync<T>(response.GetResponseStream()).ConfigureAwait(false);
        }

        public static string UrlEncode(string value)
        {
            var data = UTF8Encoding.GetBytes(value);
            var sb = new StringBuilder(data.Length * 3);

            foreach (byte c in data)
            {
                if (_urlSafeCharacters.Contains(c))
                {
                    sb.Append((char)c);
                }
                else
                {
                    sb.Append('%').Append(c.ToString("X2"));
                }
            }

            return sb.ToString();
        }
    }
}
