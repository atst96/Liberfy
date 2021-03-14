using System;
using System.Net.Http;
using System.Threading.Tasks;
using Liberfy.Data.InstanceKeys;
using Liberfy.Utilieis;

namespace Liberfy.Managers
{
    /// <summary>
    /// クライアントキーの管理を行うクラス
    /// </summary>
    internal class ClientKeyManager
    {
        /// <summary>
        /// キー管理APIのURLを生成する
        /// </summary>
        /// <param name="service">サービス名</param>
        /// <param name="instance">インスタンス名</param>
        /// <returns></returns>
        private static string GetApiUrl(string service, string instance)
        {
            return $"{ Config.KeyManager.ApiEndpoint.AbsoluteUri }/{ Uri.EscapeUriString(service) }/{ Uri.EscapeUriString(instance) }/key";
        }

        /// <summary>
        /// Mastodonのキー情報を取得する
        /// </summary>
        /// <param name="instanceName"></param>
        public static async Task<MastodonKeyInfo> GetMastodonKey(string instanceName)
        {
            var endpoint = GetApiUrl("mastodon", instanceName);

            // TODO: 下記部分はSocialApisプロジェクトのアクセサを呼べるようにする
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                var response = await client.SendAsync(request).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                using var stream = response.Content.ReadAsStream();
                return await JsonUtil.DeserializeAsync<MastodonKeyInfo>(stream).ConfigureAwait(false);
            }
        }
    }
}
