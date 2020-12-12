using Liberfy.Data.Settings.Columns;
using MessagePack;

namespace Liberfy.Data.Settings.Columns
{
    /// <summary>
    /// カラム設定のインタフェース
    /// </summary>
    [Union(0, typeof(HomeColumnSetting))]
    [Union(1, typeof(NotificationColumnSetting))]
    internal interface IColumnSetting
    {
        /// <summary>
        /// 内部ユーザID
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// カラム種別
        /// </summary>
        public ColumnType Type { get; }
    }
}
