using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class HashtagEntity : EntityBase
    {
        [DataMember(Name = "text")]
        private string _text;
        public string Text => _text;
    }
}
