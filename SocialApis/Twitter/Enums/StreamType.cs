using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public enum StreamType
    {
        Control,
        Delete,
        DirectMessage,
        Disconnect,
        Envelopes,
        Event,
        Friends,
        Limit,
        Raw,
        ScrubGeo,
        Status,
        StatusWithheld,
        UserWithheld,
        Warning,
    }
}
