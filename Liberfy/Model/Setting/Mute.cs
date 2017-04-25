using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liberfy
{
	[JsonObject]
	sealed class Mute
	{
		[JsonConstructor]
		private Mute() { }

		public Mute(MuteType type, SearchMode search, string text)
		{
			Type = type;
			Search = search;
			Text = text;
		}

		private static readonly StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
		private const RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled;

		private Regex r;

		[JsonIgnore]
		public bool IsInvalidItem { get; private set; } = true;

		[JsonProperty("type")]
		public MuteType Type { get; private set; }

		[JsonProperty("search")]
		public SearchMode Search { get; private set; }

		[JsonProperty("text")]
		public string Text { get; private set; }

		public void Apply()
		{
			if(IsAvailable(Type, Search, Text))
			{
				IsInvalidItem = false;

				if (Search == SearchMode.Regex)
				{
					r = new Regex(Text, regexOptions);
				}
			}
		}

		private bool MatchText(string baseText)
		{
			if (IsInvalidItem)
				return false;

			switch(Search)
			{
				case SearchMode.Partial:
					return baseText.IndexOf(Text, comparison) > 0;

				case SearchMode.Forward:
					return baseText.StartsWith(Text, comparison);

				case SearchMode.Backward:
					return baseText.EndsWith(Text, comparison);

				case SearchMode.Perfect:
					return baseText.Equals(Text, comparison);

				case SearchMode.Regex:
					return r.IsMatch(baseText);
			}

			return false;
		}

		public static bool IsAvailable(MuteType type, SearchMode search, string text)
		{
			return Enum.IsDefined(typeof(MuteType), type)
				&& Enum.IsDefined(typeof(SearchMode), search)
				&& !string.IsNullOrWhiteSpace(text)
				&& (search != SearchMode.Regex || IsEnableRegex(text));
		}

		public static bool IsAvailable(Mute mute)
		{
			return IsAvailable(mute.Type, mute.Search, mute.Text);
		}

		private static bool IsEnableRegex(string text)
		{
			Regex regex;
			try
			{
				regex = new Regex(text, RegexOptions.IgnoreCase);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				regex = null;
			}
		}
	}

	enum MuteType : uint
	{
		Content = 0,
		UserId = 1,
		ScreenName = 2,
		ViewName = 3,
		Client = 4,
	}

	enum SearchMode : uint
	{
		Partial = 0,
		Forward = 1,
		Backward = 2,
		Perfect = 3,
		Regex = 4,
	}
}
