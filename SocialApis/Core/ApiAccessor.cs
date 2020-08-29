using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SocialApis.Utils;

namespace SocialApis.Core
{
    public abstract class ApiAccessor
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ApiAccessor()
        {
            this._httpClient = WebFactory.CreateHttpClient();
        }

        internal HttpClient InternalHttpClient => this._httpClient;

        /// <summary>
        /// JSON APIにリクエストを送信する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">リクエスト</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        internal async Task<T> SendRequest<T>(HttpRequestMessage request, CancellationToken? cancellationToken = default)
            where T : class
        {
            var client = this._httpClient;
            var resposne = cancellationToken.HasValue
                ? await client.SendAsync(request, cancellationToken.Value).ConfigureAwait(false)
                : await client.SendAsync(request).ConfigureAwait(false);

            if (!resposne.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(resposne);
                return default;
            }

            return this.OnWebRequestSucceed<T>(resposne);
        }

        /// <summary>
        /// JSON APIにリクエストを送信する。
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task SendJsonResponse(HttpRequestMessage request, CancellationToken? cancellationToken = default)
        {
            var client = this._httpClient;
            var resposne = cancellationToken.HasValue
                ? await client.SendAsync(request).ConfigureAwait(false)
                : await client.SendAsync(request, cancellationToken.Value).ConfigureAwait(false);

            if (!resposne.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(resposne);
                return;
            }
        }

        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var response = await this._httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(response);
            }

            return response;
        }


        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
        {
            var response = await this._httpClient.SendAsync(request, completionOption).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(response);
            }

            return response;
        }


        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
        {
            var response = await this._httpClient.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(response);
            }

            return response;
        }


        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await this._httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this.OnWebRequestFailed(response);
            }

            return response;
        }

        /// <summary>
        /// JSON APIへのリクエスト成功時
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual T OnWebRequestSucceed<T>(HttpResponseMessage response)
            where T : class
        {
            var content = response.Content;
            var data = content.ReadAsByteArrayAsync().WaitResult();

            if (typeof(T) == typeof(string))
            {
                var text = EncodingUtil.UTF8.GetString(data);
                return text as T;
            }

            using var stream = new MemoryStream(data);
            try
            {
                return JsonUtil.Deserialize<T>(stream);
            }
            catch (Utf8Json.JsonParsingException)
            {
                stream.Position = 0;

                var text = EncodingUtil.UTF8.GetString(data);
                Debug.Write("== Json parse error. ==");
                Debug.WriteLine(text);

                throw;
            }
        }

        /// <summary>
        /// JSON APIへのリクエスト失敗時
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual void OnWebRequestFailed(HttpResponseMessage response)
            => response.EnsureSuccessStatusCode();

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~ApiAccessor()
        {
            this._httpClient?.Dispose();
        }
    }
}
