using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct MediaSize
    {
        [DataMember(Name = "h")]
        private long _h;
        public long Height => _h;

        [DataMember(Name = "w")]
        private long _w;
        public long Width => _w;

        [DataMember(Name = "resize")]
        private string _resize;
        public string Resize => _resize;
    }
}
