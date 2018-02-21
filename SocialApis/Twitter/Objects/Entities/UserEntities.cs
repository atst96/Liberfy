using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct UserEntities
    {
        [DataMember(Name = "url")]
        public Entities Url { get; private set; }

        [DataMember(Name = "description")]
        public Entities Description { get; private set; }
    }
}
