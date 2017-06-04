using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	/// <summary>
	/// アカウント設定をJsonデータに変換するためのクラス
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	internal class AccountItem
	{
		[JsonProperty("user_id")]
		public long Id { get; set; }

		[JsonProperty("screen_name")]
		public string ScreenName { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("profile_image_url")]
		public string ProfileImageUrl { get; set; }

		[JsonProperty("is_protected")]
		public bool IsProtected { get; set; }

		[JsonProperty("token")]
		public ApiTokenInfo Token { get; set; }

		[JsonProperty("tokens_third", NullValueHandling = NullValueHandling.Ignore)]
		public ApiTokenInfo[] ThirdPartyTokens { get; set; }

		[JsonProperty("automatically_login")]
		public bool AutomaticallyLogin { get; set; }

		[JsonProperty("automatically_load_timeline")]
		public bool AutomaticallyLoadTimeline { get; set; }
	}
}
