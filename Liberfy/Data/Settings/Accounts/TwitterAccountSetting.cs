#nullable enable
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

using MessagePack;
using SocialApis.Twitter;

namespace Liberfy.Settings
{
    [MessagePackObject]
    internal class TwitterAccountSetting : IAccountSetting
    {
        [Key("item.id")]
        public string ItemId { get; init; }

        [Key("id")]
        public long UserId { get; init; }

        [Key("user.screen_name")]
        public string ScreenName { get; set; }

        [Key("user.name")]
        public string Name { get; set; }

        [Key("user.is_protected")]
        public bool IsProtected { get; set; }

        [Key("user.profile_image_url")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// ConsumerKey
        /// </summary>
        [Key("key.consumer_key")]
        public string ConsumerKey { get; set; }

        /// <summary>
        /// ConsumerSecret
        /// </summary>
        [Key("key.consumer_secret")]
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        [Key("key.access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// AccessTokenSecret
        /// </summary>
        [Key("key.access_token_secret")]
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
