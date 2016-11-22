using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Liberfy
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		internal static readonly object CommonLockObject = new object();

		internal static AccountCollection Accounts { get; private set; }
		internal static Setting Setting { get; private set; } 

		protected override void OnStartup(StartupEventArgs e)
		{
			InitializeSettings();

			base.OnStartup(e);
		}

		private static bool InitializeSettings()
		{
			Setting = OpenSettingFile<Setting>("settings.json");
			Accounts = OpenSettingFile<AccountCollection>("accounts.json");

			return false;
		}

		private static T OpenJsonFile<T>(string path)
		{
			StreamReader sr = null;

			try
			{
				return JsonConvert.DeserializeObject<T>(
					(sr = File.OpenText(path)).ReadToEnd());
			}
			finally
			{
				sr?.Dispose();
			}
		}

		private static T OpenSettingFile<T>(string path) where T: class
		{
			try
			{
				return OpenJsonFile<T>(path);
			}
			catch (FileNotFoundException)
			{
				return Activator.CreateInstance<T>();
			}
			catch (Exception ex)
			{
				MsgBoxResult i;
				switch(i = MessageBox.Show(IntPtr.Zero,
					"設定ファイルの読み込みに失敗しました:\n" + ex.Message,
					"Liberfy", MsgBoxButtons.CancelTryContinue, MsgBoxIcon.Error))
				{
					case MsgBoxResult.TryAgain:
						return OpenSettingFile<T>(path);

					case MsgBoxResult.Continue:
						return Activator.CreateInstance<T>();

					default: ForceExit(); return null;
				}
			}
		}

		internal static void ForceExit()
		{
			// TODO: 仮処理
			Environment.Exit(0);
		}
	}
}
