using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    internal static class WebUtility
    {
        private static readonly char[] UrlSpritCharacters = new[] { '?', '&', '#' };

        public static HttpWebRequest CreateWebRequest(string endpoint, IQuery query, string method, WebHeaderCollection headers = null, bool autoSetting = true)
        {
            query = query ?? new Query();
            method = method?.ToUpper() ?? RESTfulAPIMethods.Get;
            endpoint = endpoint.Split(UrlSpritCharacters, 2)[0];

            var queryString = default(string);
            if (autoSetting)
            {
                queryString = Query.GetQueryString(query);
                
                if (method == RESTfulAPIMethods.Get || method == RESTfulAPIMethods.Delete)
                {
                    endpoint = string.Concat(endpoint, "?", queryString);
                }
            }

            var webReq = WebRequest.CreateHttp(endpoint);
            webReq.Method = method;
            if (headers != null)
                webReq.Headers = headers;

            if (autoSetting)
            {
                if (method == RESTfulAPIMethods.Post || method == RESTfulAPIMethods.Put)
                {
                    webReq.ContentType = "application/x-www-form-urlencoded";

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        using (var str = webReq.GetRequestStream())
                        {
                            var data = Encoding.UTF8.GetBytes(queryString);
                            str.Write(data, 0, data.Length);

                            data = null;
                        }
                    }
                }
            }

            return webReq;
        }

        public static HttpWebRequest CreateOAuthWebRequest(string endpoint, ITokensBase tokens, IQuery query, string method, bool autoSetting = true)
        {
            query = query ?? new Query();

            var timeStamp = OAuthHelper.GenerateTimeStamp();
            var nonce = OAuthHelper.GenerateNonce();

            var oauthHeader = OAuthHelper.GenerateAuthenticationHeader(endpoint, tokens, query, method, timeStamp, nonce);

            var headers = new WebHeaderCollection
            {
                [HttpRequestHeader.Authorization] = oauthHeader,
            };

            return CreateWebRequest(endpoint, query, method, headers, autoSetting);
        }

        public static async Task<string> SendRequestText(HttpWebRequest httpWebRequest)
        {
            using (var webRes = await httpWebRequest.GetResponseAsync())
            using (var sr = new StreamReader(webRes.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        public static async Task SendRequestVoid(HttpWebRequest httpWebRequest)
        {
            await httpWebRequest.GetRequestStreamAsync();
        }

        public static async Task<T> SendRequest<T>(HttpWebRequest httpWebRequest) where T : class
        {
            using (var webRes = await httpWebRequest.GetResponseAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(webRes.GetResponseStream(),
                    Utf8Json.Resolvers.StandardResolver.AllowPrivate);
            }
        }
    }
}
