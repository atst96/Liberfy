using Liberfy.Commands;
using Liberfy.Commands.TweetWindow;
using Liberfy.Managers;
using Liberfy.Services.Common;
using Liberfy.Utils;
using Livet.Messaging.IO;
using NowPlayingLib;
using SocialApis.Twitter;
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
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfMvvmToolkit;
using static Liberfy.Defaults;

namespace Liberfy.ViewModels
{
    internal class TweetWindowViewModel : ViewModelBase
    {
        protected static Setting Setting { get; } = App.Setting;

        public IEnumerable<IAccount> Accounts { get; } = AccountManager.Accounts;

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._selectedAccount, value))
                {
                    this.PostCommand.RaiseCanExecute();
                    this.ServiceConfiguration = value.ServiceConfiguration;
                    this.UpdateShowSpoilderText();
                    this.UpdateShowPolls();
                    this.UpdateCanPost();
                }
            }
        }

        public TweetWindowViewModel()
        {
            this.PostParameters = new ServicePostParameters();
            this.PostParameters.PropertyChanged += this.OnPostParametersProeprtyChanged;

            //this.SelectedAccount = this.Accounts.First();
        }

        public void SetPostAccount(IAccount account)
        {
            this.SelectedAccount = account;
            this.UpdateCanPost();
        }

        private void OnPostParametersProeprtyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(this.PostParameters.SpoilerText):
                case nameof(this.PostParameters.Text):
                    this.UpdateCanPost();
                    break;

                case nameof(this.PostParameters.HasSpoilerText):
                    this.UpdateCanPost();
                    this.UpdateShowSpoilderText();
                    break;

                case nameof(this.PostParameters.HasPolls):
                    this.UpdateShowPolls();
                    break;
            }
        }

        public ServicePostParameters PostParameters { get; }

        private IServiceConfiguration _serviceConfiguration;
        public IServiceConfiguration ServiceConfiguration
        {
            get => this._serviceConfiguration;
            set => this.RaisePropertyChangedIfSet(ref this._serviceConfiguration, value);
        }

        private bool _isShowSpilerText;
        public bool IsShowSpoilerText
        {
            get => this._isShowSpilerText;
            set => this.RaisePropertyChangedIfSet(ref this._isShowSpilerText, value);
        }

        private bool _isShowPolls;
        public bool IsShowPolls
        {
            get => this._isShowPolls;
            set => this.RaisePropertyChangedIfSet(ref this._isShowPolls, value);
        }

        private void UpdateShowSpoilderText()
        {
            if (this.ServiceConfiguration != null)
            {
                this.IsShowSpoilerText = this.ServiceConfiguration.HasSpoilerText && this.PostParameters.HasSpoilerText;
            }
        }

        private void UpdateShowPolls()
        {
            if (this.ServiceConfiguration != null)
            {
                this.IsShowPolls = this.ServiceConfiguration.IsSupportPolls && this.PostParameters.HasPolls;
            }
        }

        private int _textLength;
        public int TextLength
        {
            get => this._textLength;
            set => this.RaisePropertyChangedIfSet(ref this._textLength, value);
        }

        private bool _canUpdateContent;
        public bool CanPostContent
        {
            get => this._canUpdateContent;
            set => this.RaisePropertyChangedIfSet(ref this._canUpdateContent, value);
        }

        private string _uploadStatusText;
        public string UploadStatusText
        {
            get => this._uploadStatusText;
            set => this.RaisePropertyChangedIfSet(ref this._uploadStatusText, value);
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get => this._isUploading;
            set => this.RaisePropertyChangedIfSet(ref this._isUploading, value);
        }

        internal void UpdateCanPost()
        {
            var postParams = this.PostParameters;
            var validator = this.SelectedAccount?.Validator;

            if (validator != null)
            {
                this.TextLength = validator.GetTextLength(this.PostParameters);
                this.CanPostContent = validator.CanPost(this.PostParameters);
            }

            this.PostCommand.RaiseCanExecute();
        }

        private bool _isSensitiveMedia = Setting.NoticePostSensitiveMedia;
        public bool IsSensitiveMedia
        {
            get => _isSensitiveMedia;
            set => RaisePropertyChangedIfSet(ref _isSensitiveMedia, value);
        }

        private bool _closeOnPostComplated = Setting.CloseWindowAfterPostComplated;
        public bool CloseOnPostComplated
        {
            get => _closeOnPostComplated;
            set => RaisePropertyChangedIfSet(ref _closeOnPostComplated, value);
        }

        public IStatusInfo ReplyToStatus { get; private set; }
        public bool HasReplyStatus { get; private set; } = false;

        public IUserInfo ReplyUser { get; private set; }
        public bool HasReplyUser { get; private set; } = false;

        public void SetReplyToUser(IUserInfo user)
        {
            this.ReplyUser = user;
            this.HasReplyUser = user != null;

            this.RaisePropertyChanged(nameof(this.ReplyUser));
            this.RaisePropertyChanged(nameof(this.HasReplyUser));
        }

        private static string WrapReplyText(string screenName) => $"@{ screenName } ";

        public void SetReplyToStatus(StatusItem statusItem)
        {
            if (statusItem == null)
            {
                return;
            }

            //this.ReplyToStatus = status;
            //this.HasReplyStatus = status != null;

            //this.RaisePropertyChanged(nameof(this.ReplyToStatus));
            //this.RaisePropertyChanged(nameof(this.HasReplyStatus));

            //var mentionEntity = status?.Entities.UserMentions;

            //int mentionEntityCount = mentionEntity?.Length ?? 0;

            //if (mentionEntityCount == 0)
            //{
            //    this.Tweet = WrapReplyText(status.User.ScreenName);
            //}
            //else if (Setting.IncludeOtherAtReply)
            //{
            //    var mentionUserList = new LinkedList<string>(mentionEntity.Select(m => m.ScreenName));
            //    mentionUserList.AddFirst(status.User.ScreenName);

            //    var mentionList = mentionUserList
            //        .Distinct(_stringIgnroeCaseCompare)
            //        .Select(WrapReplyText);

            //    this.Tweet = string.Concat(mentionList);
            //}
        }

        private static StringIgnoreCaseEqualityComparer _stringIgnroeCaseCompare = new StringIgnoreCaseEqualityComparer();
        private class StringIgnoreCaseEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj) => this.GetHashCode();
        }

        private Command _addPollItemCommand;
        public Command AddPollItemCommand => this._addPollItemCommand ??= this.RegisterCommand(new AddPollItemCommand(this));

        private Command _removePollItemCommand;
        public Command RemovePollItemCommand => this._removePollItemCommand ??= this.RegisterCommand(new RemovePollItemCommand(this));

        public IReadOnlyDictionary<string, int> PollDurationList { get; } = new Dictionary<string, int>
        {
            ["5 分"] = 301,
            ["30 分"] = 1800,
            ["1 時間"] = 3600,
            ["6 時間"] = 21600,
            ["1 日"] = 86400,
            ["3 日"] = 259200,
            ["7 日"] = 604800,
        };

        public TextBoxController TextBoxController { get; private set; } = new TextBoxController();

        #region NowPlaying

        public static IReadOnlyDictionary<string, string> NowPlayingPlayerList { get; } = new Dictionary<string, string>
        {
            ["wmplayer"] = "Windows Media Player",
            ["itunes"] = "iTunes",
            ["foobar2000"] = "foobar2000"
        };

        public static IReadOnlyDictionary<string, string> NowPlayingFormatParameters { get; } = new Dictionary<string, string>
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
        };

        private Command<string> _nowPlayingCommand;
        public Command<string> NowPlayingCommand => this._nowPlayingCommand ??= this.RegisterCommand(new InsertNowPlayingCommand(this));

        #endregion NowPlaying


        #region Command: PostCommand

        private Command<IAccount> _postCommand;
        public Command<IAccount> PostCommand => this._postCommand ??= this.RegisterCommand(new PostCommand(this));

        internal void ClearStatus()
        {
            this.HasReplyUser = false;
            this.ReplyUser = null;
            this.HasReplyStatus = false;
            this.ReplyToStatus = null;

            this.PostParameters.Clear();
        }

        public bool CanPostTweet()
        {
            return this.SelectedAccount != null && !this.IsUploading && !this.IsBusy && this.CanPostContent;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => this._isBusy;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._isBusy, value))
                {
                    this._addImageCommand?.RaiseCanExecute();
                    this._postCommand?.RaiseCanExecute();
                }
            }
        }

        internal void BeginUpload()
        {
            this.IsUploading = true;
            this.IsBusy = true;
        }

        internal void EndUpload()
        {
            this.IsUploading = false;
            this.IsBusy = false;
        }

        #endregion

        private Command<OpeningFileSelectionMessage> _addImageCommand;
        public Command<OpeningFileSelectionMessage> AddImageCommand => this._addImageCommand ??= this.RegisterCommand(new AddImageCommand(this));

        private Command<UploadMedia> _removeMediaCommand;
        public Command<UploadMedia> RemoveMediaCommand => this._removeMediaCommand ??= this.RegisterCommand(new RemoveMediaCommand(this));

        public static bool IsUploadableExtension(string ext)
        {
            return UploadableMediaExtensions.Contains(ext.ToLower());
        }

        private Command<IDataObject> _dragDropCommand;
        public Command<IDataObject> DragDropCommand
        {
            get => this._dragDropCommand ??= this.RegisterCommand<IDataObject>(this.OnDragDrop, this.ValidateDragDrop);
        }

        private bool ValidateDragDrop(IDataObject dataObject)
        {
            if (this.IsBusy)
            {
                return false;
            }

            var dataType = DragDropUtil.GetDataType(dataObject);
            switch (dataType)
            {
                case DragDropDataType.Text:
                case DragDropDataType.Url:
                    this.DropDescriptionMessage = "挿入";
                    this.DragDropEffects = DragDropEffects.Copy;
                    this.DropDescriptionIcon = DropImageType.Label;
                    return true;

                case DragDropDataType.FileDrop when TweetWindowViewModel.HasEnableMediaFiles(DragDropUtil.GetFileDrop(dataObject)):
                    this.DropDescriptionMessage = "添付";
                    this.DragDropEffects = DragDropEffects.Copy;
                    this.DropDescriptionIcon = DropImageType.Copy;
                    return true;

                default:
                    this.DropDescriptionMessage = "無効な形式";
                    this.DragDropEffects = DragDropEffects.None;
                    this.DropDescriptionIcon = DropImageType.None;
                    return false;
            }
        }

        private void OnDragDrop(IDataObject dataObject)
        {
            var dataType = DragDropUtil.GetDataType(dataObject);
            switch (dataType)
            {
                case DragDropDataType.Text:
                case DragDropDataType.Url:
                    var tbController = this.TextBoxController;
                    if (DragDropUtil.TryGetString(dataObject, out var value))
                    {
                        tbController.Insert(value);
                        tbController.Focus();
                    }
                    return;

                case DragDropDataType.FileDrop:
                    var droppedFiles = DragDropUtil.GetFileDrop(dataObject);
                    var filteredFils = GetEnableMediaFiles(droppedFiles).Select(path => UploadMedia.FromFile(path));
                    this.PostParameters.Attachments.AddRange(filteredFils);
                    this.UpdateCanPost();
                    return;
            }
        }

        private static IEnumerable<string> GetEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Where(f => IsUploadableExtension(Path.GetExtension(f)));
        }

        private DragDropEffects _dragDropEffects;
        public DragDropEffects DragDropEffects
        {
            get => this._dragDropEffects;
            set => this.RaisePropertyChangedIfSet(ref this._dragDropEffects, value);
        }

        private string _dropDescriptionMessage;
        public string DropDescriptionMessage
        {
            get => this._dropDescriptionMessage;
            set => this.RaisePropertyChangedIfSet(ref this._dropDescriptionMessage, value);
        }

        private DropImageType _dropDescriptionIcon;
        public DropImageType DropDescriptionIcon
        {
            get => this._dropDescriptionIcon;
            set => this.RaisePropertyChangedIfSet(ref this._dropDescriptionIcon, value);
        }

        public static bool HasEnableMediaFiles(StringCollection strCollection)
        {
            foreach (var str in strCollection)
            {
                if (IsUploadableExtension(Path.GetExtension(str)))
                    return true;
            }

            return false;
        }

        public static bool HasEnableMediaFiles(IEnumerable<string> files)
        {
            return files.Any(f => IsUploadableExtension(Path.GetExtension(f)));
        }


        private Command _pasteImageCommand;
        public Command PasteImageCommand => this._pasteImageCommand ?? (this._pasteImageCommand = this.RegisterCommand(new PasteImageCommand(this)));

        private ICommand _closeCommand;
        public ICommand CloseCommand => this._closeCommand ??= this.RegisterCommand(this.OnClose, this.OnCloseRequest);

        internal bool OnCloseRequest()
        {
            return !this.IsUploading;
        }

        internal void OnClose()
        {
            this.TextBoxController = null;

            this.PostParameters.Attachments.DisposeAll();
        }

        public string Filter { get; } = GetFilter();

        private static string GetFilter()
        {
            var filters = CreateExtensionFilter().Select(p => $"{p.name}|*{string.Join(";*", p.extentions)}");
            return string.Join("|", filters);
        }

        private static IReadOnlyList<(string name, IReadOnlyList<string> extentions)> CreateExtensionFilter()
        {
            // OpenFileDialogで用いる拡張子フィルタの生成
            // e.g. 表示名|*.ext1|表示名(拡張子複数指定)|*.ext2;*.ext2|...

            return new (string, IReadOnlyList<string>)[]
            {
                ("アップロード可能なメディア", UploadableMediaExtensions),
                ("画像ファイル", ImageExtensions),
                ("動画ファイル", VideoExtensions),
                ("すべてのファイル", new[] { ".*" }),
            };
        }
    }
}
