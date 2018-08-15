using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Common
{
    public interface IStatus
    {
        SocialService Service { get; }

        long Id { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
