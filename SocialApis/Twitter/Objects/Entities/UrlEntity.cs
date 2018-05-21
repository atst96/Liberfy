using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UrlEntity : EntityBase
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "display_url")]
        public string DisplayUrl { get; set; }

        [DataMember(Name = "expanded_url")]
        public string ExpandedUrl { get; set; }

        [DataMember(Name = "unwound")]
        public UrlUnwound Unwound { get; set; }
    }
}
