using SocialApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal interface IAccountAuthenticator
    {
        ServiceType Service { get; }
        IApi Api { get; }
        string AuthorizeUrl { get; }
        Task Authentication(Uri instanceUri, string consumerKey, string consumerSecret);
        Task GetAccessToken(string code);
    }
}
