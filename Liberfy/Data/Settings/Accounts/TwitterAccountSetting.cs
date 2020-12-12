#nullable enable
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

using System;
using System.Windows.Navigation;
using MessagePack;
using SocialApis.Twitter;

namespace Liberfy.Settings
{
    [MessagePackObject]
    internal class TwitterAccountSetting : IAccountSetting
    {
        [Key(0)]
        public string ItemId { get; init; }

        [Key(1)]
        public long UserId { get; init; }

        [Key(2)]
        public string ScreenName { get; set; }

        [Key(3)]
        public string Name { get; set; }

        [Key(4)]
        public bool IsProtected { get; set; }

        [Key(5)]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// ConsumerKey
        /// </summary>
        [Key(6)]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// ConsumerSecret
        /// </summary>
        [Key(7)]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        [Key(8)]
        public string AccessToken { get; set; }

        /// <summary>
        /// AccessTokenSecret
        /// </summary>
        [Key(9)]
        public string AccessTokenSecret { get; set; }

        public TwitterAccountSetting Clone() => new()
        {
            ItemId = this.ItemId,
            UserId = this.UserId,
            ScreenName = this.ScreenName,
            Name = this.Name,
            IsProtected = this.IsProtected,
            ProfileImageUrl = this.ProfileImageUrl,
            ConsumerKey = this.ConsumerKey,
            ConsumerSecret = this.ConsumerSecret,
            AccessToken = this.AccessToken,
            AccessTokenSecret = this.AccessTokenSecret,
        };

        public TwitterApi CreateApi()
        {
            return new TwitterApi(this.ConsumerKey, this.ConsumerSecret, this.AccessToken, this.AccessTokenSecret);
        }
    }
}

#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
