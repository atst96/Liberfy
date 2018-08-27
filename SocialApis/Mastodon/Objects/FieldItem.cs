using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class FieldItem
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "value")]
        public string Value { get; private set; }
    }
}
