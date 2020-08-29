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

        private async Task<IMastodonStreamClient> StartStreaming(StreamType streamType, string streamParam)
        {
            var client = new MastodonStreamClient(this.Api, streamType, streamParam);

            await client.Connect().ConfigureAwait(false);

            return client;
        }

        public Task<IMastodonStreamClient> User()
            => this.StartStreaming(StreamType.User, null);

        public Task<IMastodonStreamClient> Public()
            => this.StartStreaming(StreamType.Public, null);

        public Task<IMastodonStreamClient> Local()
            => this.StartStreaming(StreamType.PublicLocal, null);

        public Task<IMastodonStreamClient> Hashtag(string tag)
            => this.StartStreaming(StreamType.Hashtag, tag);

        public Task<IMastodonStreamClient> HashtagLocal(string tag)
            => this.StartStreaming(StreamType.HashtagLocal, tag);

        public Task<IMastodonStreamClient> List(long listId)
            => this.StartStreaming(StreamType.List, listId.ToString());

        public Task<IMastodonStreamClient> Direct()
            => this.StartStreaming(StreamType.Direct, null);
    }
}
