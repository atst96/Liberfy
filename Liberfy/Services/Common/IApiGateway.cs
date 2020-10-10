using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Common
{
    // TODO: Gatewayクラス廃止予定
    [Obsolete]
    internal interface IApiGateway
    {
        [Obsolete]
        Task PostStatus(ServicePostParameters parameters);

        [Obsolete]
        Task Favorite(StatusItem item);

        [Obsolete]
        Task Unfavorite(StatusItem item);

        [Obsolete]
        Task Retweet(StatusItem item);

        [Obsolete]
        Task Unretweet(StatusItem item);
    }
}
