using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSize
    {
        [DataMember(Name = "h")]
        public long Height { get; set; }

        [DataMember(Name = "w")]
        public long Width { get; set; }

        [DataMember(Name = "resize")]
        public string Resize { get; set; }
    }
}
