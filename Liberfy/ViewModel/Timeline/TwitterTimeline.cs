using Liberfy.ViewModel;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal class TwitterTimeline : NotificationObject, ViewModel.Timeline.ITimeline
    {
        private static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly TwitterAccount _account;
        public Tokens _tokens => (SocialApis.Twitter.Tokens)_account.InternalTokens;
        public FluidCollection<IColumn> Columns { get; } = new FluidCollection<IColumn>();

        public event EventHandler<IEnumerable<StatusItem>> OnHomeStatusesLoaded;
        public event EventHandler<IEnumerable<IItem>> OnNotificationsLoaded;
        public event EventHandler OnUnloading;

        public TwitterTimeline(TwitterAccount account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public TwitterTimeline(TwitterAccount account, IEnumerable<ColumnOptionBase> columnOptions)
            : this(account)
        {
            this.LoadColumns(columnOptions);
        }

        public void LoadColumns(IEnumerable<ColumnOptionBase> columnOptions)
        {
            foreach (var columnSetting in columnOptions)
            {
                if (ColumnBase.TryFromSetting(columnSetting, this, out var column))
                    this.Columns.Add(column);
            }
        }

        public void Load()
        {
            this.LoadHomeTimelineAsync();
            this.LoadNotificationTimelineAsync();
            this.LoadMessageTimelineAsync();
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {
            return statuses.Select(s => new StatusItem(s, _account));
        }

        private Task LoadHomeTimelineAsync() => Task.Run(async () =>
        {
            try
            {
                var statuses = await _tokens.Statuses.HomeTimeline();
                var items = this.GetStatusItem(statuses);

                if (this.OnHomeStatusesLoaded != null)
                {
                    await _dispatcher.InvokeAsync(() => this.OnHomeStatusesLoaded(this, items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        });

        private Task LoadNotificationTimelineAsync() => Task.Run(async () =>
        {
            try
            {
                var statuses = await _tokens.Statuses.MentionsTimeline();
                var items = this.GetStatusItem(statuses);

                if (this.OnNotificationsLoaded != null)
                {
                    await _dispatcher.InvokeAsync(() => this.OnNotificationsLoaded(this, items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        });

        private Task LoadMessageTimelineAsync() => Task.Run(() =>
        {
            try
            {

            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        });

        public void Unload()
        {
            this.OnUnloading?.Invoke(this, EventArgs.Empty);
            this.Columns.Clear();
        }
    }
}
