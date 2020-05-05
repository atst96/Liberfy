using System;
using System.Threading.Tasks;
using Liberfy.Settings;

namespace Liberfy.Utilieis
{
    /// <summary>
    /// 設定データに関するクラス
    /// </summary>
    internal static class SettingUtil
    {
        /// <summary>
        /// すべての設定ファイルを読み込む。
        /// </summary>
        /// <returns></returns>
        public static async Task<(Setting, AccountSettings)> LoadSettings()
        {
            var generalSettingTask = SettingUtil.LoadGeneralSettings();
            var accountSettingTask = SettingUtil.LoadAccountsSetting();

            await Task.WhenAll(generalSettingTask, accountSettingTask).ConfigureAwait(false);

            return (
                generalSettingTask.Result ?? new Setting(),
                accountSettingTask.Result ?? new AccountSettings());
        }

        /// <summary>
        /// アカウント設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        private static Task<Setting> LoadGeneralSettings()
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return ParseSettingAsync<Setting>(filePath);
        }

        /// <summary>
        /// 一般設定ファイルを読み込む
        /// </summary>
        /// <returns></returns>
        private static Task<AccountSettings> LoadAccountsSetting()
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return ParseSettingAsync<AccountSettings>(filePath);
        }

        /// <summary>
        /// 設定ファイルを読み込む。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static async Task<T> ParseSettingAsync<T>(string filename) where T : class
        {
            try
            {
                return await JsonUtil.DeserializeFileAsync<T>(filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定ファイル \"{filename}\" の読み込みに失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// すべての設定ファイルを保存する。
        /// </summary>
        /// <returns>Task</returns>
        public static Task SaveSettings(Setting generalSetting, AccountSettings accountSetting)
        {
            return Task.WhenAll(
                SettingUtil.SaveGeneralSettings(generalSetting),
                SettingUtil.SaveAccountsSettings(accountSetting));
        }

        /// <summary>
        /// 設定データを保存する。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="setting"></param>
        /// <returns></returns>
        private static async Task SaveSetting<T>(string filename, T setting) where T : class
        {
            try
            {
                await JsonUtil.SerializeFileAsync(setting, filename).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"設定 \"{filename}\" の保存に失敗しました。\n{ex.GetMessage()}", ex);
            }
        }

        /// <summary>
        /// アカウント設定をファイルに保存する。
        /// </summary>
        /// <param name="accountSettings">アカウント設定</param>
        /// <returns></returns>
        private static Task SaveAccountsSettings(AccountSettings accountSettings)
        {
            var filePath = App.GetLocalFilePath(Defaults.AccountsFile);

            return SettingUtil.SaveSetting(filePath, accountSettings);
        }

        /// <summary>
        /// 一般設定をファイルに保存する。
        /// </summary>
        /// <param name="setting">設定</param>
        /// <returns></returns>
        private static Task SaveGeneralSettings(Setting setting)
        {
            var filePath = App.GetLocalFilePath(Defaults.SettingFile);

            return SettingUtil.SaveSetting(filePath, setting);
        }
    }
}
