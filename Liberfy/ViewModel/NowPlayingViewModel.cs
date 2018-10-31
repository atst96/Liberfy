using NowPlayingLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPLib = NowPlayingLib;

namespace Liberfy.ViewModel
{
    internal class NowPlayingViewModel : ContentWindowViewModel
    {
        private static Setting Setting => App.Setting;

        private string _player = Setting.NowPlayingDefaultPlayer;
        public string NowPlayingPlayer
        {
            get => this._player;
            set => this.SetProperty(ref this._player, value, this._getPlayingDataCommand);
        }

        private string _insertinText;
        public string InsertionText
        {
            get => this._insertinText;
            set => this.SetProperty(ref this._insertinText, value);
        }

        public NotifiableCollection<ArtworkItem> Artworks { get; } = new NotifiableCollection<ArtworkItem>();

        private Command _getPlayingDataCommand;
        public Command GetPlayingDataCommand => this._getPlayingDataCommand ?? (this._getPlayingDataCommand = this.RegisterCommand(DelegateCommand
            .When(() =>
            {
                return !string.IsNullOrEmpty(_player)
                    && TweetWindow.NowPlayingPlayerList.ContainsKey(_player);
            })
            .Exec(async () =>
            {
                var copiedArtworks = this.Artworks.ToArray();
                this.Artworks.Clear();
                copiedArtworks.DisposeAll();

                MediaPlayerBase player = null;

                if (!IsProcessRunning(this._player))
                {
                    this.DialogService.MessageBox(
                        $"再生情報の取得に失敗しました。プレーヤが起動しているか確認してください。",
                        MsgBoxButtons.Ok, MsgBoxIcon.Error);
                    return;
                }

                try
                {
                    switch (this._player)
                    {
                        case "wmplayer":
                            player = new NPLib.WindowsMediaPlayer(registerEvents: false);
                            break;

                        case "itunes":
                            player = new NPLib.iTunes(registerEvents: false);
                            break;

                        case "foobar2000":
                            player = new NPLib.Foobar2000(registerEvents: false);
                            break;

                        default:
                            return;
                    }

                    var media = await player.GetCurrentMedia();

                    this.InsertionText = ReplaceMediaAlias(media, Setting.NowPlayingFormat);
                    foreach (var stream in media.Artworks)
                    {
                        try
                        {
                            this.Artworks.Add(new ArtworkItem(stream, Setting.InsertThumbnailAtNowPlayying));
                        }
                        catch { /* 非対応形式のアートワークは処理しない */ }
                    }
                }
                catch (Exception ex)
                {
                    this.DialogService.MessageBox(
                        $"再生情報の取得に失敗しました。プレーヤで楽曲が再生中かどうか確認してください。\n\nエラー：\n{ex.Message}",
                        MsgBoxButtons.Ok, MsgBoxIcon.Error);
                }
                finally
                {
                    if (player != null)
                    {
                        player.Dispose();
                        player = null;
                    }
                }
            })));

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private string ReplaceMediaAlias(MediaItem media, string format)
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

            string replacedString = format;

            foreach (var arias in aliases)
            {
                replacedString = Regex.Replace(replacedString, arias.Key, arias.Value, RegexOptions.IgnoreCase);
            }

            return replacedString;
        }

        private Command _insertTextCommand;
        public Command InsertTextCommand => this._insertTextCommand ?? (this._insertTextCommand = this.RegisterCommand(() =>
        {
            this.DialogService.Close(true);
        }));
    }
}
