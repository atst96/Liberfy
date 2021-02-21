#nullable enable
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

using System;
using MessagePack;
using SocialApis.Mastodon;

namespace Liberfy.Settings
{
    [MessagePackObject]
    internal class MastodonAccountSetting : IAccountSetting
    {
        [Key("item.id")]
        public string ItemId { get; init; }

        [Key("user.id")]
        public long Id { get; init; }

        [Key("instance_url")]
        public Uri InstanceUrl { get; init; }

        [Key("user.name")]
        public string UserName { get; set; }

        [Key("user.display_name")]
        public string DisplayName { get; set; }

        [Key("user.avatar_url")]
        public string Avatar { get; set; }

        [Key("user.is_locked")]
        public bool IsLocked { get; set; }

        [Key("user.client_id")]
        public string ClientId { get; set; }

        [Key("key.client_secret")]
        public string ClientSecret { get; set; }

        [Key("key.access_token")]
        public string AccessToken { get; set; }

        public MastodonAccountSetting Clone() => new()
        {
            ItemId = this.ItemId,
            Id = this.Id,
            InstanceUrl = this.InstanceUrl,
            UserName = this.UserName,
            DisplayName = this.DisplayName,
            Avatar = this.Avatar,
            IsLocked = this.IsLocked,
            ClientId = this.ClientId,
            ClientSecret = this.ClientSecret,
            AccessToken = this.AccessToken,
        };

        public MastodonApi CreateApi()
        {
            return new MastodonApi(this.InstanceUrl, this.ClientId, this.ClientSecret, this.AccessToken);
        }
    }
}

#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
