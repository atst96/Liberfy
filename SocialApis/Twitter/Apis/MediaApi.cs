using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace SocialApis.Twitter.Apis
{
    public class MediaApi : TokenApiBase
    {
        internal MediaApi(Tokens tokens) : base(tokens) { }

        public Task<MediaResponse> Upload(string filename, long[] additionalOwners = null)
        {
            return this.Upload(File.OpenRead(filename), additionalOwners);
        }

        private const string _apiEndpoint = "https://upload.twitter.com/1.1/media/upload.json";

        private static HttpWebRequest CreateRequester(Tokens tokens, out string boundary)
        {
            var req = tokens.CreatePostRequester(_apiEndpoint, null, false);
            boundary = OAuthHelper.GenerateNonce();

            req.ContentType = $"multipart/form-data; boundary={ boundary }";

            return req;
        }

        public async Task<MediaResponse> Upload(Stream stream, long[] additionalOwners = null)
        {
            var req = CreateRequester(this.Tokens, out var boundary);

            using (var reqStr = await req.GetRequestStreamAsync())
            using (var writer = new StreamWriter(reqStr, Encoding.UTF8))
            {
                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"media\"");
                writer.WriteLine();
                writer.Flush();

                await stream.CopyToAsync(reqStr);

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

            return await this.Tokens.SendRequest<MediaResponse>(req);
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

            return this.Tokens.PostRequestAsync<MediaResponse>(_apiEndpoint, query);
        }

        public async Task<string> ChunkedUploadAppend(long mediaId, byte[] media, long segmentIndex)
        {
            var req = CreateRequester(this.Tokens, out var boundary);

            using (var reqStr = await req.GetRequestStreamAsync())
            using (var writer = new StreamWriter(reqStr))
            {
                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"command\"");
                writer.WriteLine();
                writer.WriteLine("APPEND");

                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"media_id\"");
                writer.WriteLine();
                writer.WriteLine(mediaId);

                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"segment_index\"");
                writer.WriteLine();
                writer.WriteLine(segmentIndex);

                writer.WriteLine($"--{ boundary }");
                writer.WriteLine("Content-Disposition: form-data; name=\"media\"");
                writer.WriteLine();
                writer.Flush();

                await reqStr.WriteAsync(media, 0, media.Length);

                writer.WriteLine();

                writer.WriteLine($"--{ boundary }--");

                writer.Flush();
            }

            try
            {
                using (var webRes = await req.GetResponseAsync())
                using (var sr = new StreamReader(webRes.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException wex) when (wex.Response != null)
            {
                var response = wex.Response.GetResponseStream();

                var errors = JsonSerializer.Deserialize<TwitterErrorContainer>(response, Utf8Json.Resolvers.StandardResolver.AllowPrivate);
                throw new TwitterException(wex, errors);
            }
        }

        public Task<UploadMediaInfo> ChunkedUploadStatus(long mediaId)
        {
            return this.Tokens.PostRequestAsync<UploadMediaInfo>(_apiEndpoint, new Query
            {
                ["command"] = "STATUS",
                ["media_id"] = mediaId,
            });
        }

        public Task<UploadMediaInfo> ChunkedUploadFinalize(long mediaId)
        {
            return this.Tokens.PostRequestAsync<UploadMediaInfo>(_apiEndpoint, new Query
            {
                ["command"] = "FINALIZE",
                ["media_id"] = mediaId,
            });
        }
    }
}
