using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class DictionaryEx<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private ConcurrentDictionary<TKey, TValue> _dic;

		public TValue DefaultValue { get; set; }

		public DictionaryEx()
		{
			_dic = new ConcurrentDictionary<TKey, TValue>();
		}

		public DictionaryEx(IDictionary<TKey, TValue> dic)
		{
			_dic = new ConcurrentDictionary<TKey, TValue>(dic);
		}

		public DictionaryEx(IDictionary<TKey, TValue> dic, TValue defaultValue) : this(dic)
		{
			DefaultValue = defaultValue;
		}

		public virtual TValue this[TKey key]
		{
			get
			{
				TValue value;
				return _dic.TryGetValue(key, out value)
					? value : DefaultValue;
			}
			set
			{
				_dic.AddOrUpdate(key, (k) => value, (k, v) => value);
			}
		}

		public int Count => _dic.Count;

		public bool IsReadOnly { get; } = false;

		public ICollection<TKey> Keys => _dic.Keys;

		public ICollection<TValue> Values => _dic.Values;

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			this[item.Key] = item.Value;
		}

		public void Add(TKey key, TValue value)
		{
			this[key] = value;
		}

		public TValue AddOrUpdate(TKey key, Func<TKey, TValue> funcAdd, Func<TKey, TValue, TValue> funcUpdate)
		{
			return _dic.AddOrUpdate(key, funcAdd, funcUpdate);
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> funcAdd)
		{
			return _dic.GetOrAdd(key, funcAdd);
		}

		public TValue GetOrAdd(TKey key,  TValue value)
		{
			return _dic.GetOrAdd(key, value);
		}

		public void Clear() => _dic.Clear();

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return _dic.Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			return _dic.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((IDictionary<TKey, TValue>)_dic).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dic.GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		public bool Remove(TKey key)
		{
			TValue value;
			return _dic.TryRemove(key, out value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _dic.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dic.GetEnumerator();
		}
	}
}
