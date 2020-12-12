using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal static class Defaults
    {
        #region Setting files
        public const string @SettingFile = "settings.json";
        public const string @WindowFile = "winconf.json";
        public const string @AccountsFile = "accounts.bin";
        #endregion

        #region
        public const string ImageCacheFile = "cache.bin";
        #endregion

        #region Default settings
        public const string @DefaultNowPlayingPlayer = "wmplayer";
        public const string @DefaultNowPlayingFormat = @"%artist% - %name% / %album% #NowPlaying";

        private const string _defSoundPath = @"%windir%\Media\Windows Notify.wav";
        public static readonly string @DefaultSoundFile = Environment.ExpandEnvironmentVariables(_defSoundPath);

        public static readonly string DefaultTimelineFont = "Meiryo";
        public const double DefaultTimelineFontSize = 12;
        #endregion

        public const double MinimumProfileImageWidth = 20;
        public const double MaximumProfileImageWidth = 60;
        public const double DefaultProfileImageWidth = 40;

        public const double MinimumColumnWidth = 200;
        public const double MaximumColumnWidth = 600;
        public const double DefaultColumnWidth = 300;

        public readonly static string[] ImageExtensions =
        {
            // JPEG
            ".jpeg", ".jpg", ".jpe", ".jfif", ".jfi", ".jif",

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
            = ImageExtensions.Concat(VideoExtensions).ToArray();
    }
}
