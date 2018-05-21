using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class UploadMediaInfo
    {
        [DataMember(Name = "image_type")]
        public string ImageType { get; set; }

        [DataMember(Name = "w")]
        public int Width { get; set; }

        [DataMember(Name = "h")]
        public int Height { get; set; }

        [DataMember(Name = "processing_info")]
        public ProcessingInfo ProcessingInfo { get; set; }
    }
}
