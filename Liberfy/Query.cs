using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class Query : Dictionary<string, object>
	{
		public Query() : base() { }
		public Query(IDictionary<string, object> dic) : base(dic) { }
	}

	class Query<T> : Dictionary<string, T>
	{
		public Query() : base() { }
		public Query(IDictionary<string, T> dic) : base(dic) { }
	}
}
