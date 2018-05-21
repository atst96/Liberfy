using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UploadVideoInfo
    {
        [DataMember(Name = "video_type")]
        public string VideoType { get; set; }
    }
}
