using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Cursor<T>
    {
        [DataMember(Name = "previous_cursor")]
        public int PreviousCursor { get; private set; }

        [DataMember(Name = "next_cursor")]
        public int NextCursor { get; private set; }

        [DataMember(Name = "ids")]
        public T[] Ids { get; private set; }
    }
}
