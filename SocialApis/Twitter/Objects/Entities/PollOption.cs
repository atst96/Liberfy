using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public struct PollOption
    {
        [DataMember(Name = "position")]
        public int Position { get; private set; }

        [DataMember(Name = "text")]
        public string Text { get; private set; }
    }
}
