using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSizes
    {
        [DataMember(Name = "thumb")]
        public MediaSize Thumb { get; private set; }

        [DataMember(Name = "large")]
        public MediaSize Large { get; private set; }

        [DataMember(Name = "medium")]
        public MediaSize Medium { get; private set; }

        [DataMember(Name = "small")]
        public MediaSize Small { get; private set; }
    }
}
