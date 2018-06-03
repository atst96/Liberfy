using SocialApis.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    [DataContract]
    public class Account : ICommonAccount
    {
        [IgnoreDataMember]
        public SocialService Service { get; } = SocialService.Mastodon;

        [DataMember(Name = "id")]
        [Utf8Json.JsonFormatter(typeof(Formatters.ToLongFormatter))]
        public long Id { get; private set; }
        [IgnoreDataMember]
        long? ICommonAccount.Id => this.Id;

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

        [IgnoreDataMember]
        string ICommonAccount.LongUserName => this.Acct;

        [IgnoreDataMember]
        bool ICommonAccount.IsProtected => this.IsLocked;

        [IgnoreDataMember]
        string ICommonAccount.AvatarImageUrl => this.Avatar;

        [IgnoreDataMember]
        string ICommonAccount.HeaderImageUrl => this.Header;

        [IgnoreDataMember]
        EntityBase[] ICommonAccount.UrlEntities { get; }

        [IgnoreDataMember]
        EntityBase[] ICommonAccount.DescriptionEntities { get; }

        [IgnoreDataMember]
        string ICommonAccount.Url { get; }

        [IgnoreDataMember]
        string ICommonAccount.RemoteUrl => this.Url;

        [IgnoreDataMember]
        bool? ICommonAccount.IsSuspended { get; }

        [IgnoreDataMember]
        string ICommonAccount.Language { get; }

        [IgnoreDataMember]
        string ICommonAccount.Location { get; }

        [IgnoreDataMember]
        string ICommonAccount.Description { get; }
    }
}
