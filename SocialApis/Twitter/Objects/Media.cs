using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Media
    {
        [DataMember(Name = "media_id")]
        public long MediaId { get; private set; }

        [DataMember(Name = "size")]
        public int Size { get; private set; }

        [DataMember(Name = "expires_after_secs")]
        public int ExpiresAfterSecs { get; private set; }

        [DataMember(Name = "image")]
        public UploadMediaInfo Image { get; private set; }

        [DataMember(Name = "video")]
        public UploadVideoInfo Video { get; private set; }
    }
}
