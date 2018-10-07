using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Utf8Json;

namespace Liberfy
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        internal static readonly object CommonLockObject = new object();

        private static Setting _setting;
        internal static Setting Setting => _setting;

        internal static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        public const string AppName = "Liberfy";
        public const string AppCodeName = "Francium";
        public const string AppVersion = "0.1.2.0";

        internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

        internal static TaskbarIcon TaskbarIcon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            InitializeProgram();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.Logoff)
            {
                e.Cancel = Setting.SystemCancelSignout;
            }
            else if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                e.Cancel = Setting.SystemCancelShutdown;
            }
        }

        private static void InitializeProgram()
        {
            Settings.AccountSetting accountsSetting = null;

            // 作業ディレクトリの再指定（自動起動時に C:/Windows/system32になってしまうため）

            var workingDirectory = Path.GetDirectoryName(Assembly.Location);
            Directory.SetCurrentDirectory(workingDirectory);

            // アプリケーション設定の読み込み

            if (TryParseSettingFileOrDisplayError(Defines.AccountsFile, ref accountsSetting))
            {
                if (accountsSetting.Accounts?.Any() ?? false)
                {
                    AccountManager.Load(accountsSetting.Accounts);
                }

                if (accountsSetting.Columns?.Any() ?? false)
                {
                    var columns = new LinkedList<ColumnBase>();

                    foreach (var columnSetting in accountsSetting.Columns)
                    {
                        var account = AccountManager.Get(columnSetting.Service, columnSetting.UserId);

                        if (account != null && ColumnBase.FromSetting(columnSetting, account, out var column))
                        {
                            columns.AddLast(column);
                        }
                    }

                    TimelineBase.Columns.Reset(columns);
                }
            }
            else
            {
                Shutdown(false);
                return;
            }

            if (!TryParseSettingFileOrDisplayError(Defines.SettingFile, ref _setting))
            {
                Shutdown(false);
                return;
            }

            UI.ApplyFromSettings();

            foreach (var muteItem in _setting.Mute)
            {
                muteItem.Apply();
            }

            TaskbarIcon = GetResource<TaskbarIcon>("taskbarIcon");
        }

        private static bool TryParseSettingFileOrDisplayError<T>(string filename, ref IEnumerable<T> setting)
        {
            try
            {
                setting = FileContentUtility.DeserializeJsonFromFile<IEnumerable<T>>(filename) ?? Enumerable.Empty<T>();
                return true;
            }
            catch (Exception e)
            {
                DisplayException(e, $"設定ファイルの読み込みに失敗しました:\n{ Path.GetFileName(filename) }");
                return true;
            }
        }

        private static bool TryParseSettingFileOrDisplayError<T>(string filename, ref T setting) where T : class, new()
        {
            try
            {
                setting = FileContentUtility.DeserializeJsonFromFile<T>(filename) ?? new T();
                return true;
            }
            catch (Exception e)
            {
                DisplayException(e, "設定ファイルの読み込みに失敗しました");
                return true;
            }
        }

        public static void DisplayException(Exception ex, string instruction = null)
        {
            const string NewLine = "\n";

            var message = string.IsNullOrEmpty(instruction)
                ? string.Empty
                : instruction + NewLine;

            message += string.Join(NewLine, ex.Message, ex.StackTrace);

            MessageBox.Show(IntPtr.Zero, message, "エラー", icon: MsgBoxIcon.Error);
        }

        private static void SaveSettings()
        {
            var accountsSetting = new Settings.AccountSetting
            {
                Accounts = AccountManager.Accounts.Select(a => a.ToSetting()),
                Columns = TimelineBase.Columns.Select(c => c.GetOption()),
            };

            // 設定をファイルに保存
            SaveSettingWithErrorDialog(Defines.SettingFile, Setting);
            SaveSettingWithErrorDialog(Defines.AccountsFile, accountsSetting);
        }

        private static void SaveSettingWithErrorDialog<T>(string filename, T setting) where T : class
        {
            try
            {
                FileContentUtility.SerializeJsonToFile(setting, filename);
            }
            catch (Exception e)
            {
                var message = string.Join("\n",
                    "設定ファイルの保存に失敗しました\nファイル名：", Path.GetFileName(filename), e.Message, e.StackTrace);

                MessageBox.Show(IntPtr.Zero, message, "エラー", icon: MsgBoxIcon.Error);
            }
        }

        private static bool _appClsoing;

        public static void Shutdown(bool saveSettings)
        {
            if (_appClsoing) return;
            _appClsoing = true;

            if (saveSettings)
            {
                App.SaveSettings();
            }

            Current.Shutdown();
        }

        public static Visibility BoolToVisibility(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static T GetResource<T>(object resourceKey)
        {
            return Current.TryFindResource(resourceKey) is T value ? value : default;
        }

        public static bool SetResource(object key, object value)
        {
            if (Current.TryFindResource(key) != value)
            {
                Current.Resources[key] = value;

                return true;
            }

            return false;
        }

        internal static bool Open(string path)
        {
            try
            {
                Process.Start(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static bool Open(Uri uri)
        {
            return Open(uri.AbsoluteUri);
        }
    }
}
