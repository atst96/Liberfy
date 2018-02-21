using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class HashtagEntity : EntityBase
    {
        [DataMember(Name = "text")]
        public string Text { get; private set; }
    }
}
