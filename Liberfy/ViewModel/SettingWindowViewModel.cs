using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace Liberfy.ViewModel
{
	class SettingWindow : ViewModelBase
	{
		public SettingWindow() : base()
		{
			ViewFonts = new FluidCollection<FontFamily>();
			SetFontSettings();
		}

		public Setting Setting => App.Setting;

		public AccountSetting AccountSetting => App.AccountSetting;
		public FluidCollection<Account> Accounts => AccountSetting.Accounts;

		private const string AutoStartupRegSubKey = @"Software\Microsoft\Windows\CurrentVersion\Run";

		private RegistryKey CreateAutoStartuRegSubKey()
		{
			return Registry.CurrentUser.OpenSubKey(AutoStartupRegSubKey, true);
		}

		private bool GetAutoStartupEnabled()
		{
			using (var reg = CreateAutoStartuRegSubKey())
			{
				var path = Regex.Escape(App.Assembly.Location);

				return reg.GetValue(App.ApplicationName) is string regKeyValue
					&& !string.IsNullOrEmpty(regKeyValue)
					&& Regex.IsMatch(regKeyValue, $@"^(""{path}""|{path})(?<params>\s+.*)?$", RegexOptions.IgnoreCase);
			}
		}

		public bool IsAccountPage { get; private set; }

		private int _tabPageIndex;
		public int TabPageIndex
		{
			get => _tabPageIndex;
			set
			{
				if (SetProperty(ref _tabPageIndex, value))
				{
					IsAccountPage = value == 1;
					RaisePropertyChanged(nameof(IsAccountPage));
				}
			}
		}

		public bool ShowInTaskTray
		{
			get => Setting.ShowInTaskTray;
			set
			{
				Setting.ShowInTaskTray = value;
				RaisePropertyChanged(nameof(ShowInTaskTray));
			}
		}

		public bool AutoStartup
		{
			get => GetAutoStartupEnabled();
			set
			{
				try
				{
					bool isAutoStartupEnabled = GetAutoStartupEnabled();
					if (Equals(isAutoStartupEnabled, value)) return;

					using (var reg = CreateAutoStartuRegSubKey())
					{
						if (isAutoStartupEnabled)
						{
							reg.DeleteValue(App.ApplicationName);
						}
						else
						{
							reg.SetValue(App.ApplicationName, $"\"{App.Assembly.Location}\" /startup");
						}
					}

					DialogService.MessageBox($"自動起動を{(value ? "有効" : "無効")}にしました", MsgBoxButtons.Ok, MsgBoxIcon.Information);

				}
				catch (Exception ex)
				{
					DialogService.MessageBox("自動起動の設定に失敗しました: " + ex.Message, MsgBoxButtons.Ok, MsgBoxIcon.Information);
				}

				RaisePropertyChanged(nameof(AutoStartup));
			}
		}

		public bool ShowInTaskTrayAtMinimized
		{
			get => Setting.ShowInTaskTrayAtMinimzied;
			set
			{
				Setting.ShowInTaskTrayAtMinimzied = value;
				RaisePropertyChanged(nameof(ShowInTaskTrayAtMinimized));
			}
		}

		#region General

		private void SetFontSettings()
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
			get => ViewFontRendering == TextFormattingMode.Display;
			set
			{
				ViewFontRendering = TextFormattingMode.Display;
				RaisePropertyChanged(nameof(ViewFontRendering));
				RaisePropertyChanged(nameof(IsGDIPlusRender));
			}
		}

		public bool IsGDIPlusRender
		{
			get => ViewFontRendering == TextFormattingMode.Ideal;
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
			get => _viewFontSize;
			set
			{
				if (SetProperty(ref _viewFontSize, value))
				{
					UpdateFontUI();
				}
			}
		}

		public TextFormattingMode ViewFontRendering { get; private set; }

		private string _fontFontFilter;
		public string FontFontFilter
		{
			get => _fontFontFilter;
			set
			{
				if (SetProperty(ref _fontFontFilter, value))
				{
					FilteringFontFontList();
				}
			}
		}

		private ICollection<FontFamily> _baseFontList = Fonts.SystemFontFamilies;
		private List<FontFamily> _fontFontList;
		public List<FontFamily> FontFontList
		{
			get
			{
				if (_fontFontList == null)
				{
					FilteringFontFontList();
				}

				return _fontFontList;
			}
		}

		private static readonly StringComparison strComp = StringComparison.CurrentCultureIgnoreCase;

		private void FilteringFontFontList()
		{
			if (string.IsNullOrWhiteSpace(_fontFontFilter))
			{
				_fontFontList = new List<FontFamily>(_baseFontList);
			}
			else
			{
				bool isFontFiltering(FontFamily fontFamily)
				{
					return fontFamily.Source.Contains(_fontFontFilter, strComp)
						|| fontFamily.FamilyNames.Any(kvp => kvp.Value.Contains(_fontFontFilter, strComp));
				}

				_fontFontList = new List<FontFamily>(_baseFontList.Where(isFontFiltering));
			}

			RaisePropertyChanged(nameof(FontFontList));
		}

		private FontFamily _newSelectedFont;
		public FontFamily NewSelectedFont
		{
			get => _newSelectedFont;
			set => SetProperty(ref _newSelectedFont, value, _addFontCommand);
		}

		private FontFamily _selectedFont;
		public FontFamily SelectedFont
		{
			get => _selectedFont;
			set
			{
				if (SetProperty(ref _selectedFont, value))
				{
					UpdateFontUI();
				}
			}
		}

		public FluidCollection<FontFamily> ViewFonts { get; }

		private void ReloadViewFont()
		{
			ViewFont = new FontFamily(string.Join(", ", ViewFonts.Select(f => f.Source)));
			RaisePropertyChanged(nameof(ViewFont));
		}

		private void UpdateFontUI()
		{
			ReloadViewFont();

			AddFontCommand.RaiseCanExecute();
			RemoveFontCommand.RaiseCanExecute();
			IncreaseFontPriorityCommand.RaiseCanExecute();
			DecreaseFontPriorityCommand.RaiseCanExecute();
		}

		private void ApplyFontSetting()
		{
			Setting.TimelineFont = ViewFonts.Select(f => f.Source).ToArray();
			Setting.TimelineFontSize = ViewFontSize.Value;
			Setting.TimelineFontRendering = ViewFontRendering;
		}

		#region Command: AddFontCommand

		private Command<FontFamily> _addFontCommand;
		public Command AddFontCommand
		{
			get => _addFontCommand ?? (_addFontCommand = RegisterReleasableCommand<FontFamily>(AddNewFont, CanAddFont));
		}

		private void AddNewFont(FontFamily fontFamily)
		{
			ViewFonts.Insert(0, fontFamily);
			UpdateFontUI();
		}

		private bool CanAddFont(FontFamily fontFamily) => fontFamily != null;

		#endregion Command: AddFontCommand

		#region Command: FontSelectCommand

		private Command _fontSeelectCommand;
		public Command FontSelectCommand
		{
			get => _fontSeelectCommand ?? (_fontSeelectCommand = RegisterReleasableCommand(SelectFontFile));
		}

		private static bool SupportedFont(FontFamily font)
		{
			return font.FamilyTypefaces.Any(face => face.Style == FontStyles.Normal);
		}

		private void SelectFontFile()
		{
			var ofd = new OpenFileDialog
			{
				Filter = "フォントファイル|*.ttf;*.otf",
				CheckFileExists = true,
				CheckPathExists = true,
			};

			if (DialogService.OpenModal(ofd))
			{
				try
				{
					var fontFamily = Fonts.GetFontFamilies(ofd.FileName)
						.FirstOrDefault(SupportedFont);
					if (fontFamily != null)
					{
						ViewFonts.Insert(0, fontFamily);
					}
				}
				catch (Exception ex)
				{

				}
			}
		}

		#endregion Command: FontSelectCommand

		#region Command: RemoveFontCommand

		private Command<FontFamily> _removeFontCommand;
		public Command<FontFamily> RemoveFontCommand
		{
			get => _removeFontCommand ?? (_removeFontCommand = RegisterReleasableCommand<FontFamily>(RemoveFont, IsAvailableFont));
		}

		public void RemoveFont(FontFamily fontFamily)
		{
			ViewFonts.Remove(fontFamily);
			UpdateFontUI();
		}

		private static bool IsAvailableFont(FontFamily fontFamily) => fontFamily != null;

		#endregion Command: RemoveFontCommand

		#region Command: IncreaseFontPriorityCommand

		private Command<FontFamily> _increaseFontPriorityCommand;
		public Command<FontFamily> IncreaseFontPriorityCommand
		{
			get => _increaseFontPriorityCommand ?? (_increaseFontPriorityCommand = RegisterReleasableCommand<FontFamily>(IncreaseFontPriority, CanIncreaseFontPriority));
		}

		private void IncreaseFontPriority(FontFamily obj)
		{
			ViewFonts.ItemIndexDecrement(obj);
			UpdateFontUI();
		}

		private bool CanIncreaseFontPriority(FontFamily obj)
		{
			return obj != null && MathEx.IsWithin(Math.Min(ViewFonts.Count, ViewFonts.IndexOf(obj)), 1, ViewFonts.Count);
		}

		#endregion Command: IncreaseFontPriorityCommand

		#region Command: DecreaseFontPriorityCommand

		private Command<FontFamily> _decreaseFontPriorityCommand;
		public Command<FontFamily> DecreaseFontPriorityCommand
		{
			get => _decreaseFontPriorityCommand ?? (_decreaseFontPriorityCommand = RegisterReleasableCommand<FontFamily>(DecreaseFontPriority, CanDecreaseFontPriority));
		}

		private void DecreaseFontPriority(FontFamily obj)
		{
			ViewFonts.ItemIndexIncrement(obj);
			UpdateFontUI();
		}

		private bool CanDecreaseFontPriority(FontFamily obj)
		{
			return obj != null && Math.Max(0, ViewFonts.IndexOf(obj)) < ViewFonts.Count - 1;
		}

		#endregion Command: DecreaseFontPriorityCommand

		#region Command: ResetFontCommand

		private Command _resetFontSettingsCommand;
		public Command ResetFontSettingsCommand
		{
			get => _resetFontSettingsCommand ?? (_resetFontSettingsCommand = RegisterReleasableCommand(ResetFontSettings));
		}

		private void ResetFontSettings()
		{
			Setting.TimelineFont = Defines.DefaultTimelineFont;
			Setting.TimelineFontSize = Defines.DefaultTimelineFontSize;

			SetFontSettings();

			ReloadViewFont();
		}

		#endregion

		#endregion

		#region Accounts

		private ColumnType _tempColumnType;
		public ColumnType TempColumnType
		{
			get => _tempColumnType;
			set => SetProperty(ref _tempColumnType, value);
		}

		#region Commands for account

		#region Command: AccountAddCommand

		private Command _accountAddCommand;
		public Command AccountAddCommand
		{
			get => _accountAddCommand ?? (_accountAddCommand = RegisterReleasableCommand(AccountAdd));
		}

		private async void AccountAdd()
		{
			var auth = new AuthenticationViewModel();

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

				var account = Accounts.FirstOrDefault((a) => a.Id == tokens.UserId);

				if (account != null)
				{
					account.SetTokens(tokens);
				}
				else
				{
					account = new Account(tokens)
					{
						AutomaticallyLogin = Setting.AccountDefaultAutomaticallyLogin,
					};

					Accounts.Add(account);

					if(account.AutomaticallyLogin)
					{
						foreach(var cols in DefaultColumns)
						{
							App.Columns.Add(cols.ToColumn(account));
						}
					}

					account.IsLoading = true;

					await Task.Run(() =>
					{
						if (!account.Login())
						{
							DialogService.MessageBox(
								$"アカウント情報の取得に失敗しました:\n",
								MsgBoxButtons.Ok, MsgBoxIcon.Information);

							Accounts.Remove(account);
						}
					});

					account.IsLoading = false;
				}
			}
		}

		#endregion

		#region Command: AccountDeleteCommand

		private Command _accountDeleteCommand;
		public Command AccountDeleteCommand
		{
			get => _accountDeleteCommand ?? (_accountDeleteCommand = RegisterReleasableCommand<Account>(AccountDelete, Accounts.Contains));
		}

		void AccountDelete(Account account)
		{
			if (DialogService.ShowQuestion(
				$"本当にアカウントを一覧から削除しますか？\n{account.Name}@{account.ScreenName}"))
			{
				Accounts.Remove(account);
				account.Unload();
			}
		}

		#endregion

		#region Command: AccountMoveUpCommand

		private Command _accountMoveUpCommand;
		public Command AccountMoveUpCommand
		{
			get => _accountMoveUpCommand ?? (_accountMoveUpCommand = RegisterReleasableCommand<Account>(AccountMoveUp, CanAccountMoveUp));
		}

		private bool CanAccountMoveUp(Account account) => Accounts.CanItemIndexDecrement(account);

		private void AccountMoveUp(Account account) => Accounts.ItemIndexDecrement(account);

		#endregion

		#region Command: AccountMoveDownCommand

		private Command _accountMoveDownCommand;
		public Command AccountMoveDownCommand
		{
			get => _accountMoveDownCommand ?? (_accountMoveDownCommand = RegisterReleasableCommand<Account>(AccountMoveDown, CanAccountMoveDown));
		}

		private bool CanAccountMoveDown(Account account) => Accounts.CanItemIndexIncrement(account);

		private void AccountMoveDown(Account account) => Accounts.ItemIndexIncrement(account);

		#endregion

		#endregion Commands for account

		public FluidCollection<ColumnSetting> DefaultColumns => Setting.DefaultColumns;

		#region Commands for columns

		private ColumnSetting _selectedColumnSetting;
		public ColumnSetting SelectedColumnSetting
		{
			get => _selectedColumnSetting;
			set
			{
				if (SetProperty(ref _selectedColumnSetting, value))
				{
					ColumnMoveUpCommand.RaiseCanExecute();
					ColumnMoveDownCommand.RaiseCanExecute();
					ColumnRemoveCommand.RaiseCanExecute();
				}
			}
		}

		#region Command: ColumnAddCommand

		private Command<ColumnType> _columnAddCommand;
		public Command<ColumnType> ColumnAddCommand
		{
			get => _columnAddCommand ?? (_columnAddCommand = RegisterReleasableCommand<ColumnType>(ColumnAdd));
		}

		private void ColumnAdd(ColumnType type) => DefaultColumns.Add(new ColumnSetting(type, Account.Dummy));

		#endregion

		#region Command: ColumnRemoveCommand

		private Command _columnRemoveCommand;
		public Command ColumnRemoveCommand
		{
			get => _columnRemoveCommand ?? (_columnRemoveCommand = RegisterReleasableCommand<ColumnSetting>(ColumnRemove, CanColumnRemove));
		}

		private static bool CanColumnRemove(ColumnSetting column) => column != null;

		private void ColumnRemove(ColumnSetting column) => DefaultColumns.Remove(column);

		#endregion

		#region Command: ColumnMoveUpCommand

		private Command _columnMoveUpCommand;
		public Command ColumnMoveUpCommand
		{
			get => _columnMoveUpCommand ?? (_columnMoveUpCommand = RegisterReleasableCommand<ColumnSetting>(ColumnMoveUp, CanColumnMoveUp));
		}

		private bool CanColumnMoveUp(ColumnSetting column)
		{
			return column != null && DefaultColumns.CanItemIndexDecrement(column);
		}

		private void ColumnMoveUp(ColumnSetting column)
		{
			DefaultColumns.ItemIndexDecrement(column);
			ColumnMoveUpCommand.RaiseCanExecute();
			ColumnMoveDownCommand.RaiseCanExecute();
		}

		#endregion

		#region Command: ColumnMoveDownCommand

		private Command _columnMoveDownCommand;
		public Command ColumnMoveDownCommand
		{
			get => _columnMoveDownCommand ?? (_columnMoveDownCommand = RegisterReleasableCommand<ColumnSetting>(ColumnMoveRight, CanColumnMoveRight));
		}

		private bool CanColumnMoveRight(ColumnSetting column)
		{
			return column != null && DefaultColumns.CanItemIndexIncrement(column);
		}

		private void ColumnMoveRight(ColumnSetting column)
		{
			DefaultColumns.ItemIndexIncrement(column);
			ColumnMoveDownCommand?.RaiseCanExecute();
			ColumnMoveUpCommand.RaiseCanExecute();
		}

		#endregion

		#endregion Commands for columns

		#endregion Accounts

		#region Formats

		public TextBoxController NowPlayingTextBoxController { get; } = new TextBoxController();

		public string NowPlayingFormat
		{
			get => Setting.NowPlayingFormat;
			set
			{
				Setting.NowPlayingFormat = value;
				RaisePropertyChanged(nameof(NowPlayingFormat));
			}
		}

		#region InsertNowPlayingCommand

		private Command _insertNowPlayingCommand;
		public Command InsertNowPlayingParamCommand
		{
			get => _insertNowPlayingCommand ?? (_insertNowPlayingCommand = RegisterReleasableCommand<string>(InsertNowPlaying));
		}

		private void InsertNowPlaying(string p)
		{
			NowPlayingTextBoxController.Insert(p);
			NowPlayingTextBoxController.Focus();
		}

		#endregion

		#region ResetNowPlayingCommand

		private Command _resetNowPlayingCommand;
		public Command ResetNowPlayingCommand
		{
			get => _resetNowPlayingCommand ?? (_resetNowPlayingCommand = RegisterReleasableCommand(ResetNowPlaying));
		}

		private void ResetNowPlaying() => NowPlayingFormat = Defines.DefaultNowPlayingFormat;

		#endregion

		#endregion

		#region Mute

		public FluidCollection<Mute> MuteList => App.Setting.Mute;

		private MuteType _tempMuteType;
		public MuteType TempMuteType
		{
			get => _tempMuteType;
			set => SetProperty(ref _tempMuteType, value, _muteAddCommand);
		}

		private SearchMode _tempMuteSearch;
		public SearchMode TempMuteSearch
		{
			get => _tempMuteSearch;
			set => SetProperty(ref _tempMuteSearch, value, _muteAddCommand);
		}

		private string _tempMuteText;
		public string TempMuteText
		{
			get => _tempMuteText;
			set => SetProperty(ref _tempMuteText, value, _muteAddCommand);
		}

		private Mute _selectedMute;
		public Mute SelectedMute
		{
			get => _selectedMute;
			set
			{
				if(SetProperty(ref _selectedMute, value))
				{
					MuteRemoveCommand.RaiseCanExecute();
				}
			}
		}

		#region Command: AddMuteCommand

		private Command _muteAddCommand;
		public Command MuteAddCommand
		{
			get => _muteAddCommand ?? (_muteAddCommand = RegisterReleasableCommand(MuteAdd, CanAddMute));
		}

		private void MuteAdd()
		{
			var mute = new Mute(_tempMuteType, _tempMuteSearch, _tempMuteText);
			MuteList.Add(mute);
			SelectedMute = mute;
		}

		private bool CanAddMute(object _) => Mute.IsAvailable(_tempMuteType, _tempMuteSearch, _tempMuteText);

		#endregion Command: AddMuteCommand

		#region Command: RemoveMuteCommand

		private Command _removeMuteCommand;
		public Command RemoveMuteCommand
		{
			get => _removeMuteCommand ?? (_removeMuteCommand = RegisterReleasableCommand<Mute>(RemoveMute, IsAvailableMuteItem));
		}

		private void RemoveMute(Mute mute) => MuteList.Remove(mute);

		private static bool IsAvailableMuteItem(Mute mute) => mute != null;

		#endregion Command: RemoveMuteCommand

		#region MuteRemoveCommand

		private Command<Mute> _muteRemoveCommand;
		public Command<Mute> MuteRemoveCommand
		{
			get => _muteRemoveCommand ?? (_muteRemoveCommand = RegisterReleasableCommand<Mute>(MuteRemove, CanMuteRemove));
		}

		private void MuteRemove(Mute item) => MuteList.Remove(item);

		private bool CanMuteRemove(Mute item) => MuteList.Contains(item);

		#endregion

		#endregion

		#region Timeline

		public bool TimelineStatusShowRelativeTime
		{
			get => Setting.TimelineStatusShowRelativeTime;
			set
			{
				Setting.TimelineStatusShowRelativeTime = value;
				RaisePropertyChanged(nameof(TimelineStatusShowAbsoluteTime));
				RaisePropertyChanged(nameof(TimelineStatusShowRelativeTime));
			}
		}

		public bool TimelineStatusShowAbsoluteTime
		{
			get => !Setting.TimelineStatusShowRelativeTime;
			set
			{
				Setting.TimelineStatusShowRelativeTime = !value;
				RaisePropertyChanged(nameof(TimelineStatusShowAbsoluteTime));
				RaisePropertyChanged(nameof(TimelineStatusShowRelativeTime));
			}
		}


		public bool TimelineStatusShowRelativeTimeDetail
		{
			get => Setting.TimelineStatusDetailShowRelativeTime;
			set
			{
				Setting.TimelineStatusDetailShowRelativeTime = value;
				RaisePropertyChanged(nameof(TimelineStatusShowAbsoluteTimeDetail));
				RaisePropertyChanged(nameof(TimelineStatusShowRelativeTimeDetail));
			}
		}

		public bool TimelineStatusShowAbsoluteTimeDetail
		{
			get => !Setting.TimelineStatusDetailShowRelativeTime;
			set
			{
				Setting.TimelineStatusDetailShowRelativeTime = !value;
				RaisePropertyChanged(nameof(TimelineStatusShowAbsoluteTimeDetail));
				RaisePropertyChanged(nameof(TimelineStatusShowRelativeTimeDetail));
			}
		}
		#endregion

		internal override bool CanClose()
		{
			if (Accounts.Count == 0)
			{
				return DialogService.MessageBox(
					"アカウントが登録されていません。終了しますか？", "Liberfy",
					MsgBoxButtons.YesNo, MsgBoxIcon.Question) == MsgBoxResult.Yes;
			}

			if (ViewFonts.Count == 0)
			{
				return DialogService.MessageBox(
					"フォントが指定されていません。続行しますか？",
					MsgBoxButtons.YesNo, MsgBoxIcon.Question) == MsgBoxResult.Yes;
			}

			return true;
		}

		internal override void OnClosed()
		{
			ApplyFontSetting();
			App.LoadUISettingsFromSetting();
		}
	}
}
