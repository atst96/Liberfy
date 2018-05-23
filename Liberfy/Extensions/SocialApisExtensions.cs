using SocialApis.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	public static class SocialApisExtensions
	{
		public static long GetSourceId(this Status status)
		{
			return (status.RetweetedStatus ?? status).Id;
		}

        internal static IEnumerable<EntityBase> GetAllEntities(this Entities entities)
        {
            return new EntityBase[][]
            {
                entities.Hashtags,
                entities.Symbols,
                entities.Urls,
                entities.UserMentions,
                entities.Media
            }.Merge();
        }

		internal static IEnumerable<EntityBase> GetEntities(this StatusInfo status)
		{
			return new EntityBase[][]
			{
				status.Entities.Hashtags,
				status.Entities.Symbols,
				status.Entities.Urls,
				status.Entities.UserMentions,
				status.Entities.Media
			}.Merge();
		}
	}
}
