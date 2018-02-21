using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class TwitterErrorContainer
    {
        [DataMember(Name = "errors")]
        public TwitterError[] Errors { get; private set; }
    }
}
