using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    internal static class WebUtility
    {
        private static readonly char[] _uriSpritCharacters = new[] { '?', '&', '#' };

        public static HttpWebRequest CreateWebRequest(string method, string endpoint, WebHeaderCollection headers, IQuery query)
        {
            return CreateWebRequest(endpoint, query, method, headers, true);
        }

        private static bool StrEq(string val1, string val2)
        {
            return val1.Equals(val2, StringComparison.OrdinalIgnoreCase);
        }

        public static HttpWebRequest CreateWebRequest(string requestUri, IQuery parameters, string method, WebHeaderCollection headers = null, bool autoSetting = true)
        {
            method = method ?? HttpMethods.GET;
            requestUri = requestUri.Split(_uriSpritCharacters, 2)[0];

            string queryString = default;

            if (autoSetting && parameters?.Any() == true)
            {
                queryString = Query.Join(parameters);

                if (StrEq(method, HttpMethods.GET) || StrEq(method, HttpMethods.DELETE))
                {
                    requestUri = string.Concat(requestUri, "?", queryString);
                }
            }

            var request = WebRequest.CreateHttp(requestUri);
            request.Method = method;

            if (headers != null)
                request.Headers = headers;

            if (autoSetting)
            {
                if (StrEq(method, HttpMethods.POST) || StrEq(method, HttpMethods.PUT))
                {
                    request.ContentType = HttpContentTypes.FormUrlEncoded;

                    if (queryString != null)
                    {
                        using (var stream = request.GetRequestStream())
                        {
                            var data = Encoding.UTF8.GetBytes(queryString);
                            stream.Write(data, 0, data.Length);

                            data = null;
                        }
                    }
                }
            }

            return request;
        }

        public static HttpWebRequest CreateOAuthRequest(string endpoint, IApi tokens, IQuery parameters, string method, bool autoSetting = true)
        {
            var oauthHeader = OAuthHelper.GenerateAuthenticationHeader(method, endpoint, tokens, parameters);

            var headers = new WebHeaderCollection
            {
                [HttpRequestHeader.Authorization] = oauthHeader,
            };
            
            return CreateWebRequest(endpoint, parameters, method, headers, autoSetting);
        }

        public static async Task<string> SendRequestText(HttpWebRequest request)
        {
            using (var response = await request.GetResponseAsync().ConfigureAwait(false))
            {
                return await StreamUtility.ReadToEndAsync(response.GetResponseStream()).ConfigureAwait(false);
            }
        }

        public static async Task SendRequestVoid(HttpWebRequest request)
        {
            await request.GetRequestStreamAsync().ConfigureAwait(false);
        }

        public static async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            using (var response = await request.GetResponseAsync().ConfigureAwait(false))
            {
                return await JsonUtility.DeserializeAsync<T>(response.GetResponseStream())
                    .ConfigureAwait(false);
            }
        }
    }
}
