using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Twitter;

namespace Liberfy.Services.Twitter
{
	internal class TextEntityStartPositionCompare : IComparer<EntityBase>
	{
		public static TextEntityStartPositionCompare _instance;
		public static TextEntityStartPositionCompare Instance => _instance ?? (_instance = new TextEntityStartPositionCompare());

		public int Compare(EntityBase x, EntityBase y)
		{
			return x.IndexStart.CompareTo(y.IndexStart);
		}
	}
}
