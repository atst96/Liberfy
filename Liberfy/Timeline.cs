using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class Timeline : NotificationObject
	{
		private Account _account;
		private Tokens _tokens => _account.Tokens;
		private FluidCollection<ColumnBase> Columns => App.Columns;

		private readonly long _userId;

		public Timeline(Account account)
		{
			_account = account;
			_userId = account.Id;
		}

		public void Load()
		{
			Parallel.Invoke(
				LoadHomeTimeline,
				LoadNotificationTimeline,
				LoadMessageTimeline
			);
		}

		private void LoadHomeTimeline()
		{
			try
			{
				var statuses = _tokens.Statuses.HomeTimeline();
				var items = GetStatusItem(statuses);

				Columns
					.Where(c => c.Type == ColumnType.Home)
					.ForEach(c => c.Items.Reset(items), App.Current.Dispatcher);
			}
			catch
			{
				// TODO: 取得失敗時の処理
			}
		}

		private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
		{
			return statuses.Select(s => new StatusItem(s, _account));
		}

		private void LoadNotificationTimeline()
		{
			try
			{
				var statuses = _tokens.Statuses.MentionsTimeline();
				var items = GetStatusItem(statuses);

				Columns
					.Where(c => c.Type == ColumnType.Notification)
					.ForEach(c => c.Items.Reset(items), App.Current.Dispatcher);
			}
			catch
			{
				// TODO: 取得失敗時の処理
			}
		}

		private void LoadMessageTimeline()
		{
			try
			{

			}
			catch
			{
				// TODO: 取得失敗時の処理
			}
		}

		public void Unload()
		{
			var columns = Columns;

			foreach (var column in columns
				.Where(c => c.Account.Id == _userId).ToArray())
			{
				columns.Remove(column);
			}
		}
	}
}
