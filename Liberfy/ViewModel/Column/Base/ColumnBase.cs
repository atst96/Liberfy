using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal enum ColumnType
    {
        [EnumMember(Value = "status")]
        Status = 0x01,
        [EnumMember(Value = "home")]
        Home = Status | 0x02,
        [EnumMember(Value = "notification")]
        Notification = Status | 0x04,
        [EnumMember(Value = "messages")]
        Messages = 0x08,
        [EnumMember(Value = "search")]
        Search = Status | 0x10,
        [EnumMember(Value = "list")]
        List = Status | 0x20,
        [EnumMember(Value = "stream")]
        Stream = Status | 0x40,
    }

    internal static class ColumnBase
    {
        private static IColumn FromType(TwitterTimeline timeline, ColumnType type)
        {
            switch (type)
            {
                case ColumnType.Home:
                    return new HomeColumn(timeline);

                case ColumnType.Notification:
                    return new NotificationColumn(timeline);

                case ColumnType.Search:
                    return new SearchColumn(timeline);

                case ColumnType.List:
                    return new ListColumn(timeline);

                case ColumnType.Stream:
                    return new StreamSearchColumn(timeline);

                case ColumnType.Messages:
                    return new MessageColumn(timeline);

                default:
                    return null;
            }
        }

        public static bool TryFromSetting(ColumnOptionBase option, TwitterTimeline timeline, out IColumn column)
        {
            column = option == null ? null : FromType(timeline, option.Type);

            if (column == null)
            {
                return false;
            }
            else
            {
                column.SetOption(option);
                return true;
            }
        }

        public static IColumn FromSettings(ColumnOptionBase s, TwitterTimeline timeline)
        {
            return TryFromSetting(s, timeline, out var c) ? c : throw new NotSupportedException();
        }

        public static LocalizeDictionary<ColumnType> ColumnTypes { get; }
            = new LocalizeDictionary<ColumnType>(new Dictionary<object, string>
            {
                [ColumnType.Home] = "ホーム",
                [ColumnType.Notification] = "通知",
                [ColumnType.Messages] = "ダイレクトメッセージ",
                [ColumnType.Search] = "検索",
                [ColumnType.List] = "リスト",
                [ColumnType.Stream] = "リアルタイム検索",
            });

        public static IReadOnlyDictionary<ColumnType, string> ColumnNames { get; }
            = new ReadOnlyDictionary<ColumnType, string>(new Dictionary<ColumnType, string>
            {
                [ColumnType.Home] = "Home",
                [ColumnType.Notification] = "Notification",
                [ColumnType.Messages] = "Message",
                [ColumnType.Search] = "Search",
                [ColumnType.List] = "List",
                [ColumnType.Stream] = "Stream",
            });
    }

    internal abstract class ColumnBase<TOption> : NotificationObject, IColumn
        where TOption : ColumnOptionBase
    {
        protected ColumnBase(TwitterTimeline timeline, ColumnType type, string title = null)
        {
            this.Type = type;
            this._title = title;
            this._timeline = timeline;
        }

        protected Dispatcher Dispatcher { get; } = App.Current.Dispatcher;

        private readonly TwitterTimeline _timeline;

        public ColumnType Type { get; }

        private TOption _option;
        public TOption Option => _option ?? (_option = this.CreateOption());

        public ColumnOptionBase GetOption() => this.Option;

        public void SetOption(ColumnOptionBase option)
        {
            this._option = (option as TOption ?? this.CreateOption());
        }

        protected abstract TOption CreateOption();

        public FluidCollection<IItem> Items { get; } = new FluidCollection<IItem>();

        private string _title;
        public string Title
        {
            get => this._title;
            set => this.SetProperty(ref this._title, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => this._isLoading;
            set => this.SetProperty(ref this._isLoading, value);
        }

        private bool _isDetailOpen;
        public bool IsDetailOpen
        {
            get => this._isDetailOpen;
            set => this.SetProperty(ref this._isDetailOpen, value);
        }

        private string _status;
        public string Status
        {
            get => _status ?? string.Empty;
            set
            {
                if (this.SetProperty(ref this._status, value))
                {
                    this._hasStatus = !string.IsNullOrWhiteSpace(this._status);
                    this.RaisePropertyChanged(nameof(_hasStatus));
                }
            }
        }

        private bool _hasStatus;
        public bool HasStatus => _hasStatus;

        public virtual bool IsStatusColumn { get; } = false;

        public virtual void OnShowDetails(IItem item) { }
    }
}
