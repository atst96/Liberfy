using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using CoreTweet.Rest;
using CoreTweet.Streaming;

namespace CoreTweet.Core
{
	public abstract partial class TokensBase
	{
		public UnOfficialApi UnOfficialApi { get { return new UnOfficialApi(this); } }
	}
}
