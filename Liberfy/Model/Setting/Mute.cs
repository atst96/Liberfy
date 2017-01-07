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
	sealed class Mute : ICloneable, IDisposable
	{
		public Mute() { }


		private Regex customRegex;

		private int h_type;
		private int h_regx;
		private int h_text;

		private MuteType _type;
		private bool _useRegex;
		private string _text;

		[JsonProperty("type")]
		public MuteType Type
		{
			get { return _type; }
			set
			{
				_type = value;
				h_type = _type.GetHashCode();
			}
		}

		[JsonProperty("use_regex")]
		public bool UseRegex
		{
			get { return _useRegex; }
			set
			{
				_useRegex = value;
				h_regx = _useRegex.GetHashCode();
			}
		}

		[JsonProperty("text")]
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				h_text = _text?.ToLower().GetHashCode() ?? 0;
			}
		}

		public void Apply()
		{
			if (UseRegex)
			{
				customRegex = new Regex(Text, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			}
			else
			{
				customRegex = null;
			}
		}

		public bool IsMatchText(string text)
		{
			return UseRegex
				? text.IndexOf(Text, StringComparison.InvariantCultureIgnoreCase) >= 0
				: customRegex.IsMatch(text);
		}

		public object Clone() => new Mute
		{
			Type = Type,
			Text = Text,
			UseRegex = UseRegex,
		};

		public void Dispose()
		{
			customRegex = null;
		}

		public override int GetHashCode()
		{
			return h_type | h_regx | h_text;
		}
	}

	enum MuteType
	{
		TextContent,
		User,
		Source,
	}
}
