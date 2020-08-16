using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
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
            using var request = this.Api.CreateRestApiGetRequest(path, query);
            using var httpClient = new HttpClient();

            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);

            var receiver = new MastodonHttpStreamingReceiver(response, streamingResolver);
            receiver.Start();

            return receiver;
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
