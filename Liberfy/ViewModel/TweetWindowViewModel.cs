using CoreTweet;
using NowPlayingLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Twitter.Text;
using static Liberfy.Defines;

namespace Liberfy.ViewModel
{
    internal class TweetWindow : ViewModelBase
    {
        private static readonly Validator tweetValidator = new Validator();
        protected static Setting Setting => App.Setting;

        public FluidCollection<Account> Accounts => App.Accounts;

        private Account _selectedAccount;
        public Account SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value, _postCommand);
        }

        public void SetPostAccount(Account account)
        {
            SelectedAccount = account;
        }

        private const int MaxTweetLength = 140;
        private const int MediaUrlLength = 23;

        public TweetWindow()
        {
            Media = new FluidCollection<UploadMedia>();

            SelectedAccount = Accounts.First();

            UpdateCanPost();
        }

        public FluidCollection<UploadMedia> Media { get; }

        private bool _canUpdateContent;
        public bool CanUpdateContent => _canUpdateContent;

        private int _remainingTweetLength = MaxTweetLength;
        public int RemainingTweetLength => _remainingTweetLength;

        private string _uploadStatusText;
        public string UploadStatusText
        {
            get => _uploadStatusText;
            set => SetProperty(ref _uploadStatusText, value);
        }

        private bool _isTweetPosting;
        public bool IsTweetPosting
        {
            get => _isTweetPosting;
            set => SetProperty(ref _isTweetPosting, value);
        }

        private string _tweet = string.Empty;
        public string Tweet
        {
            get => _tweet;
            set
            {
                var newTweet = value?.Replace("\r\n", "\n") ?? string.Empty;
                if (SetProperty(ref _tweet, newTweet))
                {
                    UpdateCanPost();
                }
            }
        }

        private void UpdateCanPost()
        {
            // ツイート可能な残り文字数の算出
            int actualTweetLength = tweetValidator.GetTweetLength(_tweet);
            if (Media.Count > 0)
                actualTweetLength += MediaUrlLength;

            _remainingTweetLength = MaxTweetLength - actualTweetLength;

            // アップロード可能かを判定
            bool isTweetValiableLength =
                _remainingTweetLength > 0 && _remainingTweetLength < MaxTweetLength;
            bool isPostableMediaCount = Media.Count <= 4;

            _canUpdateContent = isTweetValiableLength && isPostableMediaCount;

            // プロパティの変更通知
            RaisePropertyChanged(nameof(RemainingTweetLength));
            RaisePropertyChanged(nameof(CanUpdateContent));

            PostCommand.RaiseCanExecute();
        }

        private bool _isSensitiveMedia = Setting.NoticePostSensitiveMedia;
        public bool IsSensitiveMedia
        {
            get => _isSensitiveMedia;
            set => SetProperty(ref _isSensitiveMedia, value);
        }

        private bool _closeOnPostComplated = Setting.CloseWindowAfterPostComplated;
        public bool CloseOnPostComplated
        {
            get => _closeOnPostComplated;
            set => SetProperty(ref _closeOnPostComplated, value);
        }

        private StatusInfo _replyToStatus;
        public StatusInfo ReplyToStatus => _replyToStatus;
        public bool HasReplyStatus { get; private set; } = false;

        private UserInfo _replyUser;
        public UserInfo ReplyUser => _replyUser;
        public bool HasReplyUser { get; private set; } = false;

        public void SetReplyToUser(UserInfo user)
        {
            if (user == null)
            {
                HasReplyUser = false;
                _replyUser = null;
            }

            _replyUser = user;
            HasReplyUser = true;

            RaisePropertyChanged(nameof(ReplyUser));
            RaisePropertyChanged(nameof(HasReplyUser));
        }

        public void SetReplyToStatus(StatusInfo status)
        {
            _replyToStatus = status;
            HasReplyStatus = _replyToStatus != null;

            RaisePropertyChanged(nameof(HasReplyStatus));
            RaisePropertyChanged(nameof(HasReplyStatus));

            var mentionEntity = status?.Entities?.UserMentions;
            bool hasMentionEntity = Setting.IncludeOtherAtReply && mentionEntity != null;

            var mentions = new List<string>((hasMentionEntity ? mentionEntity.Length : 0) + 1)
            {
                status.User.ScreenName
            };
            if (hasMentionEntity)
                mentions.AddRange(mentionEntity.Select(m => m.ScreenName));

            Tweet = string.Concat(mentions
                .GroupBy(key => key, (key, elements) => $"@{elements.First()} ", _stringIgnroeCaseCompare));
        }

        private static StringIgnoreCaseEqualityComparer _stringIgnroeCaseCompare = new StringIgnoreCaseEqualityComparer();
        private class StringIgnoreCaseEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj) => GetHashCode();
        }

        #region Command: PostCommand

        private Command _postCommand;
        public Command PostCommand
        {
            get => _postCommand ?? (_postCommand = RegisterCommand(PostTweet, CanPostTweet));
        }

        private Tokens Tokens => SelectedAccount.Tokens;
        private int _postTweetFase = -1;

        public async void PostTweet()
        {
            OnPostBegin();

            var uploadPrams = new DictionaryEx<string, object>
            {
                ["status"] = Tweet,
                ["possibly_sensitive"] = IsSensitiveMedia,
            };

            if (_replyToStatus != null)
                uploadPrams["in_reply_to_status_id"] = _replyToStatus.Id;

            // 画像および動画のアップロード
            var uploadableMedia = GetUploadableMedia(Media);
            if (uploadableMedia.Any())
            {
                _postTweetFase = 1;

                UploadStatusText = "メディアをアップロードしています...";
                uploadableMedia.ForEach(m => m.SetIsTweetPosting(true));

                if (await UploadMediaItems(Tokens, uploadableMedia).ConfigureAwait(true))
                {
                    uploadPrams["media_ids"] = from m in Media where m.IsAvailableUploadId() select m.UploadId.Value;
                }
                else
                {
                    uploadableMedia.ForEach(m => m.SetIsTweetPosting(false));

                    OnPostEnd();
                    return;
                }
            }

            // ツイート
            _postTweetFase = 2;
            UploadStatusText = "ツイートしています...";

            try
            {
                await Tokens.Statuses.UpdateAsync(uploadPrams);

                OnPostComplated();
            }
            catch (Exception ex)
            {
                DialogService.MessageBox(ex.Message, null);
            }

            OnPostEnd();

            if (_closeOnPostComplated)
            {
                DialogService.Invoke(ViewState.Close);
            }
        }

        private void OnPostComplated()
        {
            ClearStatus();
        }

        private void ClearStatus()
        {
            HasReplyUser = false;
            _replyUser = null;
            HasReplyStatus = false;
            _replyToStatus = null;

            var media = Media.ToArray();
            Media.Clear();

            for (int i = 0; i < media.Length; i++)
                media[i].Dispose();

            media = null;

            Tweet = string.Empty;
        }

        private IEnumerable<UploadMedia> GetUploadableMedia(IEnumerable<UploadMedia> media)
        {
            return media.Where(m => !m.IsAvailableUploadId());
        }

        /// <summary>
        /// 指定されたメディア項目のアップロードを試行し、アップロードが完了したかを返します。
        /// </summary>
        /// <param name="token">認証用トークン</param>
        /// <param name="media">メディア項目の一覧</param>
        /// <returns>すべての項目のアップロードが完了したかどうか</returns>
        private async Task<bool> UploadMediaItems(Tokens token, IEnumerable<UploadMedia> media)
        {
            await Task.WhenAll(media.Select(m => m.Upload(token)).ToArray());

            var badResults = media.Where(m => m.IsUploadFailed);
            int badResCount = badResults.Count();

            if (badResCount > 0)
            {
                if (DialogService.MessageBox(
                    $"{media.Count()}件中{badResCount}件 アップロードに失敗しました。\n再試行しますか？",
                    MsgBoxButtons.RetryCancel, MsgBoxIcon.Question) == MsgBoxResult.Retry)
                {
                    return await UploadMediaItems(token, GetUploadableMedia(media));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanPostTweet()
        {
            return _selectedAccount != null && !_isTweetPosting && IsEditable && _canUpdateContent;
        }

        public bool IsEditable { get; private set; } = true;

        private void OnPostBegin()
        {
            IsTweetPosting = true;
            SetIsEditable(false);
            _postTweetFase = 0;
        }

        private void OnPostEnd()
        {
            IsTweetPosting = false;
            SetIsEditable(true);
            _postTweetFase = -1;
        }

        private void SetIsEditable(bool canEdit)
        {
            IsEditable = canEdit;
            RaisePropertyChanged(nameof(IsEditable));

            AddImageCommand.RaiseCanExecute();
            PostCommand.RaiseCanExecute();
        }

        #endregion

        #region Command: SelectAccountCommand

        private Command<Account> _selectAccountCommand;
        public Command<Account> SelectAccountCommand
        {
            get => _selectAccountCommand ?? (_selectAccountCommand = RegisterCommand<Account>(SelectAccount, IsAvailableAccount));
        }

        private void SelectAccount(Account account)
        {
            SelectedAccount = account;
        }

        private bool IsAvailableAccount(Account account)
        {
            return account != null;
        }

        #endregion

        #region Command: AddImageCommand

        private Command _addImageCommand;
        public Command AddImageCommand
        {
            get => _addImageCommand ?? (_addImageCommand = RegisterCommand(AddImage, CanEditContent));
        }

        private void AddImage()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Title = "アップロードするメディアを選択",
                Filter = UploadableExtensionFilter,
                DereferenceLinks = true,
                Multiselect = true,
            };

            if (DialogService.OpenModal(ofd)
                && HasEnableMediaFiles(ofd.FileNames))
            {
                Media.AddRange(ofd.FileNames.Select(UploadMedia.FromFile));
                UpdateCanPost();
            }

            ofd.Reset();
        }

        private static bool IsUploadableExtension(string ext)
        {
            return UploadableMediaExtensions.Contains(ext.ToLower());
        }

        private bool CanEditContent() => !_isTweetPosting;



        private static string CreateExtensionFilter()
        {
            // OpenFileDialogで用いる拡張子フィルタの生成
            // e.g. 表示名|*.ext1|表示名(拡張子複数指定)|*.ext2;*.ext2|...

            var medExts = $"アップロード可能なメディア|*{string.Join(";*", UploadableMediaExtensions)}";
            var imgExts = $"画像ファイル|*{string.Join(";*", ImageExtensions)}";
            var vidExts = $"動画ファイル|*{string.Join(";*", VideoExtensions)}";
            var allExts = "すべてのファイル|*.*";

            // 上記の文字列を‘|’(縦線)を区切り文字として結合
            return string.Join("|", medExts, imgExts, vidExts, allExts);
        }

        private static readonly string UploadableExtensionFilter = CreateExtensionFilter();

        #endregion

        #region Command: RemoveImageCommand

        private Command<UploadMedia> _removeMediaCommand;
        public Command<UploadMedia> RemoveMediaCommand
        {
            get => _removeMediaCommand ?? (_removeMediaCommand = RegisterCommand<UploadMedia>(RemoveMedia, CanRemoveMedia));
        }

        private bool CanRemoveMedia(UploadMedia media)
        {
            return Media.Contains(media);
        }

        private void RemoveMedia(UploadMedia media)
        {
            using (media)
            {
                Media.Remove(media);
                UpdateCanPost();
            }
        }

        #endregion

        #region Command: DragDropCommand

        private Command<IDataObject> _dragDropCommand;
        public Command<IDataObject> DragDropCommand
        {
            get => _dragDropCommand ?? (_dragDropCommand = RegisterCommand<IDataObject>(OnDrop, CanDrop));
        }

        private DragDropEffects _dragDropEffects = DragDropEffects.None;
        public DragDropEffects DragDropEffects
        {
            get => _dragDropEffects;
            set => SetProperty(ref _dragDropEffects, value);
        }

        private string _dropDescriptionMessage;
        public string DropDescriptionMessage
        {
            get => _dropDescriptionMessage;
            set => SetProperty(ref _dropDescriptionMessage, value);
        }

        private DropImageType _dropDescriptionIcon;
        public DropImageType DropDescriptionIcon
        {
            get => _dropDescriptionIcon;
            set => SetProperty(ref _dropDescriptionIcon, value);
        }

        private static bool HasEnableMediaFiles(StringCollection strCollection)
        {
            foreach (var str in strCollection)
            {
                if (IsUploadableExtension(Path.GetExtension(str)))
                    return true;
            }

            return false;
        }

        private static bool HasEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Any(f => IsUploadableExtension(Path.GetExtension(f)));
        }

        private static IEnumerable<string> GetEnableMediaFiles(StringCollection strCollection)
        {
            foreach (var str in strCollection)
            {
                if (IsUploadableExtension(Path.GetExtension(str)))
                    yield return str;
            }

            yield break;
        }

        private static IEnumerable<string> GetEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Where(f => IsUploadableExtension(Path.GetExtension(f)));
        }

        private bool CanDrop(IDataObject data)
        {
            if (!IsEditable) return false;

            if (data.GetDataPresent(DataFormats.FileDrop)
                && data.GetData(DataFormats.FileDrop) is string[] dropFiles
                && HasEnableMediaFiles(dropFiles))
            {
                DropDescriptionMessage = "添付";
                DragDropEffects = DragDropEffects.Copy;
                DropDescriptionIcon = DropImageType.Copy;
            }
            else if (UrlDataPresets.Any(data.GetDataPresent)
                || data.GetDataPresent(DataFormats.UnicodeText)
                || data.GetDataPresent(DataFormats.Text))
            {
                DropDescriptionMessage = "挿入";
                DragDropEffects = DragDropEffects.Copy;
                DropDescriptionIcon = DropImageType.Label;
            }
            else
            {
                DropDescriptionMessage = "無効な形式";
                DragDropEffects = DragDropEffects.None;
                DropDescriptionIcon = DropImageType.None;
                return false;
            }

            return true;
        }

        private void OnDrop(IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
            {
                var droppedFiles = (string[])data.GetData(DataFormats.FileDrop);
                Media.AddRange(GetEnableMediaFiles(droppedFiles).Select(UploadMedia.FromFile));
                UpdateCanPost();
            }
            else if (UrlDataPresets.Any(data.GetDataPresent))
            {
                TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
                TextBoxController.Focus();
            }
            else if (data.GetDataPresent(DataFormats.UnicodeText))
            {
                TextBoxController.Insert((string)data.GetData(DataFormats.UnicodeText));
                TextBoxController.Focus();
            }
            else if (data.GetDataPresent(DataFormats.Text))
            {
                TextBoxController.Insert((string)data.GetData(DataFormats.Text));
                TextBoxController.Focus();
            }
        }

        private readonly static string[] UrlDataPresets = { "IESiteModeToUrl", "text/x-moz-url", "UniformResourceLocator" };

        #endregion

        #region Command: NowPlaying

        private TextBoxController _textBoxController = new TextBoxController();
        public TextBoxController TextBoxController => _textBoxController;

        private string _nowPlayingPlayer = Setting.NowPlayingDefaultPlayer;
        public string NowPlayingPlayer
        {
            get => _nowPlayingPlayer;
            set => SetProperty(ref _nowPlayingPlayer, value, _getNowPlayingTextCommand);
        }

        private string _insertinNowPlayingText;
        public string InsertionNowPlayingText
        {
            get => _insertinNowPlayingText;
            set => SetProperty(ref _insertinNowPlayingText, value);
        }

        private bool _isNowPlayingPanelOpen;
        public bool IsNowPlayingPanelOpen
        {
            get => _isNowPlayingPanelOpen;
            set => SetProperty(ref _isNowPlayingPanelOpen, value);
        }

        public FluidCollection<ArtworkItem> NowPlayingArtworks { get; } = new FluidCollection<ArtworkItem>();

        #region Command: GetNowPlayingTextCommand

        private Command _getNowPlayingTextCommand;
        public Command GetNowPlayingTextCommand
        {
            get => _getNowPlayingTextCommand ?? (_getNowPlayingTextCommand = RegisterCommand(GetNowPlayingText, IsSupportedPlayer));
        }

        private bool IsSupportedPlayer()
        {
            return !string.IsNullOrEmpty(_nowPlayingPlayer)
                && NowPlayingPlayerList.ContainsKey(_nowPlayingPlayer);
        }

        private async void GetNowPlayingText()
        {
            var atwks = NowPlayingArtworks.ToArray();
            NowPlayingArtworks.Clear();
            atwks.DisposeAll();

            MediaPlayerBase player = null;

            if (!IsProcessRunning(_nowPlayingPlayer))
            {
                DialogService.MessageBox(
                    $"再生情報の取得に失敗しました。プレーヤが起動しているか確認してください。",
                    MsgBoxButtons.Ok, MsgBoxIcon.Error);
                return;
            }

            try
            {
                switch (_nowPlayingPlayer)
                {
                    case "wmplayer":
                        player = new WindowsMediaPlayer();
                        break;

                    case "itunes":
                        player = new iTunes();
                        break;

                    case "foobar2000":
                        player = new NowPlayingLib.Foobar2000();
                        break;

                    default:
                        return;
                }

                var media = await player.GetCurrentMedia();

                InsertionNowPlayingText = ReplaceMediaAlias(media, Setting.NowPlayingFormat);
                foreach (var stream in media.Artworks)
                {
                    try
                    {
                        NowPlayingArtworks.Add(new ArtworkItem(stream, Setting.InsertThumbnailAtNowPlayying));
                    }
                    catch { /* 非対応形式のアートワークは処理しない */ }
                }
            }
            catch (Exception ex)
            {
                DialogService.MessageBox(
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
        }

        public static IDictionary<string, string> NowPlayingPlayerList { get; }
        = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            ["wmplayer"] = "Windows Media Player",
            ["itunes"] = "iTunes",
            ["foobar2000"] = "foobar2000"
        });

        public static IDictionary<string, string> NowPlayingFormatParameters { get; }
        = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        {
            ["%album%"] = "アルバム名 (%album%)",
            ["%album_artist%"] = "アルバムアーティスト (%album_artist%)",
            ["%artist%"] = "アーティスト名 (%artist%)",
            ["%composer%"] = "作曲者 (%coposer%)",
            ["%category%"] = "カテゴリ (%category%)",
            ["%genre%"] = "ジャンル (%genre%)",
            ["%name%"] = "楽曲名 (%name%)",
            ["%number%"] = "トラック番号 (%nubmer%)",
            ["%year%"] = "年代 (%year%)",
        });

        private static bool IsProcessRunning(string processName)
        {
            return Process.GetProcessesByName(processName).Length > 0;
        }

        private string ReplaceMediaAlias(MediaItem media, string format)
        {
            var aliases = new Dictionary<string, string>
            {
                ["album"] = media.Album,
                ["album_artist"] = media.AlbumArtist,
                ["artist"] = media.Artist,
                ["composer"] = media.Composer,
                ["category"] = media.Category,
                ["genre"] = media.Genre,
                ["name"] = media.Name,
                ["number"] = media.TrackNumber.ToString(),
                ["year"] = media.Year.ToString()
            };

            string replacedString = format;

            foreach (var arias in aliases)
            {
                replacedString = Regex.Replace(
                    replacedString, $"%{arias.Key}%", arias.Value, RegexOptions.IgnoreCase);
            }

            return replacedString;
        }

        #endregion Command: GetNowPlayingTextCommand

        #region Command: InsertNowPlayingTextCommand

        private Command _insertNowPlayingTextCommand;
        public Command InsertNowPlayingTextCommand
        {
            get => _insertNowPlayingTextCommand ?? (_insertNowPlayingTextCommand = RegisterCommand(InsertNowPlaying));
        }

        private void InsertNowPlaying()
        {
            TextBoxController.Insert(InsertionNowPlayingText);

            foreach (var artwork in NowPlayingArtworks)
            {
                if (artwork.Use)
                {
                    Media.Add(UploadMedia.FromArtwork(artwork));
                    artwork.Dispose(false);
                }
                else
                {
                    artwork.Dispose(true);
                }
            }

            NowPlayingArtworks.Clear();

            InsertionNowPlayingText = null;
            IsNowPlayingPanelOpen = false;
        }

        #endregion Command: InsertNowPlayingTextCommand

        #endregion NowPlaying

        #region Command: ImagePasting

        private Command _pasteImageCommand;
        public Command PasteImageCommand
        {
            get => _pasteImageCommand ?? (_pasteImageCommand = RegisterCommand(OnImagePasted, CanImagePaste));
        }

        private bool CanImagePaste()
        {
            return Clipboard.ContainsImage()
                || (Clipboard.ContainsFileDropList() && HasEnableMediaFiles(Clipboard.GetFileDropList()));
        }

        private void OnImagePasted()
        {
            if (Clipboard.ContainsImage())
            {
                Media.Add(UploadMedia.FromBitmapSource(Clipboard.GetImage()));
            }
            else if (Clipboard.ContainsFileDropList())
            {
                Media.AddRange(
                    GetEnableMediaFiles(Clipboard.GetFileDropList())
                    .Select(UploadMedia.FromFile));
            }
        }

        #endregion

        internal override bool CanClose()
        {
            return !_isTweetPosting;
        }

        internal override void OnClosed()
        {
            _textBoxController = null;

            foreach (var media in Media)
            {
                media.Dispose();
            }

            base.OnClosed();
        }
    }


    internal class ArtworkItem : NotificationObject, IDisposable
    {
        public ArtworkItem(Stream stream, bool? use)
        {
            _image = new BitmapImage();
            _image.BeginInit();
            _image.StreamSource = stream;
            _image.EndInit();

            _artStream = stream;

            this._use = use ?? false;
        }

        private bool _use;
        public bool Use
        {
            get => _use;
            set => SetProperty(ref _use, value);
        }

        private BitmapImage _image;
        public BitmapImage Image => _image;

        private Stream _artStream;
        public Stream ArtStream => _artStream;

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposeStream)
        {
            if (disposeStream)
            {
                _artStream?.Dispose();
            }

            _image = null;
            _artStream = null;
        }
    }
}