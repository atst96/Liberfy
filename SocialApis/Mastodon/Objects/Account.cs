using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Account
    {
        [DataMember(Name = "id")]
        [Utf8Json.JsonFormatter(typeof(Formatters.ToLongFormatter))]
        public long Id { get; private set; }

        [DataMember(Name = "username")]
        public string UserName { get; private set; }

        [DataMember(Name = "acct")]
        public string Acct { get; private set; }

        [DataMember(Name = "display_name")]
        public string DisplayName { get; private set; }

        [DataMember(Name = "locked")]
        public bool IsLocked { get; private set; }

        [DataMember(Name = "created_at")]
        public DateTimeOffset CreatedAt { get; private set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; private set; }

        [DataMember(Name = "following_count")]
        public int FollowingCount { get; private set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; private set; }

        [DataMember(Name = "note")]
        public string Note { get; private set; }

        [DataMember(Name = "url")]
        public string Url { get; private set; }

        [DataMember(Name = "avatar")]
        public string Avatar { get; private set; }

        [DataMember(Name = "avatar_static")]
        public string AvatarStatic { get; private set; }

        [DataMember(Name = "header")]
        public string Header { get; private set; }

        [DataMember(Name = "header_static")]
        public string HeaderStatic { get; private set; }

        [DataMember(Name = "moved")]
        public bool? Moved { get; private set; }
    }
}
