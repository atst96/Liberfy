using System;
using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class List
    {
        [DataMember(Name = "created_at")]
        [Utf8Json.JsonFormatter(typeof(TwitterTimeFormatFormatter))]
        public DateTimeOffset CreatedAt { get; set; }

        [DataMember(Name = "slug")]
        public string Slug { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "mode")]
        public string Mode { get; set; }

        [DataMember(Name = "following")]
        public bool Following { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "member_count")]
        public int MemberCount { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "subscriber_count")]
        public int SubscriberCount { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }
    }
}
