using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UploadMediaInfo
    {
        [DataMember(Name = "image_type")]
        private string _imageType;
        public string ImageType => _imageType;

        [DataMember(Name = "w")]
        private int _w;
        public int W => _w;

        [DataMember(Name = "h")]
        private int _h;
        public int H => _h;

        [DataMember(Name = "processing_info")]
        private ProcessingInfo _processingInfo;
        public ProcessingInfo ProcessingInfo => _processingInfo;
    }
}
