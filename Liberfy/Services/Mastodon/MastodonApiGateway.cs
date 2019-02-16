using Liberfy.Services.Common;
using SocialApis.Mastodon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services.Mastodon
{
    internal class MastodonApiGateway : IApiGateway
    {
        private readonly MastodonApi _api;

        public MastodonApiGateway(MastodonApi api)
        {
            this._api = api;
        }

        public Task PostStatus(ServicePostParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
