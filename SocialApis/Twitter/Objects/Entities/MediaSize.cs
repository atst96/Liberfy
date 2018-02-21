using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSize
    {
        [DataMember(Name = "h")]
        public long Height { get; private set; }

        [DataMember(Name = "w")]
        public long Width { get; private set; }

        [DataMember(Name = "resize")]
        public string Resize { get; private set; }
    }
}
