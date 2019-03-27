using Liberfy.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Twitter
{
    internal class TwitterServiceConfiguration : NotificationObject, IServiceConfiguration
    {
        public bool HasSpoilerText { get; } = false;

        public int? PostTextLength { get; } = 140;

        public bool IsPostTextLengthLimited { get; } = true;

        public bool IsSupportPolls { get; } = false;

        public int MaxPollsCount { get; } = 0;
    }
}
