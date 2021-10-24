using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using Liberfy.Commands.SettingWindowCommands;
using WpfMvvmToolkit;
using Livet.Messaging;
using System.Windows.Input;
using Liberfy.Managers;

namespace Liberfy.ViewModels
{
    internal partial class SettingWindowViewModel : ViewModelBase
    {
        public SettingWindowViewModel() : base()
        {
        }

        public static Setting GlobalSetting { get; } = App.Setting;

        public Setting Setting { get; } = GlobalSetting;

        public IEnumerable<IAccount> Accounts { get; } = App.Accounts;

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
            using var reg = this.CreateAutoStartuRegSubKey();
            var path = Regex.Escape(App.AssemblyInfo.Location);

            return reg.GetValue(App.Name) is string regKeyValue
                && !string.IsNullOrEmpty(regKeyValue)
                && Regex.IsMatch(regKeyValue, $@"^(""{path}""|{path})(?<params>\s+.*)?$", RegexOptions.IgnoreCase);
        }

        public bool IsAccountPage { get; private set; }

        private int _tabPageIndex;
        public int TabPageIndex
        {
            get => this._tabPageIndex;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._tabPageIndex, value))
                {
                    this.IsAccountPage = value == 1;
                    this.RaisePropertyChanged(nameof(this.IsAccountPage));
                }
            }
        }

        public bool ShowInTaskTray
        {
            get => this.Setting.ShowInTaskTray;
            set
            {
                this.Setting.ShowInTaskTray = value;
                this.RaisePropertyChanged(nameof(this.ShowInTaskTray));
            }
        }

        public bool AutoStartup
        {
            get => this.GetAutoStartupEnabled();
            set
            {
                try
                {
                    bool isAutoStartupEnabled = this.GetAutoStartupEnabled();
                    if (object.Equals(isAutoStartupEnabled, value)) return;

                    using var reg = this.CreateAutoStartuRegSubKey();

                    if (isAutoStartupEnabled)
                    {
                        reg.DeleteValue(App.Name);
                    }
                    else
                    {
                        reg.SetValue(App.Name, $"\"{App.AssemblyInfo.Location}\" /startup");
                    }

                }
                catch (Exception ex)
                {
                    this.Messenger.Raise(new InformationMessage("自動起動の設定に失敗しました: " + ex.Message, App.Name, MessageBoxImage.Information, "MsgKey_FailSetStartup"));
                }

                this.RaisePropertyChanged(nameof(this.AutoStartup));
            }
        }

        public bool ShowInTaskTrayAtMinimized
        {
            get => this.Setting.ShowInTaskTrayAtMinimzied;
            set
            {
                this.Setting.ShowInTaskTrayAtMinimzied = value;
                this.RaisePropertyChanged(nameof(this.ShowInTaskTrayAtMinimized));
            }
        }

        #region General

        public IReadOnlyDictionary<TextFormattingMode, string> TextRenderingModeList { get; } = new Dictionary<TextFormattingMode, string>
        {
            [TextFormattingMode.Ideal] = "標準",
            [TextFormattingMode.Display] = "GDI互換",
        };

        /// <summary>
        /// フォントサイズ
        /// </summary>
        private double? _timelineFontSize = GlobalSetting.TimelineFontSize;

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public double? TimelineFontSize
        {
            get => this._timelineFontSize;
            set => this.RaisePropertyChangedIfSet(ref this._timelineFontSize, value);
        }

        /// <summary>
        /// フォント
        /// </summary>
        private string _timelineFont = GlobalSetting.TimelineFont;

        /// <summary>
        /// フォント
        /// </summary>
        public string TimelineFont
        {
            get => this._timelineFont;
            set => this.RaisePropertyChangedIfSet(ref this._timelineFont, value);
        }

        /// <summary>
        /// フォントのレンダリングモード
        /// </summary>
        private TextFormattingMode _timelineFontRenderingMode = GlobalSetting.TimelineFontRendering;

        /// <summary>
        /// フォントのレンダリングモード
        /// </summary>
        public TextFormattingMode TimelineFontRenderingMode
        {
            get => this._timelineFontRenderingMode;
            set => this.RaisePropertyChangedIfSet(ref this._timelineFontRenderingMode, value);
        }

        public FontFamily _selectedFontFamily;
        public FontFamily SelectedFontFamily
        {
            get => this._selectedFontFamily;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._selectedFontFamily, value))
                {
                    if (value != null)
                    {
                        if (this.TimelineFont.EndsWith("\n"))
                        {
                            this.TimelineFont += value.Source;
                        }
                        else
                        {
                            this.TimelineFont += "\n" + value.Source;
                        }

                        this.SelectedFontFamily = null;
                    }
                }
            }
        }

        #endregion

        #region Accounts

        private IAccount _selectedAccount;
        public IAccount SelectedAccount
        {
            get => this._selectedAccount;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._selectedAccount, value))
                {
                    this.RaiseCanExecuteAccountCommands();
                }
            }
        }

        #region Commands for account

        internal void RaiseCanExecuteAccountCommands()
        {
            this._accountDeleteCommand?.RaiseCanExecute();
        }

        private Command<IAccount> _accountDeleteCommand;
        public Command<IAccount> AccountDeleteCommand => this._accountDeleteCommand ??= this.RegisterCommand(new AccountDeleteCommand(this));

        #endregion Commands for account


        public NotifiableCollection<ColumnBase> DefaultColumns { get; }

        #region Commands for columns

        private ColumnBase _selectedColumnSetting;
        public ColumnBase SelectedColumnSetting
        {
            get => this._selectedColumnSetting;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref _selectedColumnSetting, value))
                {
                    this.ColumnMoveUpCommand.RaiseCanExecute();
                    this.ColumnMoveDownCommand.RaiseCanExecute();
                    this.ColumnRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: ColumnAddCommand

        private Command<ColumnType> _columnAddCommand;
        public Command<ColumnType> ColumnAddCommand => this._columnAddCommand ??= this.RegisterCommand(new ColumnAddCommand(this));

        #endregion

        #region Command: ColumnRemoveCommand

        private Command<ColumnBase> _columnRemoveCommand;
        public Command<ColumnBase> ColumnRemoveCommand
        {
            get => this._columnRemoveCommand ??= this.Commands.Create<ColumnBase>(this.ColumnRemove, CanColumnRemove);
        }

        private static bool CanColumnRemove(ColumnBase column) => column != null;

        private void ColumnRemove(ColumnBase column) => this.DefaultColumns.Remove(column);

        #endregion

        #region Command: ColumnMoveUpCommand

        private Command<ColumnBase> _columnMoveUpCommand;
        public Command<ColumnBase> ColumnMoveUpCommand => this._columnMoveUpCommand ??= this.Commands.Create<ColumnBase>(this.ColumnMoveUp, this.CanColumnMoveUp);

        private bool CanColumnMoveUp(ColumnBase column)
        {
            return column != null && this.DefaultColumns.CanItemIndexDecrement(column);
        }

        private void ColumnMoveUp(ColumnBase column)
        {
            this.DefaultColumns.ItemIndexDecrement(column);
            this.ColumnMoveUpCommand.RaiseCanExecute();
            this.ColumnMoveDownCommand.RaiseCanExecute();
        }

        #endregion

        #region Command: ColumnMoveDownCommand

        private Command<ColumnBase> _columnMoveDownCommand;
        public Command<ColumnBase> ColumnMoveDownCommand => this._columnMoveDownCommand ??= this.Commands.Create<ColumnBase>(this.ColumnMoveRight, this.CanColumnMoveRight);

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

        #region NowPlaying

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

        private Command<string> _insertNowPlayingCommand;
        public Command<string> InsertNowPlayingParamCommand => this._insertNowPlayingCommand ??= this.RegisterCommand(new InsertNowPlayingParameterCommand(this));

        private Command _resetNowPlayingCommand;
        public Command ResetNowPlayingCommand => this._resetNowPlayingCommand ??= this.RegisterCommand(new ResetNowPlayingFormatCommand(this));

        #endregion

        #region Mute

        public NotifiableCollection<Mute> MuteList { get; } = App.Setting.Mute;

        private MuteType _tempMuteType;
        public MuteType TempMuteType
        {
            get => _tempMuteType;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._tempMuteType, value))
                {
                    this._muteAddCommand.RaiseCanExecute();
                }
            }
        }

        private SearchMode _tempMuteSearch;
        public SearchMode TempMuteSearch
        {
            get => this._tempMuteSearch;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._tempMuteSearch, value))
                {
                    this._muteAddCommand.RaiseCanExecute();
                }
            }
        }

        private string _tempMuteText;
        public string TempMuteText
        {
            get => this._tempMuteText;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._tempMuteText, value))
                {
                    this._muteAddCommand.RaiseCanExecute();
                }
            }
        }

        private Mute _selectedMute;
        public Mute SelectedMute
        {
            get => this._selectedMute;
            set
            {
                if (this.RaisePropertyChangedIfSet(ref this._selectedMute, value))
                {
                    this.MuteRemoveCommand.RaiseCanExecute();
                }
            }
        }

        #region Command: AddMuteCommand

        private Command _muteAddCommand;
        public Command MuteAddCommand => this._muteAddCommand ??= this.Commands.Create(this.MuteAdd, this.CanAddMute);

        private void MuteAdd()
        {
            if (Mute.Create(this._tempMuteType, this._tempMuteSearch, this._tempMuteText, out var m))
            {
                this.MuteList.Add(m);
                this.SelectedMute = m;
            }
        }

        private bool CanAddMute() => Mute.IsAvailable(this._tempMuteType, this._tempMuteSearch, this._tempMuteText);

        #endregion Command: AddMuteCommand

        #region Command: RemoveMuteCommand

        private Command<Mute> _removeMuteCommand;
        public Command<Mute> RemoveMuteCommand => this._removeMuteCommand ??= this.Commands.Create<Mute>(this.RemoveMute, IsAvailableMuteItem);

        private void RemoveMute(Mute mute) => this.MuteList.Remove(mute);

        private static bool IsAvailableMuteItem(Mute mute) => mute != null;

        #endregion Command: RemoveMuteCommand

        #region MuteRemoveCommand

        private Command<Mute> _muteRemoveCommand;
        public Command<Mute> MuteRemoveCommand
        {
            get => this._muteRemoveCommand ??= this.Commands.Create<Mute>(this.MuteRemove, this.CanMuteRemove);
        }

        private void MuteRemove(Mute item) => this.MuteList.Remove(item);

        private bool CanMuteRemove(Mute item) => this.MuteList.Contains(item);

        #endregion

        #endregion

        #region Timeline settings

        private const string SampleRelativeTime = "1分前";
        private const string SampleAbsoluteTime = "2018年1月1日 0時0分";

        private IReadOnlyList<FontInfo> _systemFonts;

        /// <summary>
        /// システムフォントリスト
        /// </summary>
        public IReadOnlyList<FontInfo> SystemFonts
        {
            get
            {
                if (this._systemFonts == null)
                {
                    this.UpdateSystemFonts();
                }

                return this._systemFonts;
            }
        }

        /// <summary>
        /// システムフォントリストを更新する。
        /// </summary>
        private void UpdateSystemFonts()
        {
            this._systemFonts = Fonts.SystemFontFamilies
                .Select(ff => new FontInfo(ff))
                .OrderBy(value => value.Source, StringComparer.CurrentCulture)
                .ToArray();

            this.RaisePropertyChanged(nameof(this.SystemFonts));
        }

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

        private ICommand _closeCommand;
        public ICommand CloseCommand => this._closeCommand ??= this.Commands.Create(this.OnClose, this.OnCloseRequest);

        private bool OnCloseRequest()
        {
            if (App.Accounts.Count == 0)
            {
                var message = new ConfirmationMessage("アカウントが登録されていません。終了しますか？", App.Name, "MsgKey_GeneralConfirm");
                this.Messenger.Raise(message);

                return message.Response ?? false;
            }

            return true;
        }

        private void OnClose()
        {
            //this.Setting.DefaultColumns.Clear();
            //foreach (var column in this.DefaultColumns)
            //{
            //    this.Setting.DefaultColumns.Add(column.GetOption());
            //}

            GlobalSetting.TimelineFont = this.TimelineFont;
            GlobalSetting.TimelineFontSize = this.TimelineFontSize.Value;
            GlobalSetting.TimelineFontRendering = this.TimelineFontRenderingMode;

            App.Instance.UIManager.Apply();
        }
    }
}
