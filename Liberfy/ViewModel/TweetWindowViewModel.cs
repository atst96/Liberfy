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
            get => this._selectedAccount;
            set => this.SetProperty(ref this._selectedAccount, value, this._postCommand);
        }

        public void SetPostAccount(Account account)
        {
            this.SelectedAccount = account;
        }

        private const int MaxTweetLength = 140;
        private const int MediaUrlLength = 23;

        public TweetWindow()
        {
            Media = new FluidCollection<UploadMedia>();

            SelectedAccount = this.Accounts.First();

            UpdateCanPost();
        }

        public FluidCollection<UploadMedia> Media { get; }

        public bool CanUpdateContent { get; private set; }
        public int RemainingTweetLength { get; private set; } = MaxTweetLength;

        private string _uploadStatusText;
        public string UploadStatusText
        {
            get => this._uploadStatusText;
            set => this.SetProperty(ref this._uploadStatusText, value);
        }

        private bool _isTweetPosting;
        public bool IsTweetPosting
        {
            get => this._isTweetPosting;
            set => this.SetProperty(ref this._isTweetPosting, value);
        }

        private string _tweet = string.Empty;
        public string Tweet
        {
            get => _tweet;
            set
            {
                var newTweet = value?.Replace("\r\n", "\n") ?? string.Empty;
                if (this.SetProperty(ref _tweet, newTweet))
                {
                    this.UpdateCanPost();
                }
            }
        }

        private void UpdateCanPost()
        {
            // ツイート可能な残り文字数の算出
            int actualTweetLength = tweetValidator.GetTweetLength(_tweet);

            if (this.Media.Count > 0)
            {
                actualTweetLength += MediaUrlLength;
            }

            this.RemainingTweetLength = MaxTweetLength - actualTweetLength;

            // アップロード可能かを判定
            bool isTweetableLength = this.RemainingTweetLength > 0 && this.RemainingTweetLength < MaxTweetLength;
            bool isPostableMediaCount = this.Media.Count <= 4;

            this.CanUpdateContent = isTweetableLength && isPostableMediaCount;

            // プロパティの変更通知
            this.RaisePropertyChanged(nameof(this.RemainingTweetLength));
            this.RaisePropertyChanged(nameof(this.CanUpdateContent));

            this.PostCommand.RaiseCanExecute();
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

        public StatusInfo ReplyToStatus { get; private set; }
        public bool HasReplyStatus { get; private set; } = false;

        public UserInfo ReplyUser { get; private set; }
        public bool HasReplyUser { get; private set; } = false;

        public void SetReplyToUser(UserInfo user)
        {
            this.ReplyUser = user;
            this.HasReplyUser = user != null;

            this.RaisePropertyChanged(nameof(this.ReplyUser));
            this.RaisePropertyChanged(nameof(this.HasReplyUser));
        }

        private static string WrapReplyText(string screenName) => $"@{ screenName } ";

        public void SetReplyToStatus(StatusInfo status)
        {
            this.ReplyToStatus = status;
            this.HasReplyStatus = status != null;

            this.RaisePropertyChanged(nameof(this.ReplyToStatus));
            this.RaisePropertyChanged(nameof(this.HasReplyStatus));

            var mentionEntity = status?.Entities?.UserMentions;

            int mentionEntityCount = mentionEntity?.Length ?? 0;

            if (mentionEntityCount == 0)
            {
                this.Tweet = WrapReplyText(status.User.ScreenName);
            }
            else if (Setting.IncludeOtherAtReply)
            {
                var mentionUserList = new LinkedList<string>(mentionEntity.Select(m => m.ScreenName));
                mentionUserList.AddFirst(status.User.ScreenName);

                var mentionList = mentionUserList
                    .Distinct(_stringIgnroeCaseCompare)
                    .Select(WrapReplyText);

                this.Tweet = string.Concat(mentionList);
            }
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

        public TextBoxController TextBoxController { get; private set; } = new TextBoxController();

        #region NowPlaying

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

        #endregion NowPlaying


        #region Command: PostCommand

        private Command _postCommand;
        public Command PostCommand
        {
            get => _postCommand ?? (_postCommand = RegisterCommand(PostTweet, CanPostTweet));
        }

        private Tokens Tokens => SelectedAccount.Tokens;
        private int _postTweetPhase = -1;

        public async void PostTweet()
        {
            this.OnPostBegin();

            var uploadPrams = new DictionaryEx<string, object>
            {
                ["status"] = this.Tweet,
                ["possibly_sensitive"] = this.IsSensitiveMedia,
            };

            if (this.HasReplyStatus)
                uploadPrams["in_reply_to_status_id"] = this.ReplyToStatus.Id;

            // 画像および動画のアップロード
            var uploadableMedia = this.GetUploadableMedia(Media);
            if (uploadableMedia.Any())
            {
                _postTweetPhase = 1;

                this.UploadStatusText = "メディアをアップロードしています...";
                uploadableMedia.ForEach(m => m.SetIsTweetPosting(true));

                if (await this.UploadMediaItems(this.Tokens, uploadableMedia).ConfigureAwait(true))
                {
                    uploadPrams["media_ids"] = Media
                        .Where(m => m.IsAvailableUploadId())
                        .Select(m => m.UploadId.Value);
                }
                else
                {
                    uploadableMedia.ForEach(m => m.SetIsTweetPosting(false));
                    this.OnPostEnd();
                    return;
                }
            }

            // ツイート
            _postTweetPhase = 2;
            this.UploadStatusText = "ツイートしています...";

            try
            {
                await this.Tokens.Statuses.UpdateAsync(uploadPrams);

                this.OnPostComplated();
            }
            catch (Exception ex)
            {
                this.DialogService.MessageBox(ex.Message, null);
            }

            this.OnPostEnd();

            if (_closeOnPostComplated)
            {
                this.DialogService.Invoke(ViewState.Close);
            }
        }

        private void OnPostComplated()
        {
            ClearStatus();
        }

        private void ClearStatus()
        {
            this.HasReplyUser = false;
            this.ReplyUser = null;
            this.HasReplyStatus = false;
            this.ReplyToStatus = null;

            var media = Media.ToArray();
            this.Media.Clear();

            media.DisposeAll();

            media = null;

            this.Tweet = string.Empty;
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
                if (this.DialogService.MessageBox(
                    $"{ media.Count() }件中{ badResCount }件 アップロードに失敗しました。\n再試行しますか？",
                    MsgBoxButtons.RetryCancel, MsgBoxIcon.Question) == MsgBoxResult.Retry)
                {
                    return await this.UploadMediaItems(token, this.GetUploadableMedia(media));
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
            return _selectedAccount != null && !_isTweetPosting && this.IsEditable && this.CanUpdateContent;
        }

        public bool IsEditable { get; private set; } = true;

        private void OnPostBegin()
        {
            this.IsTweetPosting = true;
            this.SetIsEditable(false);
            _postTweetPhase = 0;
        }

        private void OnPostEnd()
        {
            this.IsTweetPosting = false;
            this.SetIsEditable(true);
            _postTweetPhase = -1;
        }

        private void SetIsEditable(bool canEdit)
        {
            this.IsEditable = canEdit;
            this.RaisePropertyChanged(nameof(IsEditable));

            this.AddImageCommand.RaiseCanExecute();
            this.PostCommand.RaiseCanExecute();
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

        private Command _selectAccountCommand;
        public Command SelectAccountCommand => this._selectAccountCommand ?? (this._selectAccountCommand = this.RegisterCommand(() =>
        {
            var res = this.DialogService.SelectDialog(new SelectDialogOption<Account>
            {
                Instruction = "ツイートするアカウントを選択してください",
                Items = this.Accounts,
                ItemTemplate = Application.Current.TryFindResource("AccountItemTemplate") as DataTemplate,
            });
        }));

        private Command _showNowPlayingDialogCommand;
        public Command ShowNowPlayingDialogCommand => this._showNowPlayingDialogCommand ?? (this._showNowPlayingDialogCommand = this.RegisterCommand(() =>
        {
            var nowPlaying = new ViewModel.NowPlayingViewModel();

            if (this.DialogService.OpenModal(nowPlaying, new ViewOption
            {
                ResizeMode = ResizeMode.NoResize,
                StartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Manual,
                Width = 340,
                Height = 320,
                WindowChrome = new System.Windows.Shell.WindowChrome
                {
                    GlassFrameThickness = new Thickness(0, 1, 0, 0),
                    UseAeroCaptionButtons = false,
                    CornerRadius = new CornerRadius(0),
                    CaptionHeight = 0,
                },
                ShowInTaskbar = false,
            }))
            {
                this.TextBoxController.Insert(nowPlaying.InsertionText);

                foreach (var artwork in nowPlaying.Artworks)
                {
                    if (artwork.Use)
                    {
                        this.Media.Add(UploadMedia.FromArtwork(artwork));
                        artwork.Dispose(false);
                    }
                    else
                    {
                        artwork.Dispose(true);
                    }
                }

                nowPlaying.Artworks.Clear();
                nowPlaying.InsertionText = null;
                nowPlaying = null;
            }
        }));

        internal override bool CanClose()
        {
            return !_isTweetPosting;
        }

        internal override void OnClosed()
        {
            this.TextBoxController = null;
            
            this.Media.DisposeAll();

            base.OnClosed();
        }
    }


    internal class ArtworkItem : NotificationObject, IDisposable
    {
        public ArtworkItem(Stream stream, bool? use)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = stream;
            img.EndInit();
            this.Image = img;

            this.ArtStream = stream;

            this._use = use ?? false;
        }

        private bool _use;
        public bool Use
        {
            get => _use;
            set => SetProperty(ref _use, value);
        }

        public BitmapImage Image { get; private set; }

        public Stream ArtStream { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposeStream)
        {
            if (disposeStream)
            {
                this.ArtStream?.Dispose();
            }

            this.Image = null;
            this.ArtStream = null;
        }
    }
}