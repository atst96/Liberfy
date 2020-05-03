using Liberfy.Commands;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public WindowStatus WindowStatus { get; }

        public MainWindowViewModel() : base()
        {
            this.Accounts = AccountManager.Accounts;
            this.SelectedAccount = this.Accounts?.FirstOrDefault();
            this.WindowStatus = App.Setting?.Window?.Main;

            this.MediaPreviewCommand = this.RegisterCommand(new DelegateCommand<MediaAttachmentInfo>(this.MediaPreview));
        }

        public IEnumerable<IAccount> Accounts { get; }

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set => this.SetProperty(ref this._selectedAccount, value);
        }

        private bool _initialized;
        internal override void OnInitialized()
        {
            if (this._initialized) return;
            this._initialized = true;
        }

        private Command<IAccount> _openTweetWindowCommand;
        public Command<IAccount> OpenTweetWindowCommand => this._openTweetWindowCommand ??= this.RegisterCommand(new OpenTweetWindowCommand(this));

        public IEnumerable<ColumnBase> Columns { get; } = TimelineBase.Columns;

        public Command<MediaAttachmentInfo> MediaPreviewCommand { get; }

        private void MediaPreview(MediaAttachmentInfo mediaItemInfo)
        {
            var viewModel = new MediaPreviewWindowViewModel();
            viewModel.SetMediaItemInfo(mediaItemInfo);

            this.Messenger.Raise(new TransitionMessage(viewModel, "MsgKey_PreviewAttachment"));
        }
    }
}
