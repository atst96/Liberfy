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

            this.ApplyItemsCount();
        }

        public FluidCollection(IEnumerable<T> collection)
        {
            this._list = collection?.ToList() ?? new List<T>();

            this.ApplyItemsCount();
        }

        public int Count { get; private set; }

        public bool HasItems { get; private set; }

        #region Bein: List impleents

        public bool IsReadOnly { get; } = false;

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

        public T this[int index]
        {
            get => this._list[index];
            set => this._list[index] = value;
        }

        public int IndexOf(T item)
        {
            return this._list.IndexOf(item);
        }

        #endregion End: List implements

        public void Add(T item)
        {
            this._list.Add(item);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, Count - 1));

            this.ApplyItemsCount();
        }

        public void AddRange(IEnumerable<T> collection)
        {
            this._list.AddRange(collection);

            int count = this.Count;

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, collection, count));

            this.ApplyItemsCount();

            //foreach (var item in collection)
            //    this.Add(item);
        }

        public void Insert(int index, T item)
        {
            this._list.Insert(index, item);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, index));

            this.ApplyItemsCount();
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            this._list.InsertRange(index, collection);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, collection, index));

            this.ApplyItemsCount();

            //int i = index;
            //foreach (var item in collection)
            //{
            //    this.Insert(i, item);
            //    i++;
            //}
        }

        public void DeleteRange(int index, int count)
        {
            var items = this._list.GetRange(index, count);

            if (items.Count > 0)
            {
                this._list.RemoveRange(index, items.Count);

                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, items, index));

                this.ApplyItemsCount();
            }
        }

        public IEnumerable<T> GetRange(int index, int count)
        {
            return this._list.GetRange(index, count);
        }

        public void Move(int oldIndex, int newIndex)
        {
            if (this.HasItems && MathEx.IsWithin(oldIndex, 0, this.Count - 1))
            {
                T item = this._list[oldIndex];
                this._list.RemoveAt(oldIndex);
                this._list.Insert(newIndex, item);

                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Move,
                        item, newIndex, oldIndex));

                this.ApplyItemsCount();
            }
        }

        public bool Remove(T item)
        {
            int index = this._list.IndexOf(item);

            if (this._list.Remove(item))
            {
                this.RaiseCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, item, index));

                this.ApplyItemsCount();

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

            if (this.HasItems && MathEx.IsWithin(index, 0, oldCount - 1))
            {
                T item = this._list[index];
                this._list.RemoveAt(index);

                if (oldCount != this._list.Count)
                {
                    this.RaiseCollectionChanged(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove, item, index));

                    this.ApplyItemsCount();
                }
            }
        }

        private void RemoveAtImpl(T item, int index)
        {
            this._list.RemoveAt(index);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, item, index));
        }

        public void RemoveAll(T item)
        {
            int index = 0;
            int count = this._list.Count;

            while (index < this._list.Count)
            {
                var _item = this._list[index];

                if (object.Equals(item, _item))
                    this.RemoveAtImpl(_item, index);
                else
                    ++index;
            }

            if (this._list.Count != count)
                this.ApplyItemsCount();
        }

        public void RemoveAll(Predicate<T> predicte)
        {
            int index = 0;
            int count = this._list.Count;

            while (index < this._list.Count)
            {
                var _item = this._list[index];

                if (predicte.Invoke(_item))
                    this.RemoveAtImpl(_item, index);
                else
                    ++index;
            }

            if (this._list.Count != count)
                this.ApplyItemsCount();
        }

        public void Clear()
        {
            this._list.Clear();

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            this.ApplyItemsCount();
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void OnItmesCountChanged()
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.Count)));
                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.HasItems)));
            }
        }

        private void ApplyItemsCount()
        {
            this.Count = this._list.Count;
            this.HasItems = this.Count > 0;

            this.OnItmesCountChanged();
        }

        public void Reset() => this.Clear();

        public void Reset(IEnumerable<T> collection)
        {
            this._list.Clear();

            this._list.AddRange(collection);

            this.RaiseCollectionChanged(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));

            this.ApplyItemsCount();
        }
    }
}
