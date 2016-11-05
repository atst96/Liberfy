using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Liberfy
{
	static class DataStore
	{
		static DataStore()
		{
			BindingOperations.EnableCollectionSynchronization(Statuses, App.CommonLockObject);
			BindingOperations.EnableCollectionSynchronization(Users, App.CommonLockObject);
		}

		public static ConcurrentDictionary<long, StatusInfo> Statuses { get; } = new ConcurrentDictionary<long, StatusInfo>();
		public static ConcurrentDictionary<long, UserInfo> Users { get; } = new ConcurrentDictionary<long, UserInfo>();
	}
}
