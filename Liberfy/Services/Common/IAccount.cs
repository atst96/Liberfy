using Liberfy.Services;
using Liberfy.Services.Common;
using Liberfy.Settings;
using SocialApis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal interface IAccount
    {
        string ItemId { get; }
        ServiceType Service { get; }
        IApiGateway ApiGateway { get; }
        IServiceConfiguration ServiceConfiguration { get; }
        bool IsLoading { get; }
        bool IsVerified { get; }
        Task StartActivity();
        IValidator Validator { get; }
        void StopActivity();
        IAccountSetting GetSetting();
    }
}
