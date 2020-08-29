using SocialApis.Mastodon;
using SocialApis.Mastodon.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonTimeline : TimelineBase, IObserver<StreamResponse>
    {
        private readonly static Dispatcher _dispatcher = App.Current.Dispatcher;

        private readonly long _userId;
        private readonly MastodonAccount _account;
        public MastodonApi _tokens => _account.Api;

        private IMastodonStreamClient _streamClient;
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
                this.LoadNotificationTimeline(),
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            this._streamClient = await this._tokens.Streaming.User();
            this._streamDisposer = this._streamClient.Subscribe(this);

            // this.LoadMessageTimeline(),
        }

        public override void Unload()
        {
            this._streamClient?.Dispose();
            this.AccountColumns.Clear();
        }

        private IEnumerable<StatusItem> GetStatusItem(IEnumerable<Status> statuses)
        {
            foreach (var status in statuses)
            {
                yield return new StatusItem(status, this._account);
            }
        }

        private IEnumerable<IItem> GetItem(IEnumerable<Notification> notifications)
        {
            foreach (var notification in notifications)
            {
                if (notification.Type == NotificationTypes.Mention)
                {
                    yield return new StatusItem(notification.Status, this._account);
                }
                else
                {
                    yield return new NotificationItem(notification, this._account);
                }
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

        private async Task LoadNotificationTimeline()
        {
            try
            {
                var statuses = await this._tokens.Notifications.GetNotifications().ConfigureAwait(false);
                var items = this.GetItem(statuses);

                foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Notification))
                {
                    _dispatcher.Invoke(() => column.Items.Reset(items));
                }
            }
            finally { }
        }

        public void OnStreamingUpdate(Status status)
        {
            var item = new StatusItem(status, this._account);

            foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Home))
            {
                column.Items.Insert(0, item);
            }
        }

        public void OnStreamingNotification(Notification notification)
        {
            //var item = new StatusItem(status, this._account);

            //foreach (var column in this.AccountColumns.GetsTypeOf(ColumnType.Notification))
            //{
            //    column.Items.Insert(0, item);
            //}
        }

        public void OnStreamingDelete(long id)
        {
        }

        public void OnStreamingFilterChanged()
        {
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(StreamResponse value)
        {
            _dispatcher.InvokeAsync(() =>
            {
                switch (value)
                {
                    case StreamStatusResponse statusResposne:
                        this.OnStreamingUpdate(statusResposne.Status);
                        break;

                    case StreamNotificationResponse notificationResponse:
                        this.OnStreamingNotification(notificationResponse.Notification);
                        break;

                    case StreamDeleteResponse deleteResposne:
                        this.OnStreamingDelete(deleteResposne.StatusId);
                        break;

                    case StreamFiltersChangedResponse filtersChangedResponse:
                        this.OnStreamingFilterChanged();
                        break;

                    default:
                        break;
                }
            });
        }

        ~MastodonTimeline()
        {
            this._streamDisposer?.Dispose();
            this._streamClient?.Dispose();
        }
    }
}
