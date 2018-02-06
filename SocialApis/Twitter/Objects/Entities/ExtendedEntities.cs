using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class ExtendedEntities
    {
        [DataMember(Name = "media")]
        private MediaEntity[] _media;
        public MediaEntity[] Media => _media;
    }
}
