using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class MastodonConfig : NotificationObject, ISocialConfig
    {
        public string Name { get; } = "Mastodon";

        public bool IsVariableDomain { get; } = true;

        public string ClientKeyName { get; } = "ConsumerKey";

        public string ClientSecretKeyName { get; } = "ConsumerSecret";
    }
}
