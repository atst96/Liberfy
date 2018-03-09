using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    public enum CardType
    {
        [EnumMember(Value = "link")]  Link,
        [EnumMember(Value = "photo")] Photo,
        [EnumMember(Value = "video")] Video,
        [EnumMember(Value = "rich")]  Rich,
    }
}
