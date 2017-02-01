using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	partial class Defines
	{
		// public const string @ConsumerKey = "";
		// public const string @ConsumerSecret = "";

		// public const string @Official_ConsumerKey = "";
		// public const string @Official_ConsumerSecret = "";

		public const string @SettingFile = "settings.json";
		public const string @WindowFile = "winconf.json";
		public const string @AccountsFile = "accounts.json";

		public const string @DefaultNowPlayingFormat = @"%artist% - %name% / %album% #NowPlaying";

		private const string _defSoundPath = @"%windir%\Media\Windows Notify.wav";
		public static readonly string @DefaultSoundFile = Environment.ExpandEnvironmentVariables(_defSoundPath);
	}
}
