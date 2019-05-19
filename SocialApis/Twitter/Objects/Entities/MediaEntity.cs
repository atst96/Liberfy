using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class MediaEntity : UrlEntity
    {
        /// <summary>
        /// SocialApis.Twitter.MediaTypes.*
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "sizes")]
        public MediaSizes Sizes { get; set; }

        [DataMember(Name = "media_url")]
        public string MediaUrl { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "media_url_https")]
        public string MediaUrlHttps { get; set; }

        [DataMember(Name = "video_info")]
        public MediaVideoInfo? VideoInfo { get; set; }
    }
}
