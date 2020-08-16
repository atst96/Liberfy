using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SocialApis.Utils;

namespace SocialApis.Mastodon
{
    internal class MastodonHttpStreamingReceiver : IDisposable
    {
        private bool _isClosing = false;
        private HttpResponseMessage _response;
        private IMastodonStreamResolver _streamResolver;

        internal MastodonHttpStreamingReceiver(HttpResponseMessage response, IMastodonStreamResolver resolver)
        {
            this._response = response;
            this._streamResolver = resolver;
        }

        internal void Start()
        {
            Task.Run(async () =>
            {
                var res = this._response;

                using var stream = await res.Content.ReadAsStreamAsync().ConfigureAwait(false);
                using var reader = new StreamReader(stream, EncodingUtil.UTF8);

                string line;

                do
                {
                    line = reader.ReadLine();

                    if (string.IsNullOrEmpty(line) || !line.StartsWith("event:"))
                    {
                        continue;
                    }

                    var eventName = line.Substring(7);

                    if ((line = reader.ReadLine()) == null)
                    {
                        break;
                    }

                    var data = line.Substring(6);

                    switch (eventName)
                    {
                        case "update":
                            var status = JsonUtil.Deserialize<Status>(data);
                            this._streamResolver.OnStreamingUpdate(status);
                            break;

                        case "notification":
                            var notification = JsonUtil.Deserialize<Notification>(data);
                            this._streamResolver.OnStreamingNotification(notification);
                            break;

                        case "delete":
                            this._streamResolver.OnStreamingDelete(long.Parse(data));
                            break;

                        case "filter_changed":
                            break;
                    }

                } while (line != null && !this._isClosing);
            });
        }

        public void Dispose()
        {
            this._response?.Dispose();
            this._isClosing = true;
        }
    }
}
