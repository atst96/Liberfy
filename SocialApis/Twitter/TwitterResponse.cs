using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public class TwitterResponse<T> where T : class
    {
        internal TwitterResponse(T value, WebHeaderCollection httpHeaders = null)
        {
            this.Response = value;
        }

        public T Response { get; }
    }
}
