using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Windows.Themes;
using Newtonsoft.Json;
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
using System.Windows.Documents;
using System.Windows.Media;

namespace Liberfy
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		internal static readonly object CommonLockObject = new object();

		private static AccountSetting _accounts;
		private static Setting _setting;

		public static Brush RetweetColor;
		public static Brush FavoriteColor;
		public static Brush RetweetFavoriteColor;

		internal static AccountSetting AccountSetting => _accounts;
		internal static Setting Setting => _setting;

		internal static FluidCollection<Account> Accounts { get; } = new FluidCollection<Account>();

		internal static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

		public const string AppName = "Liberfy";
		public const string AppCodeName = "Francium";
		public const string AppVersion = "0.1.2.0";

		internal static DictionaryEx<NotifyCode, bool> NotificationEvents { get; } = new DictionaryEx<NotifyCode, bool>();

		internal static TaskbarIcon TaskbarIcon { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// 設定を読み込む
			if (LoadSettingWithErrorDialog(Defines.AccountsFile, ref _accounts))
			{
				if (_accounts.Accounts?.Length > 0)
				{
					foreach (var item in _accounts.Accounts.Distinct())
					{
						Accounts.Add(new Account(item));
					}
				}

				if (_accounts.Columns?.Length > 0)
				{
					foreach (var s in _accounts.Columns)
					{
						if (ColumnBase.TryFromSetting(s, out var column))
						{
							Columns.Add(column);
						}
					}
				}
			}
			else
			{
				Shutdown(false);
				return;
			}

			if (!LoadSettingWithErrorDialog(Defines.SettingFile, ref _setting))
			{
				Shutdown(false);
				return;
			}

			LoadUISettingsFromSetting();

			_setting.Mute.ForEach(m => m.Apply());

			TaskbarIcon = (TaskbarIcon)FindResource("taskbarIcon");

			RetweetColor = (Brush)Current.Resources["RetweetColor"];
			FavoriteColor = (Brush)Current.Resources["FavoriteColor"];
			RetweetFavoriteColor = (Brush)Current.Resources["RetweetFavoriteColor"];

		}

		internal static FluidCollection<ColumnBase> Columns { get; } = new FluidCollection<ColumnBase>();

		private static bool LoadSettingWithErrorDialog<T>(string filename, ref T setting) where T : new()
		{
			try
			{
				setting = File.Exists(filename) ? SettingFromFile<T>(filename) : new T();
				return true;
			}
			catch (Exception e)
			{
				MessageBox.Show(IntPtr.Zero,
					$"設定ファイルの{(e is JsonException ? "保存形式が誤っています" : "読み込みに失敗しました")}：\n"
					+ $"{e.Message}\n\nアプリケーションを終了します。",
					caption: "エラー", icon: MsgBoxIcon.Error);
				return true;
			}
		}

		private static T SettingFromFile<T>(string filename)
		{
			return JsonConvert.DeserializeObject<T>(
				File.ReadAllText(filename));
		}

		private static void SaveSettingFile<T>(string filename, T TObj)
		{
			File.WriteAllText(filename,
				JsonConvert.SerializeObject(TObj, Formatting.Indented));
		}

		private static void SaveSettings()
		{
			// アカウント設定の保存
			AccountSetting.Accounts = Accounts.Select(a => a.ToSetting()).ToArray();
			// カラム設定の保存
			AccountSetting.Columns = Columns.Select(c => c.ToSetting()).ToArray();

			// 設定をファイルに保存
			SaveSettingWithErrorDialog(Defines.SettingFile, Setting);
			SaveSettingWithErrorDialog(Defines.AccountsFile, AccountSetting);
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

		public static void Shutdown(bool saveSettings = true)
		{
			if (_appClsoing) return;
			_appClsoing = true;

			if (saveSettings)
			{
				App.SaveSettings();
			}

			Current.Shutdown();
		}

		public static void LoadUISettingsFromSetting()
		{
			SetResources("ColumnWidth"                , Setting.ColumnWidth                              );
			SetResources("TweetProfileImageWidth"     , Setting.TweetProfileImageWidth                   );
			SetResources("TweetProfileImageVisibility", BoolToVisibility(Setting.IsShowTweetProfileImage));
			SetResources("TweetImagesVIsibility"      , BoolToVisibility(Setting.IsShowTweetImages      ));
			SetResources("TweetQuotedTweetVisibility" , BoolToVisibility(Setting.IsShowTweetQuotedTweet ));
			SetResources("TweetClientNameVisibility"  , BoolToVisibility(Setting.IsShowTweetClientName  ));

			Geometry clip;
			switch(Setting.ProfileImageForm)
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
					clip = null;
					break;
			}

			SetResources("TweetProfileImageClip", clip);
		}

		public static Visibility BoolToVisibility(bool isVisible)
		{
			return isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public static bool SetResources(string key, object value)
		{
			if (Current.TryFindResource(key) != value)
			{
				Current.Resources[key] = value;

				return true;
			}

			return false;
		}

		internal static void ForceExit()
		{
			// TODO: 仮処理
			Environment.Exit(0);
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
