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
    public class FluidCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private List<T> _list;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public FluidCollection()
        {
            this._list = new List<T>();
        }

        public FluidCollection(IEnumerable<T> collection)
        {
            this._list = collection?.ToList() ?? new List<T>();
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public bool Contains(T item)
        {
            return this._list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void Add(T item)
        {
            this._list.Add(item);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, Count - 1));

            this.OnItmesCountChanged();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                this.Add(item);
        }

        public void Insert(int index, T item)
        {
            this._list.Insert(index, item);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, index));

            this.OnItmesCountChanged();
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            int i = index;
            foreach (var item in collection)
            {
                this.Insert(i, item);
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

        public bool CanItemIndexDecrement(int currentIndex, int count)
        {
            return MathEx.IsWithin(currentIndex, count, Count - 1);
        }

        public bool CanItemIndexDecrement(T item, int upCount = 1)
        {
            return CanItemIndexDecrement(IndexOf(item), upCount);
        }

        #endregion

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
            if (this.HasItems && MathEx.IsWithin(oldIndex, 0, this.Count - 1))
            {
                T item = _list[oldIndex];
                this._list.RemoveAt(oldIndex);
                this._list.Insert(newIndex, item);

                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Move,
                        item, newIndex, oldIndex));

                this.OnItmesCountChanged();
            }
        }

        public bool Remove(T item)
        {
            int index = _list.IndexOf(item);

            if (_list.Remove(item))
            {
                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, item, index));

                this.OnItmesCountChanged();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            int oldCount = this._list.Count;

            if (HasItems && MathEx.IsWithin(index, 0, oldCount - 1))
            {
                T item = _list[index];
                this._list.RemoveAt(index);

                if (oldCount != Count)
                {
                    this.RaiseCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove, item, index));

                    this.OnItmesCountChanged();
                }
            }
        }

        public void Clear()
        {
            this._list.Clear();

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            this.OnItmesCountChanged();
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void OnItmesCountChanged()
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasItems)));
        }

        public bool HasItems => this._list.Count > 0;

        public T this[int index]
        {
            get => this._list[index];
            set => this._list[index] = value;
        }

        public int IndexOf(T item)
        {
            return this._list.IndexOf(item);
        }

        public void Reset() => Clear();

        public void Reset(IEnumerable<T> collection)
        {
            this._list.Clear();

            this._list.AddRange(collection);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            this.OnItmesCountChanged();
        }
    }
}
