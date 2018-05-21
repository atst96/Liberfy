using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Liberfy
{
	[DataContract]
	internal sealed class Mute
	{
		private const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;
		private const RegexOptions regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled;

		[Utf8Json.SerializationConstructor]
		private Mute()
		{
		}

		public Mute(MuteType type, SearchMode search, string text)
		{
			Type = type;
			Search = search;
			Text = text;
		}

		public static bool Create(MuteType type, SearchMode search, string text, out Mute mute)
		{
			return (mute = new Mute(type, search, text)).Apply();
		}

		private Regex r;

		public bool IsValidItem { get; set; }

		[DataMember(Name = "type")]
		public MuteType Type { get; private set; }

		[DataMember(Name = "search")]
		public SearchMode Search { get; private set; }

		[DataMember(Name = "text")]
		public string Text { get; private set; }

		[DataMember(Name = "enabled")]
		public bool IsEnabled { get; set; } = true;

		public bool Apply()
		{
			if (IsAvailable(Type, Search, Text))
			{
				if (Search == SearchMode.Regex)
				{
					r = new Regex(Text, regexOptions);
				}

				IsValidItem = true;

				return true;
			}
			return false;
		}

		private bool MatchText(string baseText)
		{
			if (!IsValidItem)
				return false;

			switch (Search)
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

		private static bool IsEnableRegex(string text)
		{
			Regex regex;

			try
			{
				regex = new Regex(text, regexOptions ^ RegexOptions.Compiled);
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

	internal enum MuteType : uint
	{
		Content = 0,
		UserId = 1,
		ScreenName = 2,
		ViewName = 3,
		Client = 4,
	}

	internal enum SearchMode : uint
	{
		Partial = 0,
		Forward = 1,
		Backward = 2,
		Perfect = 3,
		Regex = 4,
	}
}
