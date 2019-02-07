using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

    public class MediaApi : ApiBase
    {
        internal MediaApi(TwitterApi tokens) : base(tokens) { }

        public Task<MediaResponse> Upload(string filename, long[] additionalOwners = null)
        {
            return this.Upload(File.OpenRead(filename), additionalOwners);
        }

        private const string _apiEndpoint = "https://upload.twitter.com/1.1/media/upload.json";

        private static HttpWebRequest CreateMultipartRequester(TwitterApi tokens, out string boundary)
        {
            var req = tokens.CreatePostRequester(_apiEndpoint, null, false);
            boundary = OAuthHelper.GenerateNonce();

            req.ContentType = $"multipart/form-data; boundary={ boundary }";

            return req;
        }

        public async Task<MediaResponse> Upload(Stream contentStream, long[] additionalOwners = null)
        {
            var request = CreateMultipartRequester(this.Api, out var boundary);

            using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
            using (var writer = new StreamWriter(requestStream, Encoding.UTF8))
            {
                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"media\"");
                writer.WriteLine();
                writer.Flush();

                await contentStream.CopyToAsync(requestStream).ConfigureAwait(false);

                writer.WriteLine();

                if (additionalOwners != null)
                {
                    writer.WriteLine($"--{ boundary }");
                    writer.WriteLine("Content-Disposition: form-data; name=\"additional_owners\"");
                    writer.WriteLine();
                    writer.WriteLine(string.Join(",", additionalOwners));
                }

                writer.WriteLine($"--{ boundary }--");

                writer.Flush();
            }

            return await this.Api.SendRequest<MediaResponse>(request).ConfigureAwait(false);
        }

        public async Task<MediaResponse> ChunkedUpload(Stream media, string mediaType, long[] additionalOwners = null, IProgress<UploadProgress> progressReceiver = null)
        {
            var initResposne = await this.ChunkedUploadInit(media.Length, mediaType, additionalOwners).ConfigureAwait(false);

            // 1MB
            const int SegmentSize = 1024 * 1024 * 1; // 1MiB

            int fileLength = (int)media.Length;
            int segmentsCount = (fileLength + SegmentSize - 1) / SegmentSize;

            UploadProgress progress = default;
            if (progressReceiver != null)
            {
                progress = new UploadProgress()
                {
                    TotalSize = fileLength,
                };

                progressReceiver.Report(progress);
            }

            if (media.Position != 0 && media.CanSeek)
                media.Position = 0;

            int dataRemaining = fileLength;

            int dataSize = SegmentSize;

            for (int segmentIndex = 0; dataRemaining > 0; ++segmentIndex)
            {
                if (SegmentSize > dataRemaining)
                    dataSize = dataRemaining;

                var res = await this.ChunkedUploadAppend(initResposne.MediaId, media, dataSize, segmentIndex, progressReceiver, progress).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(res))
                {
                    throw new TwitterException(res);
                }

                dataRemaining -= dataSize;
            }

            return await this.ChunkedUploadFinalize(initResposne.MediaId).ConfigureAwait(false);
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
                query["additional_owners"] = additionalOwners;

            return this.Api.PostRequestAsync<MediaResponse>(_apiEndpoint, query);
        }

        public async Task<string> ChunkedUploadAppend(long mediaId, Stream media, int segmentSize, long segmentIndex, IProgress<UploadProgress> progressReceiver, UploadProgress progress)
        {
            var request = CreateMultipartRequester(this.Api, out var boundary);

            // 送信データの準備
            using (var dataStr = new MemoryStream())
            {
                var writer = new BoundaryWriter(dataStr, boundary);
                writer.WriteDisposition("command", "APPEND");
                writer.WriteDisposition("media_id", mediaId);
                writer.WriteDisposition("segment_index", segmentIndex);
                writer.WriteDispositionBinary("media", media, segmentSize);
                writer.CloseBoundary();
                writer.Flush();

                request.ContentLength = dataStr.Length;
                request.SendChunked = false;
                request.AllowWriteStreamBuffering = false;

                // 128KB ずつコピー
                const int SendBufferLength = 1024 * 128;

                int dataSize = SendBufferLength;
                int dataRemaining = (int)dataStr.Length;

                dataStr.Position = 0;

                byte[] data = new byte[SendBufferLength];

                using (var stream = request.GetRequestStream())
                {
                    dataStr.Position = 0;

                    int uploadedSize = progress.UploadedSize;

                    while (dataRemaining > 0)
                    {
                        if (SendBufferLength > dataRemaining)
                            dataSize = dataRemaining;

                        dataStr.Read(data, 0, dataSize);
                        stream.Write(data, 0, dataSize);

                        dataRemaining -= dataSize;

                        if (progressReceiver != null)
                        {
                            progress.UploadedSize = uploadedSize + (int)((1 - (dataRemaining / (double)dataStr.Length)) * segmentSize);
                            progress.UploadPercentage = progress.UploadedSize / (double)progress.TotalSize;
                            progressReceiver.Report(progress);
                        }
                    }
                }

                data = null;

                writer.Close();
            }

            try
            {
                using (var response = await request.GetResponseAsync().ConfigureAwait(false))
                {
                    return await StreamUtility.ReadToEndAsync(response.GetResponseStream()).ConfigureAwait(false);
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                var response = wex.Response.GetResponseStream();

                var errors = await JsonUtility.DeserializeAsync<TwitterErrorContainer>(response).ConfigureAwait(false);
                throw new TwitterException(wex, errors);
            }
        }

        public Task<UploadMediaInfo> ChunkedUploadStatus(long mediaId)
        {
            return this.Api.GetRequestAsync<UploadMediaInfo>(_apiEndpoint, new Query
            {
                ["command"] = "STATUS",
                ["media_id"] = mediaId,
            });
        }

        public Task<MediaResponse> ChunkedUploadFinalize(long mediaId)
        {
            return this.Api.PostRequestAsync<MediaResponse>(_apiEndpoint, new Query
            {
                ["command"] = "FINALIZE",
                ["media_id"] = mediaId,
            });
        }
    }
}
