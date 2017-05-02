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
	}
}
