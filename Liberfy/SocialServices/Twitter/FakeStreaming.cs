﻿using SocialApis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Liberfy.SocialServices.Twitter
{
    internal class FakeStreaming : IUnsubscribableObserver<IItem>
    {
        private TwitterAccount _account;

        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
        public long LatestHomeStatusId { get; set; }

        private CancellationTokenSource _cancellationTokenSource;

        private bool IsCancelRequested => this._cancellationTokenSource.IsCancellationRequested;

        public FakeStreaming(TwitterAccount account)
        {
            this._account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public async Task Start()
        {
            this._cancellationTokenSource = new CancellationTokenSource();

            await Task.Delay(this.Interval, this._cancellationTokenSource.Token);

            var sw = new Stopwatch();

            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    sw.Restart();

                    var statuses = await this._account.InternalTokens.Statuses.HomeTimeline(new Query
                    {
                        ["since_id"] = this.LatestHomeStatusId,
                        ["count"] = 200,
                    });

                    var enumerator = statuses.Reverse().GetEnumerator();


                    bool hasNext = enumerator.MoveNext();
                    var currentStatus = hasNext ? enumerator.Current : default;

                    while (hasNext && !this.IsCancelRequested)
                    {
                        var timelineItem = new StatusItem(currentStatus, this._account);
                        this.LatestHomeStatusId = currentStatus.Id;

                        foreach (var observer in this._observers)
                        {
                            observer.OnNext(timelineItem);
                        }

                        hasNext = !this.IsCancelRequested && enumerator.MoveNext();
                        if (hasNext)
                        {
                            var nextStatus = enumerator.Current;
                            var delay = nextStatus.CreatedAt - currentStatus.CreatedAt;
                            await Task.Delay(delay, this._cancellationTokenSource.Token);

                            currentStatus = nextStatus;
                        }
                    }

                    sw.Stop();

                    var remaining = this.Interval - sw.Elapsed;
                    if (sw.Elapsed.TotalSeconds > 0)
                    {
                        await Task.Delay(remaining);
                    }
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            if (this._cancellationTokenSource != null)
            {
                if (!this.IsCancelRequested)
                    this._cancellationTokenSource.Cancel();

                foreach (var observer in this._observers)
                {
                    observer.OnCompleted();
                }
            }

            this._cancellationTokenSource = null;
        }

        private List<IObserver<IItem>> _observers = new List<IObserver<IItem>>();

        public IDisposable Subscribe(IObserver<IItem> observer)
        {
            this._observers.Add(observer);

            return new ObservableUnsubscriber<IItem>(observer, this);
        }

        public void Unsubscribe(IObserver<IItem> observer)
        {
            this._observers.Remove(observer);
        }
    }
}
