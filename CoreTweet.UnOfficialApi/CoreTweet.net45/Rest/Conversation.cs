using CoreTweet.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreTweet.Rest
{
	partial class UnOfficialApi
	{
		public ListedResponse<Status> Convertion(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.AccessApiArray<Status>(MethodType.Get, "conversation/show", parameters);
		}

		public Task<ListedResponse<Status>> ConvertionAsync(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.AccessApiArrayAsync<Status>(MethodType.Get, "conversation/show", parameters);
		}
	}
}
