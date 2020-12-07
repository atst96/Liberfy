using SocialApis;
using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Liberfy.SocialServices.Twitter;

namespace Liberfy.Services.Twitter
{
    internal class TwitterTimeline : TimelineBase, IObserver<IItem>
    {
        private static readonly Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly TwitterAccount _account;
        private TwitterApi _tokens => this._account.Api;

        private FakeStreaming _streaming;

        public event EventHandler OnUnloading;

        public TwitterTimeline(TwitterAccount account)
            : base(account)
        {
            this._account = account;
        }

        public override async void Load()
        {
            var tasks = new[]
            {
                this.GetHomeTimeline(),
                this.GetNotificationTimeline(),
                this.GetMessageTimeline()
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            this.StartStream();
        }

        private void StartStream()
        {
            this._streaming = new FakeStreaming(this._account);

            var accountColumn = this.AccountColumns
                .GetsTypeOf(ColumnType.Home)
                .FirstOrDefault();

            var topItem = accountColumn?.Items?.FirstOrDefault();

            if (topItem is StatusItem item)
            {
                this._streaming.LatestStatusId = item.Id;
            }

            this._streaming.Subscribe(this);
            this._streaming.Start();
        }

        private IEnumerable<StatusItem> GetStatusItem(ICollection<Status> statuses)
        {
            return from status in statuses select new StatusItem(status, this._account);
        }

        private async Task GetHomeTimeline()
        {
            try
            {
                var statuses = await this._tokens.Statuses.HomeTimeline(new Query
                {
                    ["tweet_mode"] = "extended",
                }).ConfigureAwait(false);

                var items = this.GetStatusItem(statuses);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Home))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        }

        private async Task GetNotificationTimeline()
        {
            try
            {
                var statuses = await this._tokens.Statuses.MentionsTimeline().ConfigureAwait(false);
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Notification))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            catch
            {
                // TODO: 取得失敗時の処理
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "<保留中>")]
        private Task GetMessageTimeline()
        {
            //try
            //{
            return Task.CompletedTask;
            //}
            //catch
            //{
            //    // TODO: 取得失敗時の処理
            //}
        }

        public override void Unload()
        {
            this.OnUnloading?.Invoke(this, EventArgs.Empty);

            this.AccountColumns.Clear();

            this._streaming?.Stop();
        }

        public void OnNext(IItem value)
        {
            var columns = this.AccountColumns.GetsTypeOf(ColumnType.Home);

            foreach (var column in columns)
            {
                _dispatcher.InvokeAsync(() => column.Items.Insert(0, value));
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
