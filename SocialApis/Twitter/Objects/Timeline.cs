using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class Timeline
    {
        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "user_id")]
        public long UserId { get; private set; }

        [DataMember(Name = "collection_url")]
        public string CollectionUrl { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "visibility")]
        public string Visibility { get; private set; }

        [DataMember(Name = "timeline_order")]
        public string TimelineOrder { get; private set; }

        [DataMember(Name = "collection_type")]
        public string CollectionType { get; private set; }
    }
}
