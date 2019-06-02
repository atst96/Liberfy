using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Common
{
    internal interface IApiGateway
    {
        Task PostStatus(ServicePostParameters parameters);

        Task Favorite(StatusItem item);

        Task Unfavorite(StatusItem item);

        Task Retweet(StatusItem item);

        Task Unretweet(StatusItem item);
    }
}
