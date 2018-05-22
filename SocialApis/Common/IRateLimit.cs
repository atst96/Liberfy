using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    internal interface IRateLimit
    {
        RateLimit RateLimit { get; set; }
    }
}
