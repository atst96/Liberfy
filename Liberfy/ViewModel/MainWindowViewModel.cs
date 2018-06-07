using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy.ViewModel
{
    internal class MainWindow : ViewModelBase
    {
        public FluidCollection<AccountBase> Accounts => App.Accounts;

        private bool _isAccountsLoaded;
        public bool IsAccountsLoaded
        {
            get => this._isAccountsLoaded;
            set => this.SetProperty(ref this._isAccountsLoaded, value);
        }

        private bool _initialized;
        internal override async void OnInitialized()
        {
            if (this._initialized) return;
            this._initialized = true;

            if (this.Accounts.Count == 0 && !this.DialogService.OpenInitSettingView())
            {
                this.DialogService.Invoke(ViewState.Close);
                return;
            }

            foreach (var account in this.Accounts.Where(a => a.AutomaticallyLogin))
            {
                if (await account.TryLogin())
                {
                    await account.TryLoadDetails();
                    account.StartTimeline();
                }
            }

            this.IsAccountsLoaded = true;
        }

        private bool _isAccountMenuOpen;
        public bool IsAccountMenuOpen
        {
            get => this._isAccountMenuOpen;
            set => this.SetProperty(ref this._isAccountMenuOpen, value);
        }

        private void CloseAccountMenu()
        {
            this.IsAccountMenuOpen = false;
        }

        private Command<AccountBase> _accountTweetCommand;
        public Command<AccountBase> AccountTweetCommand
        {
            get => this._accountTweetCommand ?? (this._accountTweetCommand = this.RegisterCommand<AccountBase>(AccountTweet));
        }

        private void AccountTweet(AccountBase account)
        {
            DialogService.Open(ViewType.TweetWindow, account);
        }

        private Command _tweetCommand;
        public Command TweetCommand
        {
            get => this._tweetCommand ?? (this._tweetCommand = this.RegisterCommand(Tweet));
        }

        private void Tweet()
        {
            DialogService.Open(ViewType.TweetWindow);
        }

        private Command _showSettingDialog;
        public Command ShowSettingDialog
        {
            get =>this. _showSettingDialog ?? (this._showSettingDialog = this.RegisterCommand(() => this.DialogService.OpenSetting()));
        }

        internal override bool CanClose()
        {
            App.Shutdown(true);
            return true;
        }

        public IEnumerable<ColumnBase> Columns
        {
            get => this.Accounts.First().Timeline.Columns;
        }
    }
}
