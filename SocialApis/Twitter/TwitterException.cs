using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class TwitterException : Exception
    {
        internal TwitterException(string errorText) : base(errorText) { }

        internal TwitterException(WebException wex, TwitterErrorContainer errorContainer) : base(wex.Message, wex)
        {
            this.Errors = errorContainer?.Errors ?? new TwitterError[0];
        }

        public TwitterError[] Errors { get; }
    }
}
