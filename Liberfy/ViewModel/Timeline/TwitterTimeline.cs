using Liberfy.ViewModel;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal class TwitterTimeline : TimelineBase
    {
        private static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly TwitterAccount _account;
        public Tokens _tokens => (Tokens)_account.InternalTokens;

        public event EventHandler OnUnloading;

        public TwitterTimeline(TwitterAccount account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public override void Load()
        {
            this.LoadHomeTimelineAsync();
            this.LoadNotificationTimelineAsync();
            this.LoadMessageTimelineAsync();
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses)
            {
                yield return new StatusItem(status, this._account);
            }
        }

        private IEnumerable<ColumnBase> GetCurrentAccountColumns()
        {
            foreach (var column in TimelineBase.Columns)
            {
                if (column.Account?.Equals(this._account) ?? false)
                    yield return column;
            }
        }

        private Task LoadHomeTimelineAsync() => Task.Run(async () =>
        {
            try
            {
                var statuses = await _tokens.Statuses.HomeTimeline(new Query
                {
                    ["tweet_mode"] = "extended",
                });
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.GetCurrentAccountColumns().Where(c => c.Type == ColumnType.Home))
                {
                    await _dispatcher.InvokeAsync(() => column.Items.Reset(items));
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

                foreach (var column in this.GetCurrentAccountColumns().Where(c => c.Type == ColumnType.Notification))
                {
                    await _dispatcher.InvokeAsync(() => column.Items.Reset(items));
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

        public override void Unload()
        {
            this.OnUnloading?.Invoke(this, EventArgs.Empty);
            //this.Columns.Clear();
        }
    }
}
