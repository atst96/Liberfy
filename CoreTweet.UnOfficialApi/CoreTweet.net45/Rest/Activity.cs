using CoreTweet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreTweet.Rest
{
	public partial class UnOfficialApi : ApiProviderBase
	{
		internal UnOfficialApi(TokensBase e) : base(e) { }

		public HttpWebResponse Activity(IDictionary<string, object> parameters)
		{
			return Tokens.SendRequest(MethodType.Get, "https://api.twitter.com/i/activity/by_friends.json", parameters);
		}

		public HttpWebResponse Activity(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.SendRequest(MethodType.Get, "https://api.twitter.com/i/activity/by_friends.json", parameters);
		}

		public Task<AsyncResponse> ActivityAsync(IDictionary<string, object> parameters)
		{
			return Tokens.SendRequestAsync(MethodType.Get, "https://api.twitter.com/i/activity/by_friends.json", parameters);
		}

		public Task<AsyncResponse> ActivityAsync(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.SendRequestAsync(MethodType.Get, "https://api.twitter.com/i/activity/by_friends.json", parameters);
		}

		public ListedResponse<Activity> AboutMeActivity(IDictionary<string, object> parameters)
		{
			return Tokens.AccessApiArray<Activity>(MethodType.Get, "activity/about_me", parameters);
		}

		public ListedResponse<Activity> AboutMeActivity(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.AccessApiArray<Activity>(MethodType.Get, "activity/about_me", parameters);
		}

		public Task<ListedResponse<Activity>> AboutMeActivityAsync(params Expression<Func<string, object>>[] parameters)
		{
			return Tokens.AccessApiArrayAsync<Activity>(MethodType.Get, "activity/about_me", parameters);
		}
	}
}
