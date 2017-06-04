using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Liberfy
{
	internal class ColumnProperties : Dictionary<string, IConvertible>
	{
		public void SetValue(string propertyName, IConvertible value)
		{
			if (ContainsKey(propertyName))
				this[propertyName] = value;
			else
				Add(propertyName, value);
		}

		public T TryGetValue<T>(string propertyName)
		{
			return TryGetValue(propertyName, out var token) && token is T tToken ? tToken : default(T);
		}
	}
}
