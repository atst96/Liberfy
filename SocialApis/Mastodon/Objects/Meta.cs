using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public struct Meta
    {
        [DataMember(Name = "width")]
        public int Width { get; private set; }

        [DataMember(Name = "height")]
        public int Height { get; private set; }

        [DataMember(Name = "size")]
        public object Size { get; private set; }

        [DataMember(Name = "aspect")]
        public object Aspect { get; private set; }
    }
}
