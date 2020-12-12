using Liberfy.Managers;
using Livet.Messaging;
using System.Collections.Generic;
using System.Linq;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public WindowStatus WindowStatus { get; }

        public IEnumerable<IAccount> Accounts { get; }

        public MainWindowViewModel() : base()
        {
            this.Accounts = AccountManager.Accounts;
            this.SelectedAccount = this.Accounts?.FirstOrDefault();
            this.WindowStatus = App.Setting?.Window?.Main;
        }

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set => this.RaisePropertyChangedIfSet(ref this._selectedAccount, value);
        }

        private Command<IAccount> _openTweetWindowCommand;
        public Command<IAccount> OpenTweetWindowCommand
        {
            get => this._openTweetWindowCommand ??= this.RegisterCommand<IAccount>(this.OpenTweetWindow);
        }

        private void OpenTweetWindow(IAccount parameter)
        {
            var account = parameter ?? AccountManager.Accounts.FirstOrDefault();
            var viewModel = new TweetWindowViewModel();

            viewModel.SetPostAccount(account);

            this.Messenger.Raise(new TransitionMessage(viewModel, "MsgKey_OpenTweetDialog"));
        }

        public IEnumerable<ColumnBase> Columns { get; } = TimelineBase.Columns;

        private Command<MediaAttachmentInfo> _mediaPreviewCommand;
        public Command<MediaAttachmentInfo> MediaPreviewCommand
        {
            get => this._mediaPreviewCommand ??= this.RegisterCommand<MediaAttachmentInfo>(this.MediaPreview);
        }

        private void MediaPreview(MediaAttachmentInfo mediaItemInfo)
        {
            var viewModel = new MediaPreviewWindowViewModel();
            viewModel.SetMediaItemInfo(mediaItemInfo);

            this.Messenger.Raise(new TransitionMessage(viewModel, "MsgKey_PreviewAttachment"));
        }
    }
}
