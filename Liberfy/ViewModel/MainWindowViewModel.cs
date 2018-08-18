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
        public WindowStatus WindowStatus { get; }

        public MainWindow() : base()
        {
            this.Accounts = AccountManager.Accounts;
            this.SelectedAccount = this.Accounts.FirstOrDefault();
            this.WindowStatus = App.Setting.Window.Main;
        }

        public IEnumerable<AccountBase> Accounts { get; }

        private AccountBase _selectedAccount;
        public AccountBase SelectedAccount
        {
            get => this._selectedAccount;
            set => this.SetProperty(ref this._selectedAccount, value);
        }

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

            if (AccountManager.Count == 0 && !this.DialogService.OpenInitSettingView())
            {
                this.DialogService.Invoke(ViewState.Close);
                return;
            }

            var accounts = this.Accounts
                .Where(a => a.AutomaticallyLogin);

            bool
                foundTwitterAccount = false,
                foundMastodonAccount = false;

            var foundServicesDataStore = new LinkedList<DataStore>();

            var loadingTasks = new LinkedList<Task>();

            foreach (var account in this.Accounts.Where(a => a.AutomaticallyLogin))
            {
                if (account.Service == SocialApis.SocialService.Twitter && !foundTwitterAccount)
                {
                    foundTwitterAccount = true;
                    DataStore.Twitter.BeginAddCollectionMode();
                    foundServicesDataStore.AddLast(DataStore.Twitter);
                }
                else if (account.Service == SocialApis.SocialService.Mastodon && !foundMastodonAccount)
                {
                    foundMastodonAccount = true;
                    DataStore.Mastodon.BeginAddCollectionMode();
                    foundServicesDataStore.AddLast(DataStore.Mastodon);
                }

                var task = account.Load();
                loadingTasks.AddLast(task);
            }

            await Task.WhenAll(loadingTasks);

            foreach (var dataStore in foundServicesDataStore)
            {
                dataStore.EndAddCollectionMode();
            }

            foundServicesDataStore.Clear();
            foundServicesDataStore = null;

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
            get => this._showSettingDialog ?? (this._showSettingDialog = this.RegisterCommand(() => this.DialogService.OpenSetting()));
        }

        public IEnumerable<ColumnBase> Columns { get; } = TimelineBase.Columns;

        internal override void OnClosed()
        {
            base.OnClosed();

            App.Shutdown(true);
        }
    }
}
