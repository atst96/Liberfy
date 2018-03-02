using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class AccessTokenResponse
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; private set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; private set; }

        [DataMember(Name = "scope")]
        public string Scope { get; private set; }

        [DataMember(Name = "created_at")]
        public int CreatedAt { get; private set; }
    }
}
