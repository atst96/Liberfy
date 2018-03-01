using System;
using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class List
    {
        [DataMember(Name = "created_at")]
        [Utf8Json.JsonFormatter(typeof(DateTimeOffsetFormatter))]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "slug")]
        public string Slug { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "full_name")]
        public string FullName { get; private set; }

        [DataMember(Name = "description")]
        public string Description { get; private set; }

        [DataMember(Name = "mode")]
        public string Mode { get; private set; }

        [DataMember(Name = "following")]
        public bool Following { get; private set; }

        [DataMember(Name = "user")]
        public User User { get; private set; }

        [DataMember(Name = "member_count")]
        public int MemberCount { get; private set; }

        [DataMember(Name = "id")]
        public long Id { get; private set; }

        [DataMember(Name = "subscriber_count")]
        public int SubscriberCount { get; private set; }

        [DataMember(Name = "uri")]
        public string Uri { get; private set; }
    }
}
