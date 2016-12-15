using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	class SettingWindowViewModel : ViewModelBase
	{
		internal override void OnInitialized()
		{
			base.OnInitialized();
		}

		public Setting Setting { get; } = App.Setting;
		public AccountCollection Accounts { get; } = App.Accounts;

		public bool AutoStartup
		{
			get
			{
				using (var hkcu = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
				{
					var path = Regex.Escape(App.Assembly.Location);
					var fileName = hkcu.GetValue(App.ApplicationName) as string;
					return !string.IsNullOrEmpty(fileName)
						&& Regex.IsMatch(fileName, $@"^(""{path}""|{path})(?<params>\s+.*)?$", RegexOptions.IgnoreCase);
				}
			}
			set
			{
				try
				{
					if (Equals(AutoStartup, value)) return;

					if (!AutoStartup)
					{
						using (var reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
							reg.SetValue(App.ApplicationName, $"\"{App.Assembly.Location}\" /startup");
					}
					else if (AutoStartup)
					{
						using (var reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
							reg.DeleteValue(App.ApplicationName);
					}

					DialogService.MessageBox($"自動起動を{(value ? "有効" : "無効")}にしました", MsgBoxButtons.Ok, MsgBoxIcon.Information);

				}
				catch (Exception ex)
				{
					DialogService.MessageBox("自動起動の設定に失敗しました: " + ex.Message, MsgBoxButtons.Ok, MsgBoxIcon.Information);
				}

				RaisePropertyChanged("AutoStartup");
			}
		}

		public bool IsAccountPage { get; private set; }

		private int _tabPageIndex;
		public int TabPageIndex
		{
			get { return _tabPageIndex; }
			set
			{
				if (SetProperty(ref _tabPageIndex, value))
				{
					IsAccountPage = value == 1;
					RaisePropertyChanged("IsAccountPage");
				}
			}
		}

		#region Accounts

		private int _selectedAccountIndex = -1;
		public int SelectedAccountIndex
		{
			get { return _selectedAccountIndex; }
			set
			{
				if (SetProperty(ref _selectedAccountIndex, value))
				{
					_addAccountCommand.RaiseCanExecute();
					_deleteAccountCommand.RaiseCanExecute();
					_accountMoveUpCommand.RaiseCanExecute();
					_accountMoveDownCommand.RaiseCanExecute();
				}
			}
		}

		private Command _accountMoveUpCommand;
		public Command AccountMoveUpCommand => _accountMoveUpCommand
			?? (_accountMoveUpCommand = new DelegateCommand(
				() => Accounts.Move(_selectedAccountIndex, _selectedAccountIndex + 1),
				__ => _selectedAccountIndex >= 0 && _selectedAccountIndex < Accounts.Count - 1));

		private Command _accountMoveDownCommand;
		public Command AccountMoveDownCommand => _accountMoveDownCommand
			?? (_accountMoveDownCommand = new DelegateCommand(
				() => Accounts.Move(_selectedAccountIndex, _selectedAccountIndex - 1),
				__ => _selectedAccountIndex > 0));

		private Command _deleteAccountCommand;
		public Command DeleteAccountCommand => _deleteAccountCommand
			?? (_deleteAccountCommand = new DelegateCommand(() =>
			{
			}, _ => _selectedAccountIndex >= 0));

		private Command _addAccountCommand;
		public Command AddAccountCommand => _addAccountCommand
			?? (_addAccountCommand = new DelegateCommand(async () =>
			{
				var auth = new ViewModel.AuthenticationViewModel();

				if (DialogService.OpenModal(auth, new ViewOption
				{
					ResizeMode = ResizeMode.NoResize,
					StartupLocation = WindowStartupLocation.CenterOwner,
					SizeToContent = SizeToContent.Manual,
					Width = 400,
					Height = 240,
					WindowChrome = new System.Windows.Shell.WindowChrome
					{
						GlassFrameThickness = new Thickness(0),
						UseAeroCaptionButtons = false,
						CornerRadius = new CornerRadius(0),
						CaptionHeight = 0,
					},
					ShowInTaskbar = false,
				}))
				{
					var tokens = auth.Tokens;

					var acc = Accounts.FirstOrDefault((a) => a.Id == tokens.UserId);

					if (acc != null)
					{
						acc.SetTokens(tokens);
					}
					else
					{
						acc = new Account(tokens);
						Accounts.Add(acc);

						acc.IsLoading = true;

						try
						{
							acc.SetUser(await tokens.Account.VerifyCredentialsAsync());
						}
						catch (Exception ex)
						{
							DialogService.MessageBox(
								$"アカウント情報の取得に失敗しました:\n{ex.Message}",
								MsgBoxButtons.Ok, MsgBoxIcon.Information);

							Accounts.Remove(acc);
						}

						acc.IsLoading = false;
					}
				}

			}));

		#endregion

		public override bool CanClose()
		{
			if (!App.Accounts.Any())
			{
				return DialogService.MessageBox(
					"アカウントが登録されていません。終了しますか？", "Liberfy",
					MsgBoxButtons.YesNo, MsgBoxIcon.Question) == MsgBoxResult.Yes;
			}

			return true;
		}
	}
}
