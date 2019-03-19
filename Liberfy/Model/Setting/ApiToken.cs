using MessagePack;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Mastodon = SocialApis.Mastodon;

namespace Liberfy.Settings
{
    /// <summary>
    /// APIの利用に必要な認証情報を格納するクラス
    /// </summary>
    [DataContract]
    internal struct ApiTokenInfo
    {
        [Key("host")]
        [DataMember(Name = "host")]
        public Uri HostUrl { get; set; }

        [Key("consumer_key")]
        [DataMember(Name = "consumer_key")]
        public string ConsumerKey { get; set; }

        [Key("consumer_secret")]
        [DataMember(Name = "consumer_secret")]
        public string ConsumerSecret { get; set; }

        [Key("access_token")]
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [Key("access_token_secret")]
        [DataMember(Name = "access_token_secret")]
        public string AccessTokenSecret { get; set; }

        public static ApiTokenInfo FromTokens(IApi tokens)
        {
            if (tokens is Mastodon.MastodonApi mTokens)
            {
                return ApiTokenInfo.FromTokens(mTokens);
            }
            else
            {
                return new ApiTokenInfo
                {
                    ConsumerKey = tokens.ConsumerKey,
                    ConsumerSecret = tokens.ConsumerSecret,
                    AccessToken = tokens.ApiToken,
                    AccessTokenSecret = tokens.ApiTokenSecret
                };
            }
        }

        public static ApiTokenInfo FromTokens(Mastodon.MastodonApi tokens)
            => new ApiTokenInfo
            {
                HostUrl = tokens.HostUrl,
                ConsumerKey = tokens.ClientId,
                ConsumerSecret = tokens.ClientSecret,
                AccessToken = tokens.AccessToken,
            };
    }
}
