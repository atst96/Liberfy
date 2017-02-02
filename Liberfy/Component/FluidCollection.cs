﻿using System;
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

			RaiseCountPropertyChanged();
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

			RaiseCountPropertyChanged();
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

		public void MoveUp(int oldIndex, int count = 1) => Move(oldIndex, oldIndex - count);

		public void MoveDown(int oldIndex, int count = 1) => Move(oldIndex, oldIndex + count);

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

				RaiseCountPropertyChanged();
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

				return true;
			}
			else
				return false;
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

					RaiseCountPropertyChanged();
				}
			}
		}

		public void Clear()
		{
			list.Clear();

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));

			RaiseCountPropertyChanged();
		}

		void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			CollectionChanged?.Invoke(this, e);
		}

		void RaiseCountPropertyChanged()
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		private bool HasItems => list.Count > 0;

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

		public void Reset(IEnumerable<T> collection)
		{
			list.Clear();

			list.AddRange(collection);

			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset));

			RaiseCountPropertyChanged();
		}
	}
}
