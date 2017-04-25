using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal static partial class Defines
	{
		// public const string @ConsumerKey = "";
		// public const string @ConsumerSecret = "";

		// public const string @Official_ConsumerKey = "";
		// public const string @Official_ConsumerSecret = "";

		public const string @SettingFile = "settings.json";
		public const string @WindowFile = "winconf.json";
		public const string @AccountsFile = "accounts.json";

		public const string @DefaultNowPlayingPlayer = "wmplayer";
		public const string @DefaultNowPlayingFormat = @"%artist% - %name% / %album% #NowPlaying";

		private const string _defSoundPath = @"%windir%\Media\Windows Notify.wav";
		public static readonly string @DefaultSoundFile = Environment.ExpandEnvironmentVariables(_defSoundPath);

		public static readonly string[] DefaultTimelineFont = { "Meiryo", "Segoe UI Symbol" };
		public const double DefaultTimelineFontSize = 12;

		public readonly static string[] ImageExtensions =
		{
			// JPEG
			".jpeg",
			".jpg",
			".jpe",
			".jfif",
			".jfi",
			".jif",

			// PNG
			".png",

			// GIF
			".gif",

			// WebP
			".webp",
		};

		public readonly static string[] VideoExtensions =
		{
			".gif",
			".mp4",
		};

		public readonly static string[] UploadableMediaExtensions
			= ImageExtensions.Union(VideoExtensions).ToArray();
	}
}
