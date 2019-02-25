using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    internal class MastodonHttpStreamingReceiver : IDisposable
    {
        private bool _isClosing = false;
        private WebResponse _response;
        private IMastodonStreamResolver _streamResolver;

        internal MastodonHttpStreamingReceiver(WebResponse response, IMastodonStreamResolver resolver)
        {
            this._response = response;
            this._streamResolver = resolver;
        }

        internal void Start()
        {
            Task.Run(() =>
            {
                var res = this._response;

                using (var reader = new StreamReader(res.GetResponseStream(), WebUtility.UTF8Encoding))
                {
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
                                var status = JsonUtility.Deserialize<Status>(data);
                                this._streamResolver.OnStreamingUpdate(status);
                                break;

                            case "notification":
                                var notification = JsonUtility.Deserialize<Notification>(data);
                                this._streamResolver.OnStreamingNotification(notification);
                                break;

                            case "delete":
                                this._streamResolver.OnStreamingDelete(long.Parse(data));
                                break;

                            case "filter_changed":
                                break;
                        }

                    } while (line != null && !this._isClosing);
                }
            });
        }

        public void Dispose()
        {
            this._response.Close();
            this._isClosing = true;
        }
    }
}
