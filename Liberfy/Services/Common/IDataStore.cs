using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal interface IDataStore
    {
        ConcurrentDictionary<long, UserInfo> Accounts { get; }
        ConcurrentDictionary<long, StatusInfo> Statuses { get; }
    }
}
