using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UrlUnwound
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
