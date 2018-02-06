using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class TwitterError
    {
        [DataMember(Name = "code")]
        private int _code;
        public int Code => _code;

        [DataMember(Name = "message")]
        private string _message;
        public string Message => _message;
    }
}
