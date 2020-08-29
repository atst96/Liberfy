using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;
using System.Net.Http;
using System.Net.Http.Headers;
using SocialApis.Core;

namespace SocialApis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    internal static class WebUtility
    {
        public static readonly Encoding UTF8Encoding = new UTF8Encoding(false);

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
                var parameters = queryParameters.ToStringNameValuePairs();
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
