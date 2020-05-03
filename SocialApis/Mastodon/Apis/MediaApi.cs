using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class MediaApi : ApiBase
    {
        internal MediaApi(MastodonApi tokens) : base(tokens)
        {
        }

        public async Task<Attachment> Upload(Stream file, string description = null, float[] focus = null, IProgress<UploadProgress> progress = null)
        {
            if (focus != null && focus.Length != 2)
            {
                throw new ArgumentException(nameof(focus));
            }

            var boundary = OAuthHelper.GenerateNonce();

            var request = this.Api.CreateCustomRestApiRequest(HttpMethods.POST, "media");
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            using var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false);
            using var writer = new StreamWriter(requestStream, EncodingUtility.UTF8);

            writer.WriteLine("--" + boundary);
            writer.WriteLine(@"Content-Disposition: form-data; name=""file""; filename=""image""");
            writer.WriteLine();
            writer.Flush();

            await file.UploadCopyToAsync(requestStream, progress).ConfigureAwait(false);

            writer.WriteLine();

            if (description?.Length > 0)
            {
                writer.WriteLine("--" + boundary);
                writer.WriteLine(@"Content-Disposition: form-data; name=""description""");
                writer.WriteLine();
                writer.WriteLine(description);
            }

            if (focus != null)
            {
                writer.WriteLine("--" + boundary);
                writer.WriteLine(@"Content-Disposition: form-data; name=""focus""");
                writer.WriteLine();
                writer.WriteLine(string.Join(",", focus));
            }

            writer.WriteLine("--" + boundary + "--");

            writer.Flush();

            return await this.Api.SendRequest<Attachment>(request).ConfigureAwait(false);
        }

        public Task<Attachment> Edit(long mediaId, string description = null, float[] focus = null)
        {
            if (focus != null && focus.Length != 2)
            {
                throw new ArgumentException(nameof(focus));
            }

            var query = new Query();

            if (description != null)
            {
                query["description"] = description;
            }

            if (focus != null)
            {
                query["focus"] = string.Join(",", focus);
            }

            return this.Api.RestApiPutRequestAsync<Attachment>($"media/{ mediaId }", query);
        }
    }
}
