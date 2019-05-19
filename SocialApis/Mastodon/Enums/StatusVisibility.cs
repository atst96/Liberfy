using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public static class StatusVisibilities
    {
        public const string Public = "public";
        public const string Unlisted = "unlisted";
        public const string Private = "private";
        public const string Direct = "direct";
    }
}
