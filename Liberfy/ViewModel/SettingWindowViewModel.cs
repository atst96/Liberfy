using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Liberfy
{
	class SettingWindowViewModel : ViewModelBase
	{
		public SettingWindowViewModel() : base()
		{
			ViewFonts = new FluidCollection<FontFamily>();
			SetFontSettings();
		}

		internal override void OnInitialized()
		{
			base.OnInitialized();
		}

		public Setting Setting => App.Setting;

		public AccountCollection Accounts => App.Accounts;

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

		public bool ShowInTaskTray
		{
			get { return Setting.ShowInTaskTray; }
			set
			{
				Setting.ShowInTaskTray = value;
				RaisePropertyChanged(nameof(ShowInTaskTray));
			}
		}

		public bool ShowInTaskTrayAtMinimized
		{
			get { return Setting.ShowInTaskTrayAtMinimzied; }
			set
			{
				Setting.ShowInTaskTrayAtMinimzied = value;
				RaisePropertyChanged(nameof(ShowInTaskTrayAtMinimized));
			}
		}

		#region General

		void SetFontSettings()
		{
			ViewFont = new FontFamily(string.Join(", ", Setting.TimelineFont));
			_viewFontSize = Setting.TimelineFontSize;
			ViewFonts.Reset(Setting.TimelineFont.Select(f => new FontFamily(f)));
			ViewFontRendering = Setting.TimelineFontRendering;

			RaisePropertyChanged(nameof(ViewFont));
			RaisePropertyChanged(nameof(IsLegacyGDIRender));
			RaisePropertyChanged(nameof(IsGDIPlusRender));
			RaisePropertyChanged(nameof(ViewFontSize));
		}

		public FontFamily ViewFont { get; set; }

		public bool IsLegacyGDIRender
		{
			get { return ViewFontRendering == TextFormattingMode.Display; }
			set
			{
				ViewFontRendering = TextFormattingMode.Display;
				RaisePropertyChanged(nameof(ViewFontRendering));
				RaisePropertyChanged(nameof(IsGDIPlusRender));
			}
		}

		public bool IsGDIPlusRender
		{
			get { return ViewFontRendering == TextFormattingMode.Ideal; }
			set
			{
				ViewFontRendering = TextFormattingMode.Ideal;
				RaisePropertyChanged(nameof(ViewFontRendering));
				RaisePropertyChanged(nameof(IsLegacyGDIRender));
			}
		}

		private double? _viewFontSize;
		public double? ViewFontSize
		{
			get { return _viewFontSize; }
			set
			{
				if (SetProperty(ref _viewFontSize, value))
				{
					FontCommand.RaiseCanExecute();
				}
			}
		}

		public TextFormattingMode ViewFontRendering { get; private set; }

		private string _selectedNewFont = "Arial";
		public string SelectedNewFont
		{
			get { return _selectedNewFont; }
			set
			{
				if (SetProperty(ref _selectedNewFont, value))
				{
					FontCommand.RaiseCanExecute();
				}
			}
		}

		private FontFamily _selectedFont;
		public FontFamily SelectedFont
		{
			get { return _selectedFont; }
			set
			{
				if (SetProperty(ref _selectedFont, value))
				{
					FontCommand.RaiseCanExecute();
				}
			}
		}

		public FluidCollection<FontFamily> ViewFonts { get; }

		private Command<string> _fontCommand;
		public Command<string> FontCommand => _fontCommand
			?? (_fontCommand = new DelegateCommand<string>(
			cmd =>
			{
				switch (cmd)
				{
					case "add":
						ViewFonts.Add(new FontFamily(_selectedNewFont));
						break;

					case "del":
						ViewFonts.Remove(_selectedFont);
						break;

					case "up":
						ViewFonts.MoveUp(ViewFonts.IndexOf(_selectedFont));
						FontCommand.RaiseCanExecute();
						break;

					case "down":
						ViewFonts.MoveDown(ViewFonts.IndexOf(_selectedFont));
						FontCommand.RaiseCanExecute();
						break;

					case "save":
						Setting.TimelineFont = ViewFonts.Select(f => f.Source).ToArray();
						Setting.TimelineFontSize = ViewFontSize.Value;
						Setting.TimelineFontRendering = ViewFontRendering;
						break;

					case "reset":
						Setting.TimelineFont = Setting.DefaultTimelineFont;
						Setting.TimelineFontSize = Setting.DefaultTimelineFontSize;
						goto case "reload";

					case "reload":
						SetFontSettings();

						RaisePropertyChanged(nameof(ViewFonts));
						break;
				}

				ViewFont = new FontFamily(string.Join(", ", ViewFonts.Select(f => f.Source)));

				RaisePropertyChanged(nameof(ViewFont));
			},
			cmd =>
			{
				switch (cmd)
				{
					case "add":
						return !string.IsNullOrWhiteSpace(_selectedNewFont);

					case "del":
						return _selectedFont != null;

					case "up":
						return _selectedFont != null
							&& MathEx.IsWithin(Math.Min(
								ViewFonts.Count, ViewFonts.IndexOf(_selectedFont)), 1, ViewFonts.Count);

					case "down":
						return _selectedFont != null
							&& Math.Max(0, ViewFonts.IndexOf(_selectedFont)) < ViewFonts.Count - 1;

					case "save":
						return ViewFonts.Count > 0 && ViewFontSize.HasValue;

					case "reset":
					case "reload":
						return true;

					default:
						return false;
				}
			}));

		#endregion

		#region Accounts

		private ColumnType _tempColumnType;
		public ColumnType TempColumnType
		{
			get { return _tempColumnType; }
			set { SetProperty(ref _tempColumnType, value); }
		}

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

		public FluidCollection<ColumnSetting> DefaultColumns => Setting.DefaultColumns;

		private Command _addDefaultColumnCommand;
		public Command AddDefaultColumnCommand => _addDefaultColumnCommand
			?? (_addDefaultColumnCommand = new DelegateCommand(() =>
			{
				DefaultColumns.Add(new ColumnSetting(_tempColumnType, Account.Dummy));
			}));

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

						await Task.Run(() =>
						{
							if (!acc.Login())
							{
								DialogService.MessageBox(
									$"アカウント情報の取得に失敗しました:\n",
									MsgBoxButtons.Ok, MsgBoxIcon.Information);

								Accounts.Remove(acc);
							}
						});

						acc.IsLoading = false;
					}
				}

			}));

		private ColumnType _addColumnType = ColumnType.Home;
		public ColumnType AddColumnType
		{
			get { return _addColumnType; }
			set { SetProperty(ref _addColumnType, value); }
		}

		#endregion

		#region Formats

		public int NowPlayingSelectionStart { get; set; }
		public int NowPlayingSelectionLength { get; set; }

		public string NowPlayingFormat
		{
			get { return Setting.NowPlayingFormat; }
			set
			{
				Setting.NowPlayingFormat = value;
				RaisePropertyChanged(nameof(NowPlayingFormat));
			}
		}

		private Command<string> _insertNowPlayingParamCommand;
		public Command<string> InsertNowPlayingParamCommand => _insertNowPlayingParamCommand
			?? (_insertNowPlayingParamCommand = new DelegateCommand<string>(p =>
			   {
				   int start = NowPlayingSelectionStart;
				   int length = NowPlayingSelectionLength;

				   NowPlayingFormat = NowPlayingSelectionLength == 0
				   ? NowPlayingFormat.Insert(start, p)
				   : NowPlayingFormat.Remove(start, length).Insert(start, p);

				   NowPlayingSelectionStart = start + length;
			   }));

		private Command _resetNowPlayingCommand;
		public Command ResetNowPlayingCommand => _resetNowPlayingCommand
			?? (_resetNowPlayingCommand = new DelegateCommand(() =>
			{
				NowPlayingFormat = Defines.DefaultNowPlayingFormat;
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
