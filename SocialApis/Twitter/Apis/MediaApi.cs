using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Core;

namespace SocialApis.Twitter.Apis
{
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class MediaApi : ApiBase
    {
        internal MediaApi(TwitterApi tokens) : base(tokens) { }

        public Task<MediaResponse> Upload(string filename, long[] additionalOwners = null)
        {
            return this.Upload(File.OpenRead(filename), additionalOwners);
        }

        private static readonly Uri _apiEndpoint = new Uri("https://upload.twitter.com/1.1/media/upload.json");

        public async Task<MediaResponse> Upload(Stream contentStream, long[] additionalOwners = null, IProgress<UploadReport> progressReceiver = null)
        {
            long contentLength = contentStream.Length;

            progressReceiver?.Report(UploadReport.CreateBegin(contentLength));

            var bondary = OAuthHelper.GenerateNonce();
            using var content = new MultipartFormDataContent(bondary);

            if (additionalOwners?.Length > 0)
            {
                var joinedAdditionalOwners = string.Join(",", additionalOwners);
                content.Add(new StringContent(joinedAdditionalOwners), "additional_owners");
            }

            content.Add(new ProgressableUploadContent(contentStream, progressReceiver: progressReceiver), "media");

            var result = await this.Api.ApiPostRequestAsync<MediaResponse>(_apiEndpoint, content);

            progressReceiver?.Report(UploadReport.CreateCompleted(contentLength));

            return result;
        }

        /// <param name="uploadData"></param>
        /// <param name="mediaType"></param>
        /// <param name="additionalOwners"></param>
        /// <param name="progressReceiver"></param>
        /// <param name="segmentSize"></param>
        /// <returns></returns>
        public async Task<MediaResponse> ChunkedUpload(Stream uploadData, string mediaType, long[] additionalOwners = null,
            IProgress<UploadReport> progressReceiver = null, int segmentSize = 1024 * 1024 * 1)
        {
            if (segmentSize <= 0)
            {
                throw new ArgumentException($"{nameof(segmentSize)}には0より大きい値を指定してください。");
            }

            if (uploadData.CanSeek)
            {
                uploadData.Position = 0;
            }

            long fileLength = uploadData.Length;
            if (fileLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(uploadData));
            }

            // INITコマンド
            var initResposne = await this.ChunkedUploadInit(fileLength, mediaType, additionalOwners)
                .ConfigureAwait(false);

            long mediaId = initResposne.MediaId;

            // APPENDコマンド
            // アップロード開始
            progressReceiver?.Report(UploadReport.CreateBegin(fileLength));

            long chunkSize = segmentSize;
            long actualSize = 0;
            int index = 0;

            while (actualSize < fileLength)
            {
                if (fileLength < (actualSize + chunkSize))
                {
                    chunkSize = fileLength - actualSize;
                }

                var res = await this.ChunkedUploadAppendImpl(mediaId, uploadData, chunkSize, actualSize, fileLength, index, progressReceiver)
                    .ConfigureAwait(false);
                if (!string.IsNullOrEmpty(res))
                {
                    throw new TwitterException(res);
                }

                actualSize += chunkSize;
                ++index;
            }

            // FINALIZEコマンド
            var response = await this.ChunkedUploadFinalize(mediaId)
                .ConfigureAwait(false);

            progressReceiver?.Report(UploadReport.CreateCompleted(fileLength));

            return response;
        }

        public Task<MediaResponse> ChunkedUploadInit(long totalBytes, string mediaType, long[] additionalOwners = null)
        {
            var query = new Query
            {
                ["command"] = "INIT",
                ["total_bytes"] = totalBytes,
                ["media_type"] = mediaType,
            };

            if (additionalOwners != null)
            {
                query["additional_owners"] = additionalOwners;
            }

            return this.Api.ApiPostRequestAsync<MediaResponse>(_apiEndpoint, query);
        }

        public async Task<string> ChunkedUploadAppend(long mediaId, Stream media, long sendSize, long actualSize, long totalLength, int index, IProgress<UploadReport> progressReceiver)
        {
            long dataLength = media.Length;

            progressReceiver?.Report(UploadReport.CreateBegin(dataLength));

            var result = await this.ChunkedUploadAppendImpl(mediaId, media, sendSize, actualSize, totalLength, index, progressReceiver).ConfigureAwait(false);

            progressReceiver?.Report(UploadReport.CreateCompleted(dataLength));

            return result;
        }

        private async Task<string> ChunkedUploadAppendImpl(long mediaId, Stream media, long sendSize, long actualSize, long totalLength, int index, IProgress<UploadReport> progressReceiver)
        {
            var boundary = OAuthHelper.GenerateNonce();

            // 送信データの準備
            using var content = new MultipartFormDataContent(boundary);

            content.Add(new StringContent("APPEND"), "command");
            content.Add(new StringContent(mediaId.ToString()), "media_id");
            content.Add(new StringContent(index.ToString()), "segment_index");
            content.Add(new ProgressableUploadContent(media, actualSize, sendSize, totalLength, progressReceiver: progressReceiver), "media");

            return await this.Api.ApiPostRequestAsync<string>(_apiEndpoint, content)
                .ConfigureAwait(false);
        }

        public Task<UploadMediaInfo> ChunkedUploadStatus(long mediaId)
            => this.Api.ApiGetRequestAsync<UploadMediaInfo>(_apiEndpoint, new Query
            {
                ["command"] = "STATUS",
                ["media_id"] = mediaId,
            });

        public Task<MediaResponse> ChunkedUploadFinalize(long mediaId)
            => this.Api.ApiPostRequestAsync<MediaResponse>(_apiEndpoint, new Query
            {
                ["command"] = "FINALIZE",
                ["media_id"] = mediaId,
            });
    }
}
