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
    internal interface IAccount : IEquatable<IAccount>
    {
        string ItemId { get; }
        long Id { get; }
        ServiceType Service { get; }
        IApiGateway ApiGateway { get; }
        IServiceConfiguration ServiceConfiguration { get; }
        IUserInfo Info { get; }
        TimelineBase Timeline { get; }
        bool IsLoading { get; }
        bool IsLoggedIn { get; }
        Task Load();
        IValidator Validator { get; }
        ValueTask<bool> Login();
        Task LoadDetails();
        void Unload();
        void SetApiTokens(IApi api);
        AccountSettingBase ToSetting();
    }
}
