using Liberfy.Settings;

namespace Liberfy.Data.Settings
{
    /// <summary>
    /// 設定情報
    /// </summary>
    internal class SettingsGroup
    {
        /// <summary>
        /// アカウント設定
        /// </summary>
        public AccountSettings Accounts { get; set; }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        public Setting Application { get; set; }
    }
}
