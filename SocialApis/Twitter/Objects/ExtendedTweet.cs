using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class ExtendedTweet
    {
        [DataMember(Name = "full_text")]
        public string FullText { get; set; }

        [DataMember(Name = "display_text_range")]
        public int[] DisplayTextRange { get; set; }

        [DataMember(Name = "entities")]
        public Entities Entities { get; set; }
    }
}
