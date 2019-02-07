using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    interface ISocialConfig
    {
        string Name { get; }

        bool IsVariableDomain { get; }

        string ClientKeyName { get; }

        string ClientSecretKeyName { get; }
    }
}
