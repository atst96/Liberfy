using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApis.Core
{
    /// <summary>
    /// WebSocketクライアント
    /// </summary>
    internal abstract class WebSocketClient : IWebSocketClient, IDisposable
    {
        /// <summary>
        /// データ受信時のバッファサイズ
        /// </summary>
        private const int BufferSize = 2048;

        private CancellationToken _cancellationToken;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// WebSocketクライアント
        /// </summary>
        protected ClientWebSocket Connection { get; }


        /// <summary>
        /// 接続中かどうか取得する。
        /// </summary>
        public bool IsConnecting { get; private set; } = false;

        /// <summary>
        /// <see cref="WebSocketClient"/>を生成する。
        /// </summary>
        protected WebSocketClient()
        {
            this.Connection = WebFactory.CreateWebSocketClient();
        }

        /// <summary>
        /// 接続を開始する。
        /// </summary>
        /// <returns></returns>
        public Task Connect() => this.Connect(CancellationToken.None);

        /// <summary>
        /// 接続を開始する。
        /// </summary>
        public async Task Connect(CancellationToken cancellationToken)
        {
            var sem = this._semaphore;
            await sem.WaitAsync().ConfigureAwait(false);

            if (this.IsConnecting)
            {
                throw new InvalidOperationException();
            }

            this.IsConnecting = true;
            this._cancellationToken = cancellationToken;

            sem.Release();

            var result = await this.TryConnect(cancellationToken).ConfigureAwait(false);
            if (!result)
            {
                return;
            }

            this.StartPooling(cancellationToken);
        }

        /// <summary>
        /// 接続を開始する
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract Task<bool> TryConnect(CancellationToken token);

        /// <summary>
        /// 受信処理を開始する
        /// </summary>
        /// <returns></returns>
        private void StartPooling(CancellationToken cancellationToken)
        {
            var buffer = new Memory<byte>(new byte[BufferSize]);
            var wss = this.Connection;

            Task.Run(async () =>
            {
                try
                {
                    while (this.Connection.State == WebSocketState.Open)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var dataCollection = new LinkedList<byte[]>();
                        int destLength = 0;

                        ValueWebSocketReceiveResult result;
                        do
                        {
                            result = await wss.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await this.Close().ConfigureAwait(false);
                                return;
                            }

                            int dataCount = result.Count;
                            dataCollection.AddLast(ToArray(buffer, dataCount));
                            destLength += dataCount;
                        }
                        while (!result.EndOfMessage);

                        cancellationToken.ThrowIfCancellationRequested();

                        byte[] dest = ConcatBytes(dataCollection, destLength);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Text:
                                this.OnReceiveText(dest);
                                break;
                            case WebSocketMessageType.Binary:
                                this.OnReceiveBinary(dest);
                                break;
                        }
                    }
                }
                catch (TaskCanceledException tcex)
                {
                    // TODO: LOG
                    await this.Close().ConfigureAwait(false);
                }
            });
        }

        /// <summary>
        /// 接続を終了する。
        /// </summary>
        /// <returns></returns>
        public async Task Close()
        {
            var wss = this.Connection;
            var sem = this._semaphore;

            await sem.WaitAsync().ConfigureAwait(false);
            if (!this.IsConnecting)
            {
                return;
            }

            await wss.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None)
                .ConfigureAwait(false);

            this.IsConnecting = false;

            sem.Release();

            this.OnClosed();
        }

        /// <summary>
        /// テキストデータ受取時
        /// </summary>
        /// <param name="data">受信したデータ</param>
        protected virtual void OnReceiveText(byte[] data)
        {
        }

        /// <summary>
        /// バイナリデータ受取時
        /// </summary>
        /// <param name="data">受信したデータ</param>
        protected virtual void OnReceiveBinary(byte[] data)
        {
        }

        /// <summary>
        /// 接続終了時
        /// </summary>
        protected virtual void OnClosed()
        {
        }

        /// <summary>
        /// 複数のバイト配列を結合する。
        /// </summary>
        /// <param name="dataCollection">バイト配列</param>
        /// <param name="arrayLength">コピー先配列のバイト数</param>
        /// <returns></returns>
        private static byte[] ConcatBytes(IReadOnlyCollection<byte[]> dataCollection, int arrayLength)
        {
            byte[] dest = new byte[arrayLength];
            int offset = 0;

            foreach (var src in dataCollection)
            {
                Buffer.BlockCopy(src, 0, dest, offset, src.Length);
                offset += src.Length;
            }

            return dest;
        }

        /// <summary>
        /// <see cref="Memory{T}"/>から配列をコピーする。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">コピー元データ</param>
        /// <param name="dataLength">コピーするデータ数</param>
        /// <returns></returns>
        private static T[] ToArray<T>(Memory<T> data, int dataLength)
        {
            if (data.Length < dataLength)
            {
                throw new ArgumentOutOfRangeException(nameof(dataLength));
            }

            if (dataLength == 0)
            {
                return Array.Empty<T>();
            }

            if (data.Length == dataLength)
            {
                return data.ToArray();
            }

            return data[0..dataLength].ToArray();
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        protected virtual void Dispose()
        {
            this.Connection.Dispose();
            this._semaphore.Dispose();
        }

        /// <summary>
        /// インスタンスを破棄する。
        /// </summary>
        void IDisposable.Dispose() => this.Dispose();
    }
}
