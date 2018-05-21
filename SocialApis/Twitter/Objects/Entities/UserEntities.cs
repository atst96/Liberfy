using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct UserEntities
    {
        [DataMember(Name = "url")]
        public Entities Url { get; set; }

        [DataMember(Name = "description")]
        public Entities Description { get; set; }
    }
}
