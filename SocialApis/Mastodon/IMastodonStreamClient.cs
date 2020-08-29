using System;
using System.Collections.Generic;
using System.Text;
using SocialApis.Core;

namespace SocialApis.Mastodon
{
    public interface IMastodonStreamClient : IWebSocketClient, IObservable<StreamResponse>
    {
    }
}
