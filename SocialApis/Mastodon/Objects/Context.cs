using System.Runtime.Serialization;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Context
    {
        [DataMember(Name = "ancestors")]
        public Status[] Ancestors { get; private set; }

        [DataMember(Name = "descendants")]
        public Status[] Descendants { get; private set; }
    }
}
