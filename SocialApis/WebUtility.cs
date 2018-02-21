using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis
{
    internal static class WebUtility
    {
        public static HttpWebRequest CreateOAuthWebRequest(string endpoint, ITokensBase tokens, Query query, string method, bool autoSetting = true)
        {
            query = query ?? new Query();
            method = method?.ToUpper() ?? "GET";

            var timeStamp = OAuthHelper.GenerateTimeStamp();
            var nonce = OAuthHelper.GenerateNonce();

            endpoint = endpoint.Split(new[] { '?', '&', '#' }, 2)[0];

            var oauthHeader = OAuthHelper.GenerateAuthenticationHeader(endpoint, tokens, query, method, timeStamp, nonce);

            if (autoSetting && method == "GET")
            {
                endpoint = $"{ endpoint.Split(new[] { '?', '&' }, 2)[0] }?{ string.Join("&", query.GetRequestParameters()) }";
            }

            var webReq = WebRequest.CreateHttp(endpoint);
            webReq.Method = method;
            webReq.Headers.Add(HttpRequestHeader.Authorization, oauthHeader);

            if (autoSetting)
            {
                if (method == "POST")
                {
                    webReq.ContentType = "application/x-www-form-urlencoded";

                    if (query != null && query.Count > 0)
                    {
                        using (var str = webReq.GetRequestStream())
                        {
                            var queryString = string.Join("&", query.GetRequestParameters());
                            var data = Encoding.UTF8.GetBytes(queryString);
                            str.Write(data, 0, data.Length);

                            data = null;
                        }
                    }
                }
            }


            return webReq;
        }

        public static async Task<string> SendRequestText(HttpWebRequest httpWebRequest)
        {
            using (var webRes = await httpWebRequest.GetResponseAsync())
            using (var sr = new StreamReader(webRes.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }

        public static async Task<T> SendRequest<T>(HttpWebRequest httpWebRequest) where T : class
        {
            using (var webRes = await httpWebRequest.GetResponseAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(webRes.GetResponseStream());
            }
        }

        public static Task<T> OAuthGet<T>(string endpoint, ITokensBase tokens, Query query) where T : class
        {
            var webReq = CreateOAuthWebRequest(endpoint, tokens, query, "get");
            return SendRequest<T>(webReq);
        }
    }
}
