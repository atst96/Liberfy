using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	struct MediaEntityInfo
	{
		public Account Account { get; }
		
		public StatusItem Item { get; }

		public MediaEntity[] Entities { get; }

		public MediaEntity CurrentEntity { get; }

		public MediaEntityInfo(Account account, StatusItem item)
		{
			Account = account;
			Item = item;
			Entities = Item.Status.ExtendedEntities.Media;
			CurrentEntity = Entities.First();
		}

		public MediaEntityInfo(Account account, StatusItem item, MediaEntity currentEntity)
		{
			Account = account;
			Item = item;
			Entities = Item.Status.ExtendedEntities.Media;
			CurrentEntity = currentEntity;
		}
	}
}
