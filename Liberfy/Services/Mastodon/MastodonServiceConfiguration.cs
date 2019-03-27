using Liberfy.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonServiceConfiguration : NotificationObject, IServiceConfiguration
    {
        public bool HasSpoilerText { get; } = true;

        public int? PostTextLength { get; }

        public bool IsPostTextLengthLimited { get; } = false;

        public bool IsSupportPolls { get; } = true;

        public int MaxPollsCount { get; } = 4;
    }
}
