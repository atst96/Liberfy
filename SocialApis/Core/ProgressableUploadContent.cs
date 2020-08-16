using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialApis.Core
{
    internal class ProgressableUploadContent : HttpContent
    {
        // 初期バッファサイズ = 128KB
        private const int DefaultBufferSize = 1024 * 128;

        private long _actualSize = 0;
        private long _uploadSize = 0;
        private long _totalLength = 0;
        private readonly Stream _content;
        private readonly int _bufferSize;
        private bool _contentConsumed;
        private readonly IProgress<UploadReport> _progressReceiver;

        public ProgressableUploadContent(Stream content, int? bufferSize = null, IProgress<UploadReport> progressReceiver = null)
        {
            this._content = content;
            this._actualSize = 0;
            this._totalLength = content.Length;
            this._uploadSize = this._totalLength;
            this._bufferSize = bufferSize ?? DefaultBufferSize;
            this._progressReceiver = progressReceiver;
            this.PrepareContent();
        }

        public ProgressableUploadContent(Stream content, long? actualSize, long? sendSize, long? totalLength, int? bufferSize = default, IProgress<UploadReport> progressReceiver = null)
        {
            this._content = content;
            this._actualSize = actualSize ?? 0;
            this._totalLength = totalLength ?? -1;
            this._uploadSize = sendSize ?? -1;
            this._bufferSize = bufferSize ?? DefaultBufferSize;
            this._progressReceiver = progressReceiver;
            this.PrepareContent();
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);

            return Task.Run(() =>
            {
                var content = this._content;
                byte[] buffer = new byte[this._bufferSize];

                int sendSize = this._bufferSize;
                long totalSendSize = this._uploadSize;
                long uploaded = 0;

                while (uploaded < totalSendSize)
                {
                    if (totalSendSize < (uploaded + sendSize))
                    {
                        sendSize = (int)(totalSendSize - uploaded);
                    }

                    int length = this._content.Read(buffer, 0, sendSize);
                    if (length <= 0)
                    {
                        break;
                    }

                    stream.Write(buffer, 0, sendSize);

                    uploaded += length;
                    this._actualSize += length;

                    this._progressReceiver.Report(UploadReport.CreateProcessing(this._actualSize, this._totalLength));
                }
            });
        }

        private void PrepareContent()
        {
            if (this._contentConsumed)
            {
                return;
            }

            this._contentConsumed = true;

            var content = this._content;
            if (!content.CanSeek)
            {
                throw new InvalidOperationException();
            }

            content.Position = 0;

            if (this._totalLength < 0)
            {
                this._totalLength = this._content.Length;
            }

            if (this._uploadSize < 0)
            {
                this._uploadSize = this._totalLength;
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = this._totalLength;
            return length >= 0;
        }
    }
}
