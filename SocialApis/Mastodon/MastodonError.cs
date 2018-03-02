using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class MastodonError
    {
        [DataMember(Name = "error")]
        public string Error { get; private set; }

        [DataMember(Name = "error_description")]
        public string ErrorDescription { get; private set; }
    }
}
