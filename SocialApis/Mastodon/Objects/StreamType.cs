using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    /// <summary>
    /// ストリーミングの種類
    /// </summary>
    public enum StreamType
    {
        /// <summary>
        /// </summary>
        [EnumMember(Value = "user")]
        User,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "public")]
        Public,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "public:local")]
        PublicLocal,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "hashtag")]
        Hashtag,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "hashtag:local")]
        HashtagLocal,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "list")]
        List,

        /// <summary>
        /// </summary>
        [EnumMember(Value = "direct")]
        Direct,
    }
}
