using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class CollectionResponseInfo
    {
        [DataMember(Name = "timeline_id")]
        public string TimelineId { get; private set; }
    }
}
