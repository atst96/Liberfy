using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class TwitterConfig : NotificationObject, ISocialConfig
    {
        public string Name { get; } = "Twitter";

        public bool IsVariableDomain { get; } = false;

        public string ClientKeyName { get; } = "ConsumerKey";

        public string ClientSecretKeyName { get; } = "ConsumerSecret";
    }
}
