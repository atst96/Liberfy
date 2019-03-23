using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Mention
    {
        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "username")]
        public string UserName { get; private set; }

        [DataMember(Name = "acct")]
        public string Acct { get; private set; }

        [DataMember(Name = "id")]
        [Utf8Json.JsonFormatter(typeof(Formatters.StringToLongFormatter))]
        public long Id { get; private set; }
    }
}
