#nullable enable

using MessagePack;

namespace Liberfy.Data.Settings.Columns
{
    [MessagePackObject]
    internal class NotificationColumnSetting : IColumnSetting
    {
        /// <summary>
        /// 内部ユーザID
        /// </summary>
        [Key(0)]
        public string UserId { get; init; }

        /// <summary>
        /// カラム種別
        /// </summary>
        [Key(1)]
        public ColumnType Type { get; init; } = ColumnType.Notification;

        [SerializationConstructor]
        private NotificationColumnSetting()
        {
        }

        public NotificationColumnSetting(string userId)
        {
            this.UserId = userId;
        }
    }
}
