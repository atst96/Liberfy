using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Entities
    {
        [DataMember(Name = "hashtags")]
        public HashtagEntity[] Hashtags { get; set; }

        [DataMember(Name = "symbols")]
        public HashtagEntity[] Symbols { get; set; }

        [DataMember(Name = "urls")]
        public UrlEntity[] Urls { get; set; }

        [DataMember(Name = "user_mentions")]
        public UserMentionEntity[] UserMentions { get; set; }

        [DataMember(Name = "media")]
        public MediaEntity[] Media { get; set; }
    }
}
