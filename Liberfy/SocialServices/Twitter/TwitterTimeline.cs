using Liberfy.ViewModel;
using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Liberfy.SocialServices.Twitter;

namespace Liberfy
{
    internal class TwitterTimeline : TimelineBase, IObserver<IItem>
    {
        private static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly TwitterAccount _account;
        public Tokens _tokens => (Tokens)_account.InternalTokens;

        private FakeStreaming _streaming;

        public event EventHandler OnUnloading;

        public TwitterTimeline(TwitterAccount account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public override async void Load()
        {
            var waitingTasks = new[]
            {
                this.GetHomeTimeline(),
                this.GetNotificationTimeline(),
                this.GetMessageTimeline()
            };

            await Task.WhenAll(waitingTasks);

            this.StartStream();
        }

        private void StartStream()
        {
            this._streaming = new FakeStreaming(this._account);

            var accountColumn = Columns
                .Where(a => a.Type == ColumnType.Home && a.Account == this._account)
                .FirstOrDefault();

            var topItem = accountColumn?.Items?.FirstOrDefault();

            if (topItem is StatusItem item)
            {
                this._streaming.LatestHomeStatusId = item.Id;
            }

            this._streaming.Subscribe(this);
            this._streaming.Start();
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
                {
                    yield return column;
                }
            }
        }

        private Task GetHomeTimeline() => Task.Run(async () =>
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
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        });

        private Task GetNotificationTimeline() => Task.Run(async () =>
        {
            try
            {
                var statuses = await _tokens.Statuses.MentionsTimeline();
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.GetCurrentAccountColumns().Where(c => c.Type == ColumnType.Notification))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        });

        private Task GetMessageTimeline() => Task.Run(() =>
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

            var accountColumns = Columns
                .Where(c => c.Account == this._account)
                .ToArray();

            foreach (var c in accountColumns)
            {
                Columns.Remove(c);
            }

            this._streaming?.Stop();

            //this.Columns.Clear();
        }

        public void OnNext(IItem value)
        {
            var columns = Columns.Where(a => a.Type == ColumnType.Home && a.Account == this._account);

            foreach (var column in columns)
            {
                column.Items.Insert(0, value);
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}
