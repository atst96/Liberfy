using Liberfy.Data.Settings.Columns;

namespace Liberfy
{
    /// <summary>
    /// 通知カラム
    /// </summary>
    internal class NotificationColumn : StatusColumnBase
    {
        public NotificationColumn(IAccount account, NotificationColumnSetting setting)
            : base(account, ColumnType.Notification, "Notification")
        {
            this.LoadSetting(setting);
        }

        /// <summary>
        /// カラムの設定を読み込む。
        /// </summary>
        /// <param name="setting"></param>
        private void LoadSetting(NotificationColumnSetting setting)
        {
        }

        /// <summary>
        /// カラムの設定を取得する。
        /// </summary>
        /// <returns></returns>
        public override IColumnSetting GetSetting()
        {
            return new NotificationColumnSetting(this.Account.ItemId)
            {
            };
        }
    }
}
