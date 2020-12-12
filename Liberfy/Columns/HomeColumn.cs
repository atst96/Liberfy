using Liberfy.Data.Settings.Columns;

namespace Liberfy
{
    /// <summary>
    /// Homeカラム
    /// </summary>
    internal class HomeColumn : StatusColumnBase
    {
        public HomeColumn(IAccount account, HomeColumnSetting setting)
            : base(account, ColumnType.Home, "Home")
        {
            this.LoadSetting(setting);
        }

        /// <summary>
        /// カラムの設定を読み込む。
        /// </summary>
        /// <param name="setting"></param>
        private void LoadSetting(HomeColumnSetting setting)
        {
        }

        /// <summary>
        /// カラム設定を取得する。
        /// </summary>
        /// <returns></returns>
        public override IColumnSetting GetSetting()
        {
            return new HomeColumnSetting(this.Account.ItemId)
            {
            };
        }
    }
}
