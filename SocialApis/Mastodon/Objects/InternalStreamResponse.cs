using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    /// <summary>
    /// ストリーム受信時のレスポンス
    /// </summary>
    internal class InternalStreamResponse
    {
        /// <summary>
        /// イベント種別
        /// </summary>
        [DataMember(Name = "event")]
        public string Event { get; set; }

        /// <summary>
        /// ペイロード
        /// </summary>
        [DataMember(Name = "payload")]
        public string Payload { get; set; }
    }
}
