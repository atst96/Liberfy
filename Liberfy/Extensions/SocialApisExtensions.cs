using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterApi = SocialApis.Twitter;

namespace Liberfy
{
    public static class SocialApisExtensions
    {
        public static TwitterApi.Status GetSourceStatus(this TwitterApi.Status status)
        {
            return status.RetweetedStatus ?? status;
        }

        public static long GetSourceId(this TwitterApi.Status status)
        {
            return status.GetSourceStatus().Id;
        }
    }
}
