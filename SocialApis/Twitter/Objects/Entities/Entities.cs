using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct Entities
    {
        [DataMember(Name = "hashtags")]
        public HashtagEntity[] Hashtags { get; private set; }

        [DataMember(Name = "urls")]
        public UrlEntity[] Urls { get; private set; }

        [DataMember(Name = "user_mentions")]
        public UserMentionEntity[] UserMentions { get; private set; }

        [DataMember(Name = "symbols")]
        public HashtagEntity[] Symbols { get; private set; }

        [DataMember(Name = "media")]
        public MediaEntity[] Media { get; private set; }
    }
}
