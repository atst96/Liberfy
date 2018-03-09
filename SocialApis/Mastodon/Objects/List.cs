using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class List
    {
        [DataMember(Name = "id")]
        public string Id { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }
    }
}
