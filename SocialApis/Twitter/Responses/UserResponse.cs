using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UserResponse : User, IRateLimit
    {
        public RateLimit RateLimit { get; }
    }
}
