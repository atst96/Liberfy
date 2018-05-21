using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSizes
    {
        [DataMember(Name = "thumb")]
        public MediaSize Thumb { get; set; }

        [DataMember(Name = "large")]
        public MediaSize Large { get; set; }

        [DataMember(Name = "medium")]
        public MediaSize Medium { get; set; }

        [DataMember(Name = "small")]
        public MediaSize Small { get; set; }
    }
}
