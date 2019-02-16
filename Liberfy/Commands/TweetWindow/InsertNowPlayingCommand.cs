using Liberfy.ViewModel;
using NowPlayingLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Liberfy.Commands
{
    internal class InsertNowPlayingCommand : Command<string>
    {
        public TweetWindow _viewModel;

        private static Setting Setting { get; } = App.Setting;
        private static string[] PlayerIds = { "wmplayer", "itunes", "foobar2000" };

        public InsertNowPlayingCommand(TweetWindow viewModel)
        {
            this._viewModel = viewModel;
        }

        protected override bool CanExecute(string parameter)
        {
            return PlayerIds.Contains(parameter);
        }

        protected override async void Execute(string parameter)
        {
            if (!IsPlayerRunning(parameter))
            {
                this._viewModel.DialogService.MessageBox(
                    $"再生情報の取得に失敗しました。プレーヤが起動しているか確認してください。",
                    MsgBoxButtons.Ok, MsgBoxIcon.Error);

                return;
            }

            try
            {
                using (var player = await GetPlayerFromId(parameter))
                {
                    var media = await player.GetCurrentMedia();

                    this._viewModel.TextBoxController.Insert(ReplaceMediaAlias(media, Setting.NowPlayingFormat));

                    if (Setting.InsertThumbnailAtNowPlayying)
                    {
                        try
                        {
                            foreach (var stream in media.Artworks)
                            {
                                this._viewModel.PostParameters.Attachments.Add(UploadMedia.FromArtwork(new ArtworkItem(stream, Setting.InsertThumbnailAtNowPlayying)));
                            }
                        }
                        catch { /* pass */ }
                    }
                }
            }
            catch (Exception ex)
            {
                this._viewModel.DialogService.MessageBox(
                    $"再生情報の取得に失敗しました。プレーヤで楽曲が再生中かどうか確認してください。\n\nエラー：\n{ex.Message}",
                    MsgBoxButtons.Ok, MsgBoxIcon.Error);
            }
        }

        private static bool IsPlayerRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private static async ValueTask<MediaPlayerBase> GetPlayerFromId(string id)
        {
            switch (id)
            {
                case "wmplayer":
                    return await Task.Run(() => new WindowsMediaPlayer()).ConfigureAwait(false);

                case "itunes":
                    return await Task.Run(() => new iTunes()).ConfigureAwait(false);

                case "foobar2000":
                    return new NowPlayingLib.Foobar2000();

                default:
                    throw new NotImplementedException();
            }
        }

        private static string ReplaceMediaAlias(MediaItem media, string format)
        {
            var aliases = new Dictionary<string, string>
            {
                ["%album%"] = media.Album,
                ["%album_artist%"] = media.AlbumArtist,
                ["%artist%"] = media.Artist,
                ["%composer%"] = media.Composer,
                ["%category%"] = media.Category,
                ["%genre%"] = media.Genre,
                ["%name%"] = media.Name,
                ["%number%"] = media.TrackNumber.ToString(),
                ["%year%"] = media.Year.ToString()
            };

            string text = format;

            foreach (var alias in aliases)
            {
                text = text.Replace(alias.Key, alias.Value, StringComparison.OrdinalIgnoreCase);
            }

            return text;
        }
    }
}
