using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class MediaEntity : UrlEntity
    {
        [DataMember(Name = "type")]
        public string Type { get; private set; }

        [DataMember(Name = "sizes")]
        public MediaSizes Sizes { get; private set; }

        [DataMember(Name = "media_url")]
        public string MediaUrl { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "media_url_https")]
        public string MediaUrlHttps { get; private set; }
    }
}
