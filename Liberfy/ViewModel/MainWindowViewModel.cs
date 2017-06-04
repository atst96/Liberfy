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
		public AccountSetting AccountSetting => App.AccountSetting;
		public FluidCollection<Account> Accounts => App.Accounts;
		public FluidCollection<ColumnBase> Columns => App.Columns;

		private bool _isAccountsLoaded;
		public bool IsAccountsLoaded
		{
			get => _isAccountsLoaded;
			set => SetProperty(ref _isAccountsLoaded, value);
		}

		private bool _initialized;
		internal override async void OnInitialized()
		{
			if (_initialized) return;
			_initialized = true;

			if (Accounts.Count == 0 && !DialogService.OpenInitSettingView())
			{
				DialogService.Invoke(ViewState.Close);
				return;
			}

			foreach (var account in Accounts.Where(a => a.AutomaticallyLogin))
			{
				account.IsLoading = true;

				if (await account.LoginAsync())
				{
					await account.LoadDetails();
				}

				account.IsLoading = false;
			}

			IsAccountsLoaded = true;
		}

		private bool _isAccountMenuOpen;
		public bool IsAccountMenuOpen
		{
			get => _isAccountMenuOpen;
			private set => SetProperty(ref _isAccountMenuOpen, value);
		}

		private void CloseAccountMenu()
		{
			IsAccountMenuOpen = false;
		}

		private Command<Account> _accountTweetCommand;
		public Command<Account> AccountTweetCommand
		{
			get => _accountTweetCommand ?? (_accountTweetCommand = RegisterCommand<Account>(AccountTweet));
		}

		private void AccountTweet(Account account)
		{
			DialogService.Open(ViewType.TweetWindow, account);
		}

		private Command _tweetCommand;
		public Command TweetCommand
		{
			get => _tweetCommand ?? (_tweetCommand = RegisterCommand(Tweet));
		}

		private void Tweet()
		{
			DialogService.Open(ViewType.TweetWindow);
		}

		private Command _showSettingDialog;
		public Command ShowSettingDialog
		{
			get => _showSettingDialog ?? (_showSettingDialog = RegisterCommand(() => DialogService.OpenSetting()));
		}

		internal override bool CanClose()
		{
			App.Shutdown(true);

			return true;
		}
	}
}
