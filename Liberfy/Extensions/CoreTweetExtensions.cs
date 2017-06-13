using CoreTweet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public static class CoreTweetExtensions
	{
		public static long GetSourceId(this Status status)
		{
			return (status.RetweetedStatus ?? status).Id;
		}

		internal static IEnumerable<Entity> GetEntities(this StatusInfo status)
		{
			return new Entity[][]
			{
				status.Entities.HashTags,
				status.Entities.Symbols,
				status.Entities.Urls,
				status.Entities.UserMentions,
				status.Entities.Media
			}.Merge();
		}
	}
}
