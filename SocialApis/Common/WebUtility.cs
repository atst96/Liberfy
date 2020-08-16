using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SocialApis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    internal static class WebUtility
    {
        public static readonly Encoding UTF8Encoding = new UTF8Encoding(false);
        private static readonly char[] _uriSplitCharacters = new[] { '?', '&', '#' };


        public static HttpRequestMessage CreateWebRequest(HttpMethod method, Uri requestUri, IQuery queryParameters, WebHeaderCollection headers = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            var endpoint = requestUri.GetLeftPart(UriPartial.Path);
            var content = default(HttpContent);

            if (queryParameters?.Count > 0)
            {
                var parameters = new QueryParameterCollection(queryParameters);
                if (method == HttpMethod.Get)
                {
                    using var formUrlContent = new FormUrlEncodedContent(parameters);
                    var queryParameter = formUrlContent.ReadAsStringAsync().WaitResult();
                    endpoint = string.Concat(endpoint, "?", queryParameter);
                }
                else
                {
                    content = new FormUrlEncodedContent(parameters);
                }
            }

            var request = CreateWebRequestSimple(method, endpoint, headers);
            request.Content = content;

            return request;
        }

        public static HttpRequestMessage CreateWebRequest(HttpMethod method, Uri requestUri, HttpContent content, WebHeaderCollection headers = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            if (requestUri == null)
            {
                throw new ArgumentNullException(nameof(requestUri));
            }

            var endpoint = requestUri.GetLeftPart(UriPartial.Path);
            var request = CreateWebRequestSimple(method, endpoint, headers);
            request.Content = content;

            return request;
        }

        public static HttpWebRequest CreateWebRequest(string method, string requestUri, IQuery parameters, WebHeaderCollection headers = null)
        {
            requestUri = requestUri.Split(_uriSplitCharacters).First();
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
                    var data = EncodingUtil.UTF8.GetBytes(queryString);

                    stream.Write(data, 0, data.Length);
                }
            }

            return request;
        }

        public static HttpRequestMessage CreateWebRequestSimple(HttpMethod method, string requestUri, WebHeaderCollection headers = null)
        {
            var request = new HttpRequestMessage(method, requestUri);

            if ((headers?.Count ?? 0) > 0)
            {
                for (int index = 0; index < headers.Count; ++index)
                {
                    var key = headers.GetKey(index);
                    var values = headers.GetValues(index);

                    request.Headers.Add(key, values);
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

        public static HttpRequestMessage CreateOAuthRequest(HttpMethod method, Uri endpoint, IApi tokens, IQuery queryParameters = null)
        {
            var request = CreateWebRequest(method, endpoint, queryParameters);
            var headers = request.Headers;

            var authenticationString = OAuthHelper.GenerateAuthenticationString(method, endpoint, tokens, queryParameters);
            headers.Authorization = new AuthenticationHeaderValue("OAuth", authenticationString);

            return request;
        }

        public static HttpRequestMessage CreateOAuthRequest(HttpMethod method, Uri endpoint, IApi tokens, HttpContent content)
        {
            var request = CreateWebRequest(method, endpoint, content);
            var headers = request.Headers;

            var authenticationString = OAuthHelper.GenerateAuthenticationString(method, endpoint, tokens);
            headers.Authorization = new AuthenticationHeaderValue("OAuth", authenticationString);

            return request;
        }

        public static Task SendRequest(HttpWebRequest request)
        {
            return request.GetRequestStreamAsync();
        }

        public static async Task<T> SendRequest<T>(HttpWebRequest request) where T : class
        {
            using var response = await request.GetResponseAsync().ConfigureAwait(false);

            return await JsonUtil.DeserializeAsync<T>(response.GetResponseStream()).ConfigureAwait(false);
        }

        public static IDictionary<string, string> ParseQueryString(string content)
        {
            return content.Split('&')
                .Select(str => str.Split('=', 2))
                .ToDictionary(
                    str => str[0],
                    str => str.Length == 1 ? string.Empty : str[1]);
        }
    }
}
