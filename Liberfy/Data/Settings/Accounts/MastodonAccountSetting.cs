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
        [Key(0)]
        public string ItemId { get; init; }

        [Key(1)]
        public long Id { get; init; }

        [Key(2)]
        public Uri InstanceUrl { get; init; }

        [Key(3)]
        public string UserName { get; set; }

        [Key(4)]
        public string DisplayName { get; set; }

        [Key(5)]
        public string Avatar { get; set; }

        [Key(6)]
        public bool IsLocked { get; set; }

        [Key(7)]
        public string ClientId { get; set; }

        [Key(8)]
        public string ClientSecret { get; set; }

        [Key(9)]
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
