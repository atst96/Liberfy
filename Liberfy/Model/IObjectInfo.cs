using CoreTweet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	interface IObjectInfo<T> where T: CoreBase
	{
		long Id { get; }

		void Update(T item);
	}
}
