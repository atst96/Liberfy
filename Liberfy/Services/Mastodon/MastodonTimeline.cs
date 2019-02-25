using SocialApis.Mastodon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    internal class MastodonTimeline : TimelineBase, IMastodonStreamResolver
    {
        private readonly static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly MastodonAccount _account;
        public MastodonApi _tokens => _account.Tokens;

        private IDisposable _streamDisposer;

        public MastodonTimeline(MastodonAccount account)
            : base(account)
        {
            this._account = account;
            this._userId = account.Id;
        }

        public override async void Load()
        {
            Task[] tasks =
            {
                this.LoadHomeTimeline(),
                // this.LoadNotificationTimeline(),
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            this._streamDisposer = this._tokens.Streaming.User(this);

            // this.LoadMessageTimeline(),
        }

        public override void Unload()
        {
            this._streamDisposer?.Dispose();
            this.AccountColumns.Clear();
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses)
            {
                yield return new StatusItem(status, this._account);
            }
        }

        private async Task LoadHomeTimeline()
        {
            try
            {
                var statuses = await this._tokens.Timelines.Home().ConfigureAwait(false);
                var items = this.GetStatusItem(statuses);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Home))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            finally { }
        }

        public void OnStreamingUpdate(Status status)
        {
            _dispatcher.InvokeAsync(() =>
            {
                var item = new StatusItem(status, this._account);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Home))
                {
                    column.Items.Insert(0, item);
                }
            });
        }

        public void OnStreamingNotification(Notification notification)
        {
            //_dispatcher.InvokeAsync(() =>
            //{
            //    var item = new StatusItem(status, this._account);

            //    foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Notification))
            //    {
            //        column.Items.Insert(0, item);
            //    }
            //});
        }

        public void OnStreamingDelete(long id)
        {
        }

        public void OnStreamingFilterChanged()
        {
        }
    }
}
