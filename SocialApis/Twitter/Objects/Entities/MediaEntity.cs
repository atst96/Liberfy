using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class MediaEntity : UrlEntity
    {
        [DataMember(Name = "type")]
        private string _type;
        public string Type => _type;

        [DataMember(Name = "sizes")]
        private MediaSizes _sizes;
        public MediaSizes Sizes => _sizes;

        [DataMember(Name = "media_url")]
        private string _mediaUrl;
        public string MediaUrl => _mediaUrl;

        [DataMember(Name = "id")]
        private long _id;
        public long Id => _id;

        [DataMember(Name = "media_url_https")]
        private string _mediaUrlHttps;
        public string MediaUrlHttps => _mediaUrlHttps;
    }
}
