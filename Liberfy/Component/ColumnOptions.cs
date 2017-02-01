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
	class ColumnProperties : IDictionary<string, object>
	{
		private IDictionary<string, object> _dic = new Dictionary<string, object>();

		public ColumnProperties()
		{
			_dic = new Dictionary<string, object>();
		}

		public ColumnProperties(ColumnProperties other)
		{
			_dic = new Dictionary<string, object>(other);
		}

		public object this[string key]
		{
			get
			{
				object value;

				return _dic.TryGetValue(key, out value) ? value : null;
			}
			set
			{
				if(ContainsKey(key))
				{
					_dic[key] = value;
				}
				else
				{
					_dic.Add(key, value);
				}
			}
		}

		public int Count => _dic.Count;

		public bool IsReadOnly => false;

		public ICollection<string> Keys => _dic.Keys;

		public ICollection<object> Values => _dic.Values;

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			_dic.Add(item);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			_dic.Add(key, value);
		}

		public void Clear()
		{
			_dic.Clear();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			return _dic.Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return _dic.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			_dic.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _dic.GetEnumerator();
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			return _dic.Remove(item);
		}

		public bool Remove(string key)
		{
			return _dic.Remove(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return _dic.TryGetValue(key, out value);
		}

		public T TryGetValue<T>(string propertyName)
		{
			object token;
			if(TryGetValue(propertyName, out token))
			{
				if(token is T)
				{
					return (T)token;
				}
			}
			
			return default(T);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dic.GetEnumerator();
		}
	}
}
