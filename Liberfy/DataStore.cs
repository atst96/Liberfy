using CoreTweet;
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

		public static IdBaseCollection<StatusInfo> Statuses { get; } = new IdBaseCollection<StatusInfo>();
		public static IdBaseCollection<UserInfo> Users { get; } = new IdBaseCollection<UserInfo>();


		public static UserInfo UserAddOrUpdate(User user)
		{
			return user.Id.HasValue
				? Users.AddOrUpdate(
					user.Id.Value,
					(id) => new UserInfo(user),
					(id, info) =>
					{
						info.Update(user);
						return info;
					}) : null;
		}

		public static StatusInfo StatusAddOrUpdate(Status status)
		{
			return Statuses.AddOrUpdate(
				status.Id,
				(id) => new StatusInfo(status),
				(id, info) =>
				{
					info.Update(status);
					return info;
				});
		}
	}
}
