using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class TwitterError
    {
        [DataMember(Name = "code")]
        public int Code { get; private set; }

        [DataMember(Name = "message")]
        public string Message { get; private set; }
    }
}
