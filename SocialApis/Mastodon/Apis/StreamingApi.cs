using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon.Apis
{
    public class StreamingApi : ApiBase
    {
        internal StreamingApi(MastodonApi tokens) : base(tokens)
        {
        }

        private async Task<IDisposable> StartStreaming(string path, Query query, IMastodonStreamResolver streamingResolver)
        {
            var request = this.Api.CreateRestApiGetRequest(path, query);

            try
            {
                var response = await request.GetResponseAsync();
                var receiver = new MastodonHttpStreamingReceiver(response, streamingResolver);

                receiver.Start();

                return receiver;
            }
            catch (WebException wex)
            {
                throw MastodonException.FromWebException(wex);
            }
        }

        public Task<IDisposable> User(IMastodonStreamResolver streamingResolver)
        {
            return this.StartStreaming("streaming/user", null, streamingResolver);
        }

        public Task<IDisposable> Public(IMastodonStreamResolver streamingResolver)
        {
            return this.StartStreaming("streaming/public", null, streamingResolver);
        }

        public Task<IDisposable> Local(IMastodonStreamResolver streamingResolver)
        {
            return this.StartStreaming("streaming/public/local", null, streamingResolver);
        }

        public Task<IDisposable> Hashtag(string tag, IMastodonStreamResolver streamingResolver)
        {
            var query = new Query { ["tag"] = tag };

            return this.StartStreaming("streaming/hashtag", query, streamingResolver);
        }

        public Task<IDisposable> List(long listId, IMastodonStreamResolver streamingResolver)
        {
            var query = new Query { ["list"] = listId };

            return this.StartStreaming("streaming/list", query, streamingResolver);
        }

        public Task<IDisposable> Direct(IMastodonStreamResolver streamingResolver)
        {
            return this.StartStreaming("streaming/direct", null, streamingResolver);
        }
    }
}
