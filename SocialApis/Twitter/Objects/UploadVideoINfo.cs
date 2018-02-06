using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UploadVideoInfo
    {
        [DataMember(Name = "video_type")]
        private string _videoType;
        public string VideoType => _videoType;
    }
}
