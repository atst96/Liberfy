using System;
using System.Collections.Generic;
using System.Text;

namespace SocialApis.Mastodon
{
    public class StreamStatusResponse : StreamResponse
    {
        /// <summary>
        /// トゥート
        /// </summary>
        public Status Status { get; }

        internal StreamStatusResponse(Status status)
            : base(StreamEventType.Update)
        {
            this.Status = status;
        }
    }
}
