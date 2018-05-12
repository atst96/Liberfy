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

        internal static FluidCollection<Account> Accounts { get; private set; }

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
                e.Cancel = Setting.SystemCancelSignout;
            else if (e.Reason == SessionEndReasons.SystemShutdown)
                e.Cancel = Setting.SystemCancelShutdown;
        }

        private static void InitializeProgram()
        {
            IEnumerable<AccountItem> accountsSetting = null;

            // 作業ディレクトリの再指定（自動起動時に C:/Windows/system32になってしまうため）

            var workingDirectory = Path.GetDirectoryName(Assembly.Location);
            Directory.SetCurrentDirectory(workingDirectory);

            // アプリケーション設定の読み込み

            if (TryParseSettingFileOrDisplayError(Defines.AccountsFile, ref accountsSetting))
            {
                Accounts = new FluidCollection<Account>(
                    accountsSetting.Distinct().Select(a => new Account(a)));
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

            ApplyUISettings();

            _setting.Mute.ForEach(m => m.Apply());

            TaskbarIcon = GetResource<TaskbarIcon>("taskbarIcon");
        }

        private static bool TryParseSettingFileOrDisplayError<T>(string filename, ref IEnumerable<T> setting)
        {
            try
            {
                setting = SettingFromFile<IEnumerable<T>>(filename) ?? Enumerable.Empty<T>();
                return true;
            }
            catch (Exception e)
            {
                DisplayException(e, $"設定ファイルの読み込みに失敗しました:\n{ filename }");
                return true;
            }
        }

        private static bool TryParseSettingFileOrDisplayError<T>(string filename, ref T setting) where T : class, new()
        {
            try
            {
                setting = SettingFromFile<T>(filename) ?? new T();
                return true;
            }
            catch (Exception e)
            {
                DisplayException(e, "設定ファイルの読み込みに失敗しました");
                return true;
            }
        }

        public static void DisplayException(Exception exception, string instruction = null)
        {
            MessageBox.Show(IntPtr.Zero,
                (string.IsNullOrEmpty(instruction) ? "" : $"{ instruction }:\n")
                + $"{ exception.Message }\n\nアプリケーションを終了します。",
                caption: "エラー",
                icon: MsgBoxIcon.Error);
        }

        private static IJsonFormatterResolver _jsonFormatterResolver = Utf8Json.Resolvers.StandardResolver.AllowPrivate;

        private static T SettingFromFile<T>(string filename)
        {
            try
            {
                using (var fs = File.OpenRead(filename))
                    if (fs.Length > 0)
                        return JsonSerializer.Deserialize<T>(fs, _jsonFormatterResolver);
            }
            catch (FileNotFoundException)
            {
                // pass
            }

            return default(T);
        }

        private static void SaveSettingFile<T>(string filename, T TObj)
        {
            using (var fs = File.Open(filename, FileMode.Create))
                JsonSerializer.Serialize(fs, TObj, _jsonFormatterResolver);
        }

        private static void SaveSettings()
        {
            var accountsSetting = Accounts.Select(a => a.ToSetting());

            // 設定をファイルに保存
            SaveSettingWithErrorDialog(Defines.SettingFile, Setting);
            SaveSettingWithErrorDialog(Defines.AccountsFile, accountsSetting);
        }

        private static void SaveSettingWithErrorDialog<T>(string filename, T setting) where T : class
        {
            try
            {
                SaveSettingFile(filename, setting);
            }
            catch (Exception e)
            {
                MessageBox.Show(IntPtr.Zero,
                    $"設定ファイルの保存に失敗しました。：\n{e.Message}",
                    caption: "エラー", icon: MsgBoxIcon.Error);
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

        private static ProfileImageForm? _previousProfileImageForm = null;

        public static void ApplyUISettings()
        {
            SetResource("UI.Column.Width", Setting.ColumnWidth);
            SetResource("UI.Tweet.ProfileImage.Width", Setting.TweetProfileImageWidth);
            SetResource("UI.Tweet.ProfileImage.Visibility", BoolToVisibility(Setting.IsShowTweetProfileImage));
            SetResource("UI.Tweet.Attachment.Images.Visibility", BoolToVisibility(Setting.IsShowTweetImages));
            SetResource("UI.Tweet.Attachment.QuotedTweet.Visibility", BoolToVisibility(Setting.IsShowTweetQuotedTweet));
            SetResource("UI.Tweet.ClientName.Visibility", BoolToVisibility(Setting.IsShowTweetClientName));

            var profileImageForm = Setting.ProfileImageForm;
            if (_previousProfileImageForm != profileImageForm)
            {
                Geometry clip;
                switch (profileImageForm)
                {
                    case ProfileImageForm.RoundedCorner:
                        clip = new RectangleGeometry
                        {
                            RadiusX = 3.0d,
                            RadiusY = 3.0d,
                            Rect = new Rect
                            {
                                Width = Setting.TweetProfileImageWidth,
                                Height = Setting.TweetProfileImageWidth,
                            }
                        };
                        break;

                    case ProfileImageForm.Ellipse:
                        double halfWidth = Setting.TweetProfileImageWidth / 2.0d;
                        clip = new EllipseGeometry
                        {
                            RadiusX = halfWidth,
                            RadiusY = halfWidth,
                            Center = new Point(halfWidth, halfWidth)
                        };
                        break;

                    default:
                        clip = new RectangleGeometry(new Rect
                        {
                            Width = Setting.TweetProfileImageWidth,
                            Height = Setting.TweetProfileImageWidth,
                        });
                        break;
                }

                SetResource("UI.Tweet.ProfileImage.Clip", clip);
                _previousProfileImageForm = profileImageForm;
            }
        }

        public static Visibility BoolToVisibility(bool isVisible)
        {
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public static T GetResource<T>(object resourceKey)
        {
            return Current.TryFindResource(resourceKey) is T value ? value : default(T);
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
