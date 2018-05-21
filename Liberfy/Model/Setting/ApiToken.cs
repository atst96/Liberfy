using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    /// <summary>
    /// APIの利用に必要な認証情報を格納するクラス
    /// </summary>
    [DataContract]
    internal class ApiTokenInfo
    {
        [DataMember(Name = "consumer_key")]
        public string ConsumerKey { get; set; }

        [DataMember(Name = "consumer_secret")]
        public string ConsumerSecret { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "access_token_secret")]
        public string AccessTokenSecret { get; set; }

        public Tokens ToCoreTweetTokens()
            => new Tokens(
                this.ConsumerKey,
                this.ConsumerSecret,
                this.AccessToken,
                this.AccessTokenSecret);

        public static ApiTokenInfo FromCoreTweetTokens(Tokens tokens)
            => new ApiTokenInfo
            {
                ConsumerKey       = tokens.ConsumerKey,
                ConsumerSecret    = tokens.ConsumerSecret,
                AccessToken       = tokens.AccessToken,
                AccessTokenSecret = tokens.AccessTokenSecret
            };
    }
}
