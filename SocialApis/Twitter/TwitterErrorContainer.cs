using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class TwitterErrorContainer
    {
        [DataMember(Name = "errors")]
        private TwitterError[] _errors;
        public TwitterError[] Errors => _errors;
    }
}
