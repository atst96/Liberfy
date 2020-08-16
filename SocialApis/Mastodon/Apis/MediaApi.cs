using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Core;
using SocialApis.Utils;

namespace SocialApis.Mastodon.Apis
{
    public class MediaApi : ApiBase
    {
        internal MediaApi(MastodonApi tokens) : base(tokens)
        {
        }

        public async Task<Attachment> Upload(Stream file, string description = null, float[] focus = null, IProgress<UploadReport> progress = null)
        {
            if (focus != null && focus.Length != 2)
            {
                throw new ArgumentException(nameof(focus));
            }

            var boundary = OAuthHelper.GenerateNonce();
            using var formData = new MultipartFormDataContent(boundary);

            using var request = this.Api.CreateCustomRestApiRequest(HttpMethod.Post, "media");

            if (description?.Length > 0)
            {
                formData.Add(new StringContent(description, EncodingUtil.UTF8), "description");
            }

            if (focus != null)
            {
                var focusContent = string.Join(",", focus);
                formData.Add(new StringContent(focusContent, EncodingUtil.UTF8), "focus");
            }

            formData.Add(new ProgressableUploadContent(file, progressReceiver: progress), "file", "image");

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
