using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
	class WeakCollection<T> : ICollection<WeakReference<T>> where T : class
	{
		private IList<WeakReference<T>> _weakItems = new List<WeakReference<T>>();

		public int Count => _weakItems.Count;

		public bool IsReadOnly => _weakItems.IsReadOnly;

		public void Add(T weakItem)
		{
			Add(new WeakReference<T>(weakItem));
		}

		public void Add(WeakReference<T> item)
		{
			_weakItems.Add(item);
		}

		public void Clear()
		{
			_weakItems.Clear();
		}

		public bool Contains(WeakReference<T> item)
		{
			return _weakItems.Contains(item);
		}

		public void CopyTo(WeakReference<T>[] array, int arrayIndex)
		{
			_weakItems.CopyTo(array, arrayIndex);
		}

		public IEnumerator<WeakReference<T>> GetEnumerator()
		{
			return _weakItems.GetEnumerator();
		}

		public bool Remove(T item)
		{
			bool result = false;

			for (int i = _weakItems.Count - 1; i >= 0; --i)
			{
				if (_weakItems[i].TryGetTarget(out var tItem))
				{
					if (tItem.Equals(item))
					{
						_weakItems.RemoveAt(i);
						result = true;
					}
				}
				else
				{
					_weakItems.RemoveAt(i);
				}
			}

			return result;
		}

		public bool Remove(WeakReference<T> item)
		{
			return item.TryGetTarget(out var tItem) ? Remove(tItem) : false;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _weakItems.GetEnumerator();
		}
	}
}
