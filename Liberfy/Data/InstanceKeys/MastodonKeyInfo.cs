using System.Runtime.Serialization;

namespace Liberfy.Data.InstanceKeys
{
    /// <summary>
    /// Mastodonのキー情報
    /// </summary>
    internal class MastodonKeyInfo
    {
        /// <summary>
        /// インスタンス名
        /// </summary>
        [DataMember(Name = "instance")]
        public string Instance { get; set; }

        /// <summary>
        /// クライアント名
        /// </summary>
        [DataMember(Name = "client_name")]
        public string ClientName { get; set; }

        /// <summary>
        /// クライアントID
        /// </summary>
        [DataMember(Name = "client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// クライアントシークレット
        /// </summary>
        [DataMember(Name = "client_secret")]
        public string ClientSecret { get; set; }
    }
}
