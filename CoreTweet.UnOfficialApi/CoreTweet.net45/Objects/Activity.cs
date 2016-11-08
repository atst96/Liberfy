using System;
using System.Collections.Generic;
using CoreTweet.Core;
using Newtonsoft.Json;
using CoreTweet.Streaming;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace CoreTweet
{
	public class Activity : CoreBase
	{
		[JsonProperty("action")]
		public ActionCode Action { get; set; }

		[JsonProperty("max_position")]
		public long MaxPosition { get; set; }

		[JsonProperty("min_position")]
		public long MinPosition { get; set; }

		[JsonProperty("created_at")]
		[JsonConverter(typeof(DateTimeOffsetConverter))]
		public DateTimeOffset CreatedAt { get; set; }

		[JsonProperty("target_objects")]
		public JArray TargetObjects { get; set; }

		[JsonProperty("target_objects_size")]
		public int TargetObjectsSize { get; set; }

		[JsonProperty("targets")]
		public JArray Targets { get; set; }

		[JsonProperty("target_size")]
		public int TargetSize { get; set; }

		[JsonProperty("sources")]
		public JArray Sources { get; set; }

		[JsonProperty("sources_size")]
		public int SourcesSize { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum ActionCode
	{
		[JsonProperty("reply")]
		Reply,

		[JsonProperty("quote")]
		Quote,

		[JsonProperty("mention")]
		Mention,

		[JsonProperty("follow")]
		Follow,

		[JsonProperty("favorite")]
		Favorite,

		[JsonProperty("retweet")]
		Retweet,

		[JsonProperty("favorited_retweet")]
		FavoritedRetweet,

		[JsonProperty("retweeted_retweet")]
		RetweetedRetweet,

		[JsonProperty("list_member_added")]
		ListMemberAdded,

		[JsonProperty("favorited_mention")]
		FavoritedMention,

		[JsonProperty("retweeted_mention")]
		RetweetedMention,
	}
}
