using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UserMentionEntity : EntityBase
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; private set; }
    }
}
