using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Liberfy
{
	public class FluidCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private IList<T> collection;
		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public FluidCollection() : this(new List<T>()) { }

		public FluidCollection(List<T> list)
		{
			BindingOperations.EnableCollectionSynchronization(this, App.CommonLockObject);
			collection = list ?? new List<T>();
		}

		public FluidCollection(IEnumerable<T> list) : this(new List<T>(list)) { }

		public virtual T this[int index]
		{
			get { return collection[index]; }
			set
			{
				T oldItem = collection[index];
				if (!Equals(oldItem, value))
				{
					collection[index] = value;

					RaiseCollectionChanged(
						new NotifyCollectionChangedEventArgs(
							NotifyCollectionChangedAction.Replace,
							value, oldItem, index));
				}
			}
		}

		public int Count => collection.Count;

		public bool IsReadOnly { get; } = false;

		public void Add(T item)
		{
			collection.Add(item);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add,
					item, collection.Count - 1));

			RaisePropertyChanged(nameof(Count));
		}

		public void AddRange(IEnumerable<T> items)
		{
			int count = collection.Count;

			foreach (T item in items)
			{
				collection.Add(item);
			}

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));

			RaisePropertyChanged(nameof(Count));
		}

		public void AddRange(params T[] items) => AddRange((IEnumerable<T>)items);

		public void AddFirst(T item) => Insert(0, item);

		public void AddFirst(IEnumerable<T> items, int startIndex) => Insert(startIndex, items);

		public void Insert(int index, T item)
		{
			collection.Insert(index, item);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add,
					item, index));
			RaisePropertyChanged(nameof(Count));
		}

		public void Insert(int startIndex, IEnumerable<T> items, bool alwaysNotify = false)
		{
			if (alwaysNotify)
			{
				foreach (var item in items.Reverse())
				{
					Insert(startIndex, item);
				}
			}
			else
			{
				int index = startIndex;
				foreach (var item in items)
				{
					collection.Insert(index, item);
					index++;
				}

				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Reset));

				RaisePropertyChanged(nameof(Count));
			}
		}

		public void Clear()
		{
			collection.Clear();

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));
			RaisePropertyChanged(nameof(Count));
		}

		public IEnumerator<T> GetEnumerator() => collection.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => collection.GetEnumerator();

		public void CopyTo(T[] array, int arrayIndex) => collection.CopyTo(array, arrayIndex);

		public bool Contains(T item) => collection.Contains(item);

		public int IndexOf(T item) => collection.IndexOf(item);

		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex < collection.Count)
			{
				T item = collection.ElementAtOrDefault(oldIndex);
				collection.Remove(item);
				collection.Insert(newIndex, item);

				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Move,
						item, newIndex, oldIndex));
				RaisePropertyChanged(nameof(Count));
			}
		}

		public bool Remove(T item)
		{
			int index = collection.IndexOf(item);
			bool removed = collection.Remove(item);

			if (removed)
			{
				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove, item, index));
				RaisePropertyChanged(nameof(Count));
			}

			return removed;
		}

		public void RemoveAt(int index)
		{
			int count = collection.Count;
			T item = collection.ElementAtOrDefault(index);
			collection.RemoveAt(index);

			if (count != collection.Count)
			{
				RaiseCollectionChanged(
					new NotifyCollectionChangedEventArgs(
						NotifyCollectionChangedAction.Remove, item, index));
				RaisePropertyChanged(nameof(Count));
			}
		}


		protected void RaisePropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

	}
}
