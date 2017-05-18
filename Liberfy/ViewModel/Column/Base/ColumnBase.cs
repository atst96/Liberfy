using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	internal enum ColumnType
	{
		Status       = 0x01,
		Home         = Status | 0x02,
		Notification = Status | 0x04,
		Messages     = 0x08,
		Search       = Status | 0x10,
		List         = Status | 0x20,
		Stream       = Status | 0x40,
	}

	internal abstract class ColumnBase : NotificationObject
	{
		protected ColumnBase(Account account, ColumnType type, string title = null)
		{
			userId = (Account = account).Id;
			Type = type;
			_title = title;

			Setting = new ColumnSetting(Type, userId);
		}

		public ColumnType Type { get; }

		protected long userId;

		public Account Account { get; }

		protected ColumnSetting Setting { get; private set; }

		protected T TryGetProp<T>(string propertyName)
		{
			return Setting.Properties.TryGetValue<T>(propertyName);
		}

		protected void SetProp<T>(string propertyName, T value)
		{
			Setting.Properties[propertyName] = value;
		}

		private string _title;
		public string Title
		{
			get => _title;
			set => SetProperty(ref _title, value);
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get => _isLoading;
			set => SetProperty(ref _isLoading, value);
		}

		private string _status;
		public string Status
		{
			get => _status ?? string.Empty;
			set
			{
				if(SetProperty(ref _status, value))
				{
					_hasStatus = !string.IsNullOrWhiteSpace(_status);
					RaisePropertyChanged(nameof(_hasStatus));
				}
			}
		}

		private bool _hasStatus;
		public bool HasStatus => _hasStatus;

		public virtual bool IsStatusColumn { get; } = false;

		public abstract void OnShowDetails(IItem item);

		public ColumnSetting ToSetting() => Setting;

		protected T GetPropValue<T>(ref T refValue, string propertyName, T defaultValue = null) where T : class
		{
			return refValue ?? (refValue = Setting.Properties?.TryGetValue<T>(propertyName) ?? defaultValue);
		}

		protected T? GetPropValue<T>(ref T? refValue, string propertyName) where T : struct
		{
			return refValue ?? (refValue = Setting.Properties?.TryGetValue<T?>(propertyName));
		}

		protected T GetPropValue<T>(ref T? refValue, string propertyName, T defaultValue) where T : struct
		{
			return refValue ?? (refValue = Setting.Properties?.TryGetValue<T?>(propertyName) ?? defaultValue).Value;
		}

		protected bool SetValueWithProp<T>(ref T refValue, T newValue, string settingPropName, [CallerMemberName] string propertyName = "")
		{
			bool changed = !Equals(refValue, newValue);

			Setting.Properties[settingPropName] = JToken.FromObject(newValue);

			if (changed)
			{
				refValue = newValue;
				RaisePropertyChanged(propertyName);
			}

			return changed;
		}

		public static ColumnBase FromSettings(ColumnSetting s)
		{
			if (s == null) return null;

			ColumnBase column;

			Account ac;

			if(s.UserId == Account.DummyId)
			{
				ac = Account.Dummy;
			}
			else
			{
				ac = App.AccountSetting.FromId(s.UserId);
				if (ac == null) {
					throw new ArgumentOutOfRangeException();
				}
			}

			switch(s.Type)
			{
				case ColumnType.Home:
					column = new StatusColumn(ac, ColumnType.Home, "Home");
					break;

				case ColumnType.Notification:
					column = new StatusColumn(ac, ColumnType.Notification, "Notification");
					break;

				case ColumnType.Search:
					column = new SearchColumn(ac);
					break;

				case ColumnType.List:
					column = new ListColumn(ac);
					break;

				case ColumnType.Stream:
					column = new StreamSearchColumn(ac);
					break;

				case ColumnType.Messages:
					column = new MessageColumn(ac);
					break;

				default:
					throw new NotImplementedException();
			}

			column.Setting = s;

			return column;
		}

		public static Dictionary<ColumnType, string> ColumnTypes { get; }
			= new Dictionary<ColumnType, string>
			{
				[ColumnType.Home] = "ホーム",
				[ColumnType.Notification] = "通知",
				[ColumnType.Messages] = "ダイレクトメッセージ",
				[ColumnType.Search] = "検索",
				[ColumnType.List] = "リスト",
				[ColumnType.Stream] = "リアルタイム検索",
			};

		public static Dictionary<ColumnType, string> ColumnNames { get; }
			= new Dictionary<ColumnType, string>
			{
				[ColumnType.Home] = "Home",
				[ColumnType.Notification] = "Notification",
				[ColumnType.Messages] = "Message",
				[ColumnType.Search] = "Search",
				[ColumnType.List] = "List",
				[ColumnType.Stream] = "Stream",
			};
	}
}
