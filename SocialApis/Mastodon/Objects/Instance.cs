using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Instance
    {
        [DataMember(Name = "uri")]
        public string Uri { get; private set; }

        [DataMember(Name = "title")]
        public string Title { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "email")]
        public string Email { get; private set; }

        [DataMember(Name = "urls")]
        public string Urls { get; private set; }
    }
}
