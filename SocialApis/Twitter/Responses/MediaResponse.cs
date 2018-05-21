using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class MediaResponse : Media, IRateLimit
    {
        public RateLimit RateLimit { get; set; }
    }
}
