using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Liberfy
{
	class FluidCollection<T> : ICollection<T>, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		public List<T> list;

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		public FluidCollection()
		{
			list = new List<T>();
		}

		public FluidCollection(IEnumerable<T> collection)
		{
			list = collection?.ToList() ?? new List<T>();
		}

		public int Count => list.Count;

		public bool IsReadOnly => false;

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public void Add(T item)
		{
			list.Add(item);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, item, Count - 1));

			OnItmesCountChanged();
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (var item in collection)
			{
				Add(item);
			}
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, item);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, item, index));

			OnItmesCountChanged();
		}

		public void InsertRange(int index, IEnumerable<T> collection)
		{
			int i = index;
			foreach (var item in collection)
			{
				Insert(i, item);
				i++;
			}
		}

		#region Functions for ItemIndexDecrement

		public void ItemIndexDecrement(int oldIndex, int count = 1)
		{
			Move(oldIndex, oldIndex - count);
		}

		public void ItemIndexDecrement(T item, int count = 1)
		{
			ItemIndexDecrement(IndexOf(item), count);
		}

		public bool CanItemIndexDecrement(int currentIndex, int upCount)
		{
			return MathEx.IsWithin(currentIndex, upCount, Count - 1);
		}

		public bool CanItemIndexDecrement(T item, int upCount = 1)
		{
			return CanItemIndexDecrement(IndexOf(item), upCount);
		}

		#endregion Functions for ItemIndexDecrement

		#region Functions for ItemIndexIncrement

		public void ItemIndexIncrement(int oldIndex, int count = 1)
		{
			Move(oldIndex, oldIndex + count);
		}

		public void ItemIndexIncrement(T item, int count = 1)
		{
			ItemIndexIncrement(IndexOf(item), count);
		}

		public bool CanItemIndexIncrement(int currentIndex, int downCount = 1)
		{
			return MathEx.IsWithin(currentIndex, 0, Count - (1 + downCount));
		}

		public bool CanItemIndexIncrement(T item, int downCount = 1)
		{
			return CanItemIndexIncrement(IndexOf(item), downCount);
		}

		#endregion Functions for ItemIndexIncrement

		public void Move(int oldIndex, int newIndex)
		{
			if (HasItems && MathEx.IsWithin(oldIndex, 0, Count - 1))
			{
				T item = list[oldIndex];
				list.RemoveAt(oldIndex);
				list.Insert(newIndex, item);

				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Move,
						item, newIndex, oldIndex));

				OnItmesCountChanged();
			}
		}

		public bool Remove(T item)
		{
			int index = list.IndexOf(item);

			if (list.Remove(item))
			{
				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove, item, index));

				OnItmesCountChanged();

				return true;
			}
			else
			{
				return false;
			}
		}

		public void RemoveAt(int index)
		{
			int oldCount = list.Count;

			if (HasItems && MathEx.IsWithin(index, 0, oldCount - 1))
			{
				T item = list[index];
				list.RemoveAt(index);

				if (oldCount != Count)
				{
					RaiseCollectionChanged(
						new NotifyCollectionChangedEventArgs(
							NotifyCollectionChangedAction.Remove, item, index));

					OnItmesCountChanged();
				}
			}
		}

		public void Clear()
		{
			list.Clear();

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));

			OnItmesCountChanged();
		}

		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		void OnItmesCountChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasItems)));
		}

		public bool HasItems => list.Count > 0;

		public T this[int index]
		{
			get
			{
				return list[index];
			}

			set
			{
				list[index] = value;
			}
		}

		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		public void Reset() => Clear();

		public void Reset(params T[] arry) => Reset(arry);

		public void Reset(IEnumerable<T> collection)
		{
			list.Clear();

			list.AddRange(collection);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));

			OnItmesCountChanged();
		}
	}
}
