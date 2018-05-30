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
        public static Status GetSourceStatus(this Status status) => status.RetweetedStatus ?? status;

        public static long GetSourceId(this Status status) => status.GetSourceStatus().Id;

        internal static IEnumerable<EntityBase> GetAllEntities(this Entities entities)
        {
            if (entities == null)
                return Enumerable.Empty<EntityBase>();

            return new EntityBase[][]
            {
                entities.Hashtags,
                entities.Symbols,
                entities.Urls,
                entities.UserMentions,
                entities.Media
            }.Merge();
        }
	}
}
