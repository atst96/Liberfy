using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SocialApis.Core;
using SocialApis.Extensions;
using SocialApis.Mastodon.Objects;
using SocialApis.Utils;

namespace SocialApis.Mastodon
{
    internal class MastodonStreamClient : WebSocketClient, IMastodonStreamClient
    {
        private readonly MastodonApi _api;
        private readonly StreamType _streamType;
        private readonly string _streamParam;
        private IList<IObserver<StreamResponse>> _observers = new List<IObserver<StreamResponse>>();

        internal MastodonStreamClient(MastodonApi api, StreamType streamType, string streamParam)
            : base()
        {
            this._api = api;
            this._streamType = streamType;
            this._streamParam = streamParam;
        }

        /// <summary>
        /// WebSocket通信のURLを取得する。
        /// </summary>
        /// <returns></returns>
        private Uri GetWebSocketUri()
        {
            var api = this._api;

            var hostName = this._api.HostUrl.Host;
            var parameters = new Dictionary<string, object>(3)
            {
                ["access_token"] = api.AccessToken,
                ["stream"] = this._streamType,
            };

            switch (this._streamType)
            {
                case StreamType.List:
                    parameters.Add("list", this._streamParam);
                    break;

                case StreamType.Hashtag:
                case StreamType.HashtagLocal:
                    parameters.Add("tag", this._streamParam);
                    break;
            }

            var url = $"wss://{ hostName }/api/v1/streaming?{ Query.JoinParametersWithAmpersand(parameters) }";
            return new Uri(url);
        }

        /// <summary>
        /// 接続を試行する。
        /// </summary>
        /// <param name="cacnellationToken"></param>
        /// <returns></returns>
        protected override async Task<bool> TryConnect(CancellationToken cacnellationToken)
        {
            var wss = this.Connection;

            try
            {
                await wss.ConnectAsync(this.GetWebSocketUri(), cacnellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException tcex)
            {
                // TODO: LOG
                return false;
            }

            return true;
        }

        /// <summary>
        /// バイナリデータ受信時
        /// </summary>
        /// <param name="data"></param>
        protected override void OnReceiveBinary(byte[] data)
        {
            var text = Encoding.UTF8.GetString(data);
            Console.WriteLine("Binary: " + text);
            Console.WriteLine();
        }

        /// <summary>
        /// テキストデータ受信時
        /// </summary>
        /// <param name="data"></param>
        protected override void OnReceiveText(byte[] data)
        {
            var response = JsonUtil.Deserialize<InternalStreamResponse>(data);

            switch (response.Event)
            {
                case StreamEventTypes.Update:
                    var status = JsonUtil.Deserialize<Status>(response.Payload);
                    this.OnUpdate(status);
                    break;

                case StreamEventTypes.Notification:
                    var notification = JsonUtil.Deserialize<Notification>(response.Payload);
                    this.OnNotification(notification);
                    break;

                case StreamEventTypes.Delete:
                    var statusId = long.Parse(response.Payload);
                    this.OnDelete(statusId);
                    break;

                case StreamEventTypes.FiltersChanged:
                    this.OnFiltersChanged();
                    break;

                default:
                    Debug.WriteLine($"Unkown stream response; type: { response.Event }, payload: { response.Payload }");
                    break;
            }
        }

        /// <summary>
        /// 購読登録する。
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<StreamResponse> observer)
        {
            var observers = this._observers;
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber<StreamResponse>(observer, observers);
        }

        private void OnUpdate(Status status)
            => this._observers.OnNext(new StreamStatusResponse(status));

        private void OnNotification(Notification notification)
            => this._observers.OnNext(new StreamNotificationResponse(notification));

        private void OnDelete(long statusId)
            => this._observers.OnNext(new StreamDeleteResponse(statusId));

        private void OnFiltersChanged()
            => this._observers.OnNext(new StreamFiltersChangedResponse());

        protected override void Dispose()
        {
            this._observers.Clear();

            base.Dispose();
        }
    }
}
