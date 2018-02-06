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
        private long _mediaId;
        public long MediaId => _mediaId;

        [DataMember(Name = "size")]
        private int _size;
        public int Size => _size;

        [DataMember(Name = "expires_after_secs")]
        private int _expiresAfterSecs;
        public int ExpiresAfterSecs => _expiresAfterSecs;

        [DataMember(Name = "image")]
        private UploadMediaInfo _image;
        public UploadMediaInfo Image => _image;

        [DataMember(Name = "video")]
        private UploadVideoInfo _video;
        public UploadVideoInfo Video => _video;
    }
}
