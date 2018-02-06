using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UserMentionEntity : EntityBase
    {
        [DataMember(Name = "name")]
        private string _name;
        public string Name => _name;

        [DataMember(Name = "screen_name")]
        private string _screenName;
        public string ScreenName => _screenName;

        [DataMember(Name = "id")]
        private long _id;
        public long Id => _id;
    }
}
