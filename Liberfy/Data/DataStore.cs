using Liberfy.Services.Mastodon;
using Liberfy.Services.Twitter;
using SocialApis;
using SocialApis.Common;
using SocialApis.Twitter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Liberfy
{
    internal static class DataStore
    {
        private static TwitterDataStore _twitter;
        public static TwitterDataStore Twitter => _twitter??(_twitter = new TwitterDataStore());

        private static MultipleInstanceDataStore<MastodonDataStore> _mastodon;
        public static MultipleInstanceDataStore<MastodonDataStore> Mastodon => _mastodon ?? (_mastodon = new MultipleInstanceDataStore<MastodonDataStore>());
    }
}
