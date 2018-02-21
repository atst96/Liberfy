using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class ExtendedEntities
    {
        [DataMember(Name = "media")]
        public MediaEntity[] Media { get; private set; }
    }
}
