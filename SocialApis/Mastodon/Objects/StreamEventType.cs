using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    /// <summary>
    /// </summary>
    public enum StreamEventType
    {
        /// <summary>
        /// 新規トゥート
        /// </summary>
        [EnumMember(Value = "update")]
        Update,

        /// <summary>
        /// 通知
        /// </summary>
        [EnumMember(Value = "notification")]
        Notification,

        /// <summary>
        /// トゥート削除
        /// </summary>
        [EnumMember(Value = "delete")]
        Delete,

        /// <summary>
        /// キーワードフィルター変更
        /// </summary>
        [EnumMember(Value = "filters_changed")]
        FilterChanged,
    }
}
