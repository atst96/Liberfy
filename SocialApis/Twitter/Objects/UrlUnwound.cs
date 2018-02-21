using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UrlUnwound
    {
        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "status")]
        public int Status { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }
    }
}
