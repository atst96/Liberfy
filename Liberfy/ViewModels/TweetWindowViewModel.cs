using Liberfy.Commands;
using Liberfy.Services.Common;
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
using System.Windows.Media.Imaging;
using Twitter.Text;
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
                if (this.SetProperty(ref this._selectedAccount, value, this._postCommand))
                {
                    this.ServiceConfiguration = value.ServiceConfiguration;
                    this.UpdateShowSpoilderText();
                    this.UpdateCanPost();
                }
            }
        }

        public void SetPostAccount(IAccount account)
        {
            this.SelectedAccount = account;
            this.UpdateCanPost();
        }

        public TweetWindowViewModel()
        {
            this.PostParameters = new ServicePostParameters();
            this.PostParameters.PropertyChanged += this.OnPostParametersProeprtyChanged;

            this.SelectedAccount = this.Accounts.First();

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
            }
        }

        public ServicePostParameters PostParameters { get; }

        private IServiceConfiguration _serviceConfiguration;
        public IServiceConfiguration ServiceConfiguration
        {
            get => this._serviceConfiguration;
            set => this.SetProperty(ref this._serviceConfiguration, value);
        }

        private bool _isShowSpilerText;
        public bool IsShowSpoilerText
        {
            get => this._isShowSpilerText;
            set => this.SetProperty(ref this._isShowSpilerText, value);
        }

        private void UpdateShowSpoilderText()
        {
            if (this.ServiceConfiguration != null)
            {
                this.IsShowSpoilerText = this.ServiceConfiguration.HasSpoilerText && this.PostParameters.HasSpoilerText;
            }
        }

        private int _textLength;
        public int TextLength
        {
            get => this._textLength;
            set => this.SetProperty(ref this._textLength, value);
        }

        private bool _canUpdateContent;
        public bool CanPostContent
        {
            get => this._canUpdateContent;
            set => this.SetProperty(ref this._canUpdateContent, value);
        }

        private string _uploadStatusText;
        public string UploadStatusText
        {
            get => this._uploadStatusText;
            set => this.SetProperty(ref this._uploadStatusText, value);
        }

        private bool _isUploading;
        public bool IsUploading
        {
            get => this._isUploading;
            set => this.SetProperty(ref this._isUploading, value);
        }

        internal void UpdateCanPost()
        {
            var postParams = this.PostParameters;
            var validator = this.SelectedAccount.Validator;

            this.TextLength = validator.GetTextLength(this.PostParameters);
            this.CanPostContent = validator.CanPost(this.PostParameters);

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

        private Command<string> _nowPlayingCommand;
        public Command<string> NowPlayingCommand => this._nowPlayingCommand ?? (this._nowPlayingCommand = this.RegisterCommand(new InsertNowPlayingCommand(this)));

        #endregion NowPlaying


        #region Command: PostCommand

        private Command<IAccount> _postCommand;
        public Command<IAccount> PostCommand => this._postCommand ?? (this._postCommand = this.RegisterCommand(new PostCommand(this)));

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
                if (this.SetProperty(ref this._isBusy, value))
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

        private Command<string> _addImageCommand;
        public Command<string> AddImageCommand => this._addImageCommand ?? (this._addImageCommand = this.RegisterCommand(new AddImageCommand(this)));

        private Command<UploadMedia> _removeMediaCommand;
        public Command<UploadMedia> RemoveMediaCommand => this._removeMediaCommand ?? (this._removeMediaCommand = this.RegisterCommand(new RemoveMediaCommand(this)));

        public static bool IsUploadableExtension(string ext)
        {
            return UploadableMediaExtensions.Contains(ext.ToLower());
        }

        private Command<IDataObject> _dragDropCommand;
        public Command<IDataObject> DragDropCommand => this._dragDropCommand ?? (this._dragDropCommand = this.RegisterCommand(new DragDropCommand(this)));

        private DragDropEffects _dragDropEffects;
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

        internal override bool CanClose()
        {
            return !this.IsUploading;
        }

        internal override void OnClosed()
        {
            this.TextBoxController = null;

            this.PostParameters.Attachments.DisposeAll();

            base.OnClosed();
        }
    }
}