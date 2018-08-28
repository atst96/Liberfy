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
        [DataMember(Name = "host")]
        public string Host { get; set; }

        [DataMember(Name = "consumer_key")]
        public string ConsumerKey { get; set; }

        [DataMember(Name = "consumer_secret")]
        public string ConsumerSecret { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "access_token_secret")]
        public string AccessTokenSecret { get; set; }

        public static ApiTokenInfo FromTokens(ITokensBase tokens)
        {
            if (tokens is Mastodon.Tokens mTokens)
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

        public static ApiTokenInfo FromTokens(Mastodon.Tokens tokens)
            => new ApiTokenInfo
            {
                Host = tokens.HostUrl.AbsoluteUri.ToString(),
                ConsumerKey = tokens.ClientId,
                ConsumerSecret = tokens.ClientSecret,
                AccessToken = tokens.AccessToken,
            };
    }
}
