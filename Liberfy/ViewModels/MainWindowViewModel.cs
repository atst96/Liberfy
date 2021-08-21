using Liberfy.Conats.MessageKeys;
using Liberfy.Managers;
using Livet.Messaging;
using System.Linq;
using WpfMvvmToolkit;

namespace Liberfy.ViewModels
{
    /// <summary>
    /// <see cref="Views.MainWindow"/>のViewModel
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public WindowStatus WindowStatus { get; }

        /// <summary>
        /// アカウント情報
        /// </summary>
        public AccountManager Accounts { get; } = App.Accounts;

        /// <summary>
        /// カラム情報
        /// </summary>
        public ColumnManageer Columns { get; } = App.Columns;

        /// <summary>
        /// <see cref="MainWindowViewModel"/>を生成する
        /// </summary>
        public MainWindowViewModel() : base()
        {
            this.Accounts = App.Accounts;
            this.SelectedAccount = this.Accounts.GetDefault();
            this.WindowStatus = App.Setting?.Window?.Main;
        }

        private IAccount _selectedAccount;

        /// <summary>
        /// 選択中のアカウント
        /// </summary>
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set => this.RaisePropertyChangedIfSet(ref this._selectedAccount, value);
        }

        private Command<IAccount> _openPostWindowCommand;

        /// <summary>
        /// 投稿ウィンドウを開くコマンド
        /// </summary>
        public Command<IAccount> OpenPostWindowCommand => this._openPostWindowCommand ??= this.RegisterCommand<IAccount>(account =>
        {
            account ??= App.Accounts.GetDefault();
            var viewModel = new TweetWindowViewModel();

            viewModel.SetPostAccount(account);

            this.Messenger.Raise(new TransitionMessage(viewModel, MainWindowMessageKeys.OpenPostWindow));
        });

        private Command<MediaAttachmentInfo> _mediaPreviewCommand;

        /// <summary>
        /// 添付画像を開くコマンド
        /// </summary>
        public Command<MediaAttachmentInfo> MediaPreviewCommand => this._mediaPreviewCommand ??= this.RegisterCommand<MediaAttachmentInfo>(mediaItemInfo =>
        {
            var viewModel = new MediaPreviewWindowViewModel();
            viewModel.SetMediaItemInfo(mediaItemInfo);

            this.Messenger.Raise(new TransitionMessage(viewModel, MainWindowMessageKeys.PreviewAttachment));
        });
    }
}
