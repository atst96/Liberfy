using CoreTweet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	/// <summary>
	/// APIの利用に必要な認証情報を格納するクラス
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	internal class ApiTokenInfo
	{
		[JsonProperty("consumer_key")]
		public string ConsumerKey { get; set; }

		[JsonProperty("consumer_secret")]
		public string ConsumerSecret { get; set; }

		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("access_token_secret")]
		public string AccessTokenSecret { get; set; }

		public Tokens ToCoreTweetTokens()
			=> new Tokens
			{
				ConsumerKey       = this.ConsumerKey,
				ConsumerSecret    = this.ConsumerSecret,
				AccessToken       = this.AccessToken,
				AccessTokenSecret = this.AccessTokenSecret
			};

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
