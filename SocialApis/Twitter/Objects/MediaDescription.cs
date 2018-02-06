using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class MediaDescription
    {
        [DataMember(Name = "media_id")]
        private long _mediaId;
        public long MediaId => _mediaId;

        [DataMember(Name = "all_text")]
        private MediaDescriptionText _allText;
        public MediaDescriptionText AllText => _allText;

        [DataContract]
        public class MediaDescriptionText
        {
            [DataMember(Name = "text")]
            private string _text;
            public string Text => _text;
        }
    }
}
