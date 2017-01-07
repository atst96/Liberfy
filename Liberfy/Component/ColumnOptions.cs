using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class ColumnProperties : JObject
	{
		public ColumnProperties()
			: base()
		{
		}

		public ColumnProperties(ColumnProperties other)
			: base(other)
		{
		}

		public T TryGetValue<T>(string propertyName)
		{
			JToken token;
			if(TryGetValue(propertyName, out token))
			{
				try
				{
					return token.ToObject<T>();
				} finally { }
			}

			return default(T);
		}

		public void Add(string key, object value)
		{
			Add(key, JToken.FromObject(value));
		}
	}
}
