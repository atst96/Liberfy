using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public interface IMastodonStreamResolver
    {
        void OnStreamingUpdate(Status status);
        void OnStreamingNotification(Notification notification);
        void OnStreamingDelete(long id);
        void OnStreamingFilterChanged();
    }
}
