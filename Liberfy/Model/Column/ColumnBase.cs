using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	enum ColumnType
	{
		Status = 0x01,
		Home = Status | 0x02,
		Notification = Status | 0x04,
		Messages = 0x08,
		Search = Status | 0x10,
		List = Status | 0x20,
		Stream = Status | 0x40,
	}

	abstract class ColumnBase : NotificationObject
	{
		protected ColumnBase(Account account, ColumnType type, string title = null)
		{
			userId = (Account = account).Id;
			Type = type;
			_title = title;
		}

		public ColumnType Type { get; }

		protected long userId;
		
		public Account Account { get; }

		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}

		private bool _isLoading;
		public bool IsLoading
		{
			get { return _isLoading; }
			set { SetProperty(ref _isLoading, value); }
		}

		public virtual bool IsStatusColumn { get; } = false;

		public abstract void OnShowDetails(IItem item);

		protected virtual ColumnProperties CreateProperties() => null;

		protected virtual void ApplyProperties(ColumnProperties prop) { }

		public ColumnSetting ToSetting()
		{
			return new ColumnSetting
			{
				Type = Type,
				UserId = userId,
				Properties = CreateProperties(),
			};
		}

		public static ColumnBase FromSettings(ColumnSetting s)
		{
			ColumnBase column;

			var ac = App.Accounts.FromId(s.UserId);
			if (ac == null) {
				throw new ArgumentOutOfRangeException();
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

				default:
					throw new NotImplementedException();
			}

			if (s.Properties != null)
			{
				column.ApplyProperties(s.Properties);
			}

			return column;
		}
	}
}
