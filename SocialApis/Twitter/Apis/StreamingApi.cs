using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class StreamingApi : TokenApiBase
    {
        internal StreamingApi(Tokens tokens) : base(tokens) { }

        public async Task User(IObserver<IStreamResponse> observer, Query query = null)
        {
            const string url = "https://userstream.twitter.com/1.1/user.json";

            var req = Tokens.CreateGetRequester(url, query);
            await req.GetResponseAsync().ContinueWith(t =>
            {
                if (t.IsFaulted && t.Exception != null)
                {
                    observer.OnError(t.Exception);
                }
                else
                {
                    var res = t.Result;
                    new StreamingReceiver(res.GetResponseStream(), observer).Start();
                }
            })
            .ContinueWith(t => t.Dispose());
        }
    }
}
