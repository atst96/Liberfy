using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class CursoredIdsResponse : IRateLimit
    {
        [DataMember(Name = "ids")]
        public long[] Ids { get; set; }

        [DataMember(Name = "next_cursor")]
        public int? NextCursor { get; set; }

        [DataMember(Name = "previous_cursor")]
        public int? PreviousCursor { get; set; }

        public RateLimit RateLimit { get; set; }
    }
}
