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
using Liberfy.Settings;
using SocialApis;

namespace Liberfy.ViewModel
{
    internal partial class SettingWindow : ViewModelBase
    {
        public SettingWindow() : base()
        {
            ViewFonts = new FluidCollection<FontFamily>();
            SetFontSettings();

            this.DefaultColumns = new FluidCollection<ColumnBase>(Setting.DefaultColumns
                .Select(opt => ColumnBase.FromSetting(opt, null, out var column) ? column : ColumnBase.FromType(opt.Type)));
        }

        public Setting Setting => App.Setting;

        public IEnumerable<AccountBase> Accounts { get; } = AccountManager.Accounts;

        /*
         * [表示]タブ関連 → SettingWindowViewModel.View.cs
         * 
         * 
         */

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

                return reg.GetValue(App.AppName) is string regKeyValue
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
                            reg.DeleteValue(App.AppName);
                        }
                        else
                        {
                            reg.SetValue(App.AppName, $"\"{App.Assembly.Location}\" /startup");
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

            this.ViewFonts.Clear();
            foreach (var f in Setting.TimelineFont)
                this.ViewFonts.Add(new FontFamily(f));

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

        //private static readonly StringComparison strComp = StringComparison.CurrentCultureIgnoreCase;

        //private void FilteringFontFontList()
        //{
        //    if (string.IsNullOrWhiteSpace(_fontFontFilter))
        //    {
        //        _fontFontList = new List<FontFamily>(_baseFontList);
        //    }
        //    else
        //    {
        //        bool isFontFiltering(FontFamily fontFamily)
        //        {
        //            return fontFamily.Source.Contains(_fontFontFilter, strComp)
        //                || fontFamily.FamilyNames.Any(kvp => kvp.Value.Contains(_fontFontFilter, strComp));
        //        }

        //        _fontFontList = new List<FontFamily>(_baseFontList.Where(isFontFiltering));
        //    }

        //    RaisePropertyChanged(nameof(FontFontList));
        //}

        private FontFamily _selectedFont;
        public FontFamily SelectedFont
        {
            get => _selectedFont;
            set
            {
                if (SetProperty(ref _selectedFont, value))
                {
                    this.UpdateFontUI();
                }
            }
        }

        public FluidCollection<FontFamily> ViewFonts { get; }

        private void ReloadViewFont()
        {
            this.ViewFont = new FontFamily(string.Join(", ", ViewFonts.Select(f => f.Source)));
            this.RaisePropertyChanged(nameof(ViewFont));
        }

        private void UpdateFontUI()
        {
            this.ReloadViewFont();

            this.AddFontCommand.RaiseCanExecute();
            this.RemoveFontCommand.RaiseCanExecute();
            this.IncreaseFontPriorityCommand.RaiseCanExecute();
            this.DecreaseFontPriorityCommand.RaiseCanExecute();
        }

        private void ApplyFontSetting()
        {
            Setting.TimelineFont = this.ViewFonts.Select(f => f.Source).ToArray();
            Setting.TimelineFontSize = this.ViewFontSize.Value;
            Setting.TimelineFontRendering = this.ViewFontRendering;
        }

        private Command _addFontCommand;
        public Command AddFontCommand => this._addFontCommand ?? (this._addFontCommand = this.RegisterCommand(() =>
        {
            var option = new SelectDialogOption<FontFamily>
            {
                Instruction = "追加するフォントを選択してください",
                Items = Fonts.SystemFontFamilies,
                ItemTemplate = App.Current.TryFindResource("FontViewTemplate") as DataTemplate,
            };

            this.DialogService.SelectDialog(option);

            if (option.IsSelected)
            {
                this.ViewFonts.Insert(0, option.SelectedItem);
                this.UpdateFontUI();
            }
        }));

        private Command _selectExternalFontCommand;
        public Command SelectExternalFontCommand => this._selectExternalFontCommand ?? (this._selectExternalFontCommand = this.RegisterCommand(() =>
        {
            var ofd = new OpenFileDialog
            {
                Filter = "フォントファイル|*.ttf;*.otf",
                CheckFileExists = true,
                CheckPathExists = true,
            };

            if (this.DialogService.OpenModal(ofd))
            {
                try
                {
                    var fontFamily = Fonts
                        .GetFontFamilies(ofd.FileName)
                        .FirstOrDefault(SupportedFont);

                    if (fontFamily != null)
                    {
                        this.ViewFonts.Insert(0, fontFamily);
                    }

                    this.UpdateFontUI();
                }
                catch { /* pass */ }
            }
        }));

        private static bool SupportedFont(FontFamily font)
        {
            return font.FamilyTypefaces.Any(face => face.Style == FontStyles.Normal);
        }

        #region Command: RemoveFontCommand

        private Command<FontFamily> _removeFontCommand;
        public Command<FontFamily> RemoveFontCommand
        {
            get => _removeFontCommand ?? (_removeFontCommand = RegisterCommand<FontFamily>(RemoveFont, IsAvailableFont));
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
            get => _increaseFontPriorityCommand ?? (_increaseFontPriorityCommand = RegisterCommand<FontFamily>(IncreaseFontPriority, CanIncreaseFontPriority));
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
            get => _decreaseFontPriorityCommand ?? (_decreaseFontPriorityCommand = RegisterCommand<FontFamily>(DecreaseFontPriority, CanDecreaseFontPriority));
        }

        private void DecreaseFontPriority(FontFamily obj)
        {
            this.ViewFonts.ItemIndexIncrement(obj);
            this.UpdateFontUI();
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
            get => _resetFontSettingsCommand ?? (_resetFontSettingsCommand = RegisterCommand(ResetFontSettings));
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

        private AccountBase _selectedAccount;
        public AccountBase SelectedAccount
        {
            get => this._selectedAccount;
            set
            {
                if (this.SetProperty(ref this._selectedAccount, value))
                {
                    this.RaiseCanExecuteAccountCommands();
                }
            }
        }

        #region Commands for account

        #region Command: AccountAddCommand

        private Command _accountAddCommand;
        public Command AccountAddCommand => this._accountAddCommand ?? (this._accountAddCommand = this.RegisterCommand(this.AccountAdd));

        private async void AccountAdd()
        {
            var auth = new AuthenticationViewModel();

            if (DialogService.OpenModal(auth, new ViewOption
            {
                ResizeMode = ResizeMode.NoResize,
                StartupLocation = WindowStartupLocation.CenterOwner,
                SizeToContent = SizeToContent.Manual,
                Width = 400,
                Height = 320,
                WindowChrome = new System.Windows.Shell.WindowChrome
                {
                    GlassFrameThickness = new Thickness(0),
                    UseAeroCaptionButtons = false,
                    CornerRadius = new CornerRadius(0),
                    CaptionHeight = 0,
                },
                ShowInTaskbar = false,
            }, typeof(AuthenticationViewModel)))
            {
                if (auth.SelectedService == SocialService.Twitter)
                {
                    var tokens = auth.TwitterTokens;

                    var account = AccountManager.Get(SocialService.Twitter, tokens.UserId);

                    if (account != null)
                    {
                        account.SetTokens(ApiTokenInfo.FromTokens(tokens));
                    }
                    else
                    {
                        var user = await tokens.Account.VerifyCredentials();

                        account = new TwitterAccount(tokens, user)
                        {
                            AutomaticallyLogin = this.Setting.AccountDefaultAutomaticallyLogin,
                            AutomaticallyLoadTimeline = this.Setting.AccountDefaultAutomaticallyLoadTimeline
                        };

                        AccountManager.Add(account);

                        foreach (var columnOptions in this.DefaultColumns.Select(c => c.GetOption()))
                        {
                            if (ColumnBase.FromSetting(columnOptions, account, out var column))
                            {
                                TimelineBase.Columns.Add(column);
                            }
                        }

                        if (!await account.TryLogin())
                        {
                            this.DialogService.MessageBox(
                                $"アカウント情報の取得に失敗しました:\n",
                                MsgBoxButtons.Ok, MsgBoxIcon.Information);

                            AccountManager.Remove(account);
                        }
                    }
                }
                else if (auth.SelectedService == SocialService.Mastodon)
                {
                    var tokens = auth.MastodonTokens;

                    var user = await tokens.Accounts.VerifyCredentials();

                    var account = new MastodonAccount(tokens, user)
                    {
                        AutomaticallyLogin = this.Setting.AccountDefaultAutomaticallyLogin,
                        AutomaticallyLoadTimeline = this.Setting.AccountDefaultAutomaticallyLoadTimeline
                    };

                    AccountManager.Add(account);

                    foreach (var columnOptions in this.DefaultColumns.Select(c => c.GetOption()))
                    {
                        if (ColumnBase.FromSetting(columnOptions, account, out var column))
                        {
                            TimelineBase.Columns.Add(column);
                        }
                    }

                    if (!await account.TryLogin())
                    {
                        this.DialogService.MessageBox(
                            $"アカウント情報の取得に失敗しました:\n",
                            MsgBoxButtons.Ok, MsgBoxIcon.Information);

                        AccountManager.Remove(account);
                    }
                }
            }
        }

        #endregion

        private void RaiseCanExecuteAccountCommands()
        {
            this._accountMoveUpCommand?.RaiseCanExecute();
            this._accountMoveDownCommand?.RaiseCanExecute();
            this._accountDeleteCommand?.RaiseCanExecute();
        }

        private Command _accountDeleteCommand;
        public Command AccountDeleteCommand => this._accountDeleteCommand ?? (this._accountDeleteCommand = this.RegisterCommand(
            DelegateCommand<AccountBase>
            .When(a => AccountManager.Contains(a))
            .Exec(a =>
            {
                if (this.DialogService.ShowQuestion(
                    $"本当にアカウントを一覧から削除しますか？\n { a.Info.Name }@{ a.Info.ScreenName }"))
                {
                    AccountManager.Remove(a);
                    a.Unload();
                }

                this.RaiseCanExecuteAccountCommands();
            })));

        private Command _accountMoveUpCommand;
        public Command AccountMoveUpCommand => this._accountMoveUpCommand ?? (this._accountMoveUpCommand = this.RegisterCommand(
            DelegateCommand<AccountBase>
            .When(a => AccountManager.IndexOf(a) > 1)
            .Exec(a =>
            {
                int index = AccountManager.IndexOf(a);
                AccountManager.Move(index, index - 1);

                this.RaiseCanExecuteAccountCommands();
            })));

        private Command _accountMoveDownCommand;
        public Command AccountMoveDownCommand => this._accountMoveDownCommand = (this._accountMoveDownCommand = this.RegisterCommand(
            DelegateCommand<AccountBase>
            .When(a => AccountManager.IndexOf(a) < AccountManager.Count - 1)
            .Exec(a =>
            {
                int index = AccountManager.IndexOf(a);
                AccountManager.Move(index, index + 1);

                this.RaiseCanExecuteAccountCommands();
            })));

        #endregion Commands for account


        public FluidCollection<ColumnBase> DefaultColumns { get; }

        #region Commands for columns

        private ColumnBase _selectedColumnSetting;
        public ColumnBase SelectedColumnSetting
        {
            get => this._selectedColumnSetting;
            set
            {
                if (this.SetProperty(ref _selectedColumnSetting, value))
                {
                    this.ColumnMoveUpCommand.RaiseCanExecute();
                    this.ColumnMoveDownCommand.RaiseCanExecute();
                    this.ColumnRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: ColumnAddCommand

        private Command _columnAddCommand;
        public Command ColumnAddCommand => this._columnAddCommand ?? (this._columnAddCommand = this.RegisterCommand(() =>
        {
            var res = this.DialogService.SelectDialog(new SelectDialogOption<KeyValuePair<object, string>>
            {
                Items = ColumnBase.ColumnTypes,
                SelectedValuePath = "Key",
                DisplayMemberPath = "Value",
            });

            if (res.IsSelected && res.SelectedValue is ColumnType columnType)
            {
                this.DefaultColumns.Add(ColumnBase.FromType(columnType));
            }
        }));

        #endregion

        #region Command: ColumnRemoveCommand

        private Command _columnRemoveCommand;
        public Command ColumnRemoveCommand
        {
            get => _columnRemoveCommand ?? (_columnRemoveCommand = RegisterCommand<ColumnBase>(ColumnRemove, CanColumnRemove));
        }

        private static bool CanColumnRemove(ColumnBase column) => column != null;

        private void ColumnRemove(ColumnBase column) => this.DefaultColumns.Remove(column);

        #endregion

        #region Command: ColumnMoveUpCommand

        private Command _columnMoveUpCommand;
        public Command ColumnMoveUpCommand
        {
            get => this._columnMoveUpCommand ?? (this._columnMoveUpCommand = this.RegisterCommand<ColumnBase>(this.ColumnMoveUp, this.CanColumnMoveUp));
        }

        private bool CanColumnMoveUp(ColumnBase column)
        {
            return column != null && DefaultColumns.CanItemIndexDecrement(column);
        }

        private void ColumnMoveUp(ColumnBase column)
        {
            this.DefaultColumns.ItemIndexDecrement(column);
            this.ColumnMoveUpCommand.RaiseCanExecute();
            this.ColumnMoveDownCommand.RaiseCanExecute();
        }

        #endregion

        #region Command: ColumnMoveDownCommand

        private Command _columnMoveDownCommand;
        public Command ColumnMoveDownCommand
        {
            get => this._columnMoveDownCommand ?? (this._columnMoveDownCommand = this.RegisterCommand<ColumnBase>(this.ColumnMoveRight, this.CanColumnMoveRight));
        }

        private bool CanColumnMoveRight(ColumnBase column)
        {
            return column != null && DefaultColumns.CanItemIndexIncrement(column);
        }

        private void ColumnMoveRight(ColumnBase column)
        {
            this.DefaultColumns.ItemIndexIncrement(column);
            this.ColumnMoveDownCommand?.RaiseCanExecute();
            this.ColumnMoveUpCommand.RaiseCanExecute();
        }

        #endregion

        #endregion Commands for columns

        #endregion Accounts

        #region Formats

        public TextBoxController NowPlayingTextBoxController { get; } = new TextBoxController();

        public string NowPlayingFormat
        {
            get => this.Setting.NowPlayingFormat;
            set
            {
                this.Setting.NowPlayingFormat = value;
                this.RaisePropertyChanged(nameof(NowPlayingFormat));
            }
        }

        #region InsertNowPlayingCommand

        public string SelectedNowPlayingParameter
        {
            get => null;
            set
            {
                this.NowPlayingTextBoxController.Insert(value);
                this.NowPlayingTextBoxController.Focus();

                this.RaisePropertyChanged(nameof(this.SelectedNowPlayingParameter));
            }
        }

        private Command _insertNowPlayingCommand;
        public Command InsertNowPlayingParamCommand
        {
            get => _insertNowPlayingCommand ?? (_insertNowPlayingCommand = RegisterCommand<string>(InsertNowPlaying));
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
            get => _resetNowPlayingCommand ?? (_resetNowPlayingCommand = RegisterCommand(ResetNowPlaying));
        }

        private void ResetNowPlaying() => NowPlayingFormat = Defines.DefaultNowPlayingFormat;

        #endregion

        #endregion

        #region Mute

        public FluidCollection<Mute> MuteList { get; } = App.Setting.Mute;

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
                if (SetProperty(ref _selectedMute, value))
                {
                    MuteRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: AddMuteCommand

        private Command _muteAddCommand;
        public Command MuteAddCommand
        {
            get => _muteAddCommand ?? (_muteAddCommand = RegisterCommand(MuteAdd, CanAddMute));
        }

        private void MuteAdd()
        {
            if (Mute.Create(_tempMuteType, _tempMuteSearch, _tempMuteText, out var m))
            {
                MuteList.Add(m);
                SelectedMute = m;
            }
        }

        private bool CanAddMute() => Mute.IsAvailable(_tempMuteType, _tempMuteSearch, _tempMuteText);

        #endregion Command: AddMuteCommand

        #region Command: RemoveMuteCommand

        private Command _removeMuteCommand;
        public Command RemoveMuteCommand
        {
            get => _removeMuteCommand ?? (_removeMuteCommand = RegisterCommand<Mute>(RemoveMute, IsAvailableMuteItem));
        }

        private void RemoveMute(Mute mute) => MuteList.Remove(mute);

        private static bool IsAvailableMuteItem(Mute mute) => mute != null;

        #endregion Command: RemoveMuteCommand

        #region MuteRemoveCommand

        private Command<Mute> _muteRemoveCommand;
        public Command<Mute> MuteRemoveCommand
        {
            get => _muteRemoveCommand ?? (_muteRemoveCommand = RegisterCommand<Mute>(MuteRemove, CanMuteRemove));
        }

        private void MuteRemove(Mute item) => MuteList.Remove(item);

        private bool CanMuteRemove(Mute item) => MuteList.Contains(item);

        #endregion

        #endregion

        #region Timeline settings

        private const string SampleRelativeTime = "1分前";
        private const string SampleAbsoluteTime = "2018年1月1日 0時0分";

        public string TimelineStatusTimeText
        {
            get
            {
                if (this.Setting.TimelineStatusShowRelativeTime)
                    return SampleRelativeTime;
                else
                    return SampleAbsoluteTime;
            }
        }

        public bool IsShowRelativeTimeTimelineStatus
        {
            get => this.Setting.TimelineStatusShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusShowRelativeTime = value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.TimelineStatusTimeText));
            }
        }

        public bool IsShowAbsoluteTimeTimelineStatus
        {
            get => !this.Setting.TimelineStatusShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusShowRelativeTime = !value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeTimelineStatus),
                    nameof(this.IsShowAbsoluteTimeTimelineStatus),
                    nameof(this.TimelineStatusTimeText));
            }
        }

        public string DetailStatusTimeText
        {
            get
            {
                if (this.Setting.TimelineStatusDetailShowRelativeTime)
                    return SampleRelativeTime;
                else
                    return SampleAbsoluteTime;
            }
        }

        public bool IsShowRelativeTimeDetailStatus
        {
            get => this.Setting.TimelineStatusDetailShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusDetailShowRelativeTime = value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.DetailStatusTimeText));
            }
        }

        public bool IsShowAbsoluteTimeDetailStatus
        {
            get => !this.Setting.TimelineStatusDetailShowRelativeTime;
            set
            {
                this.Setting.TimelineStatusDetailShowRelativeTime = !value;
                this.RaisePropertiesChanged(
                    nameof(this.IsShowAbsoluteTimeDetailStatus),
                    nameof(this.IsShowRelativeTimeDetailStatus),
                    nameof(this.DetailStatusTimeText));
            }
        }

        #endregion Timeline settings



        internal override bool CanClose()
        {
            if (AccountManager.Count == 0)
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
            this.Setting.DefaultColumns.Reset(
                this.DefaultColumns.Select(c => c.GetOption()));

            ApplyFontSetting();
            App.UI.ApplyFromSettings();
        }
    }
}
