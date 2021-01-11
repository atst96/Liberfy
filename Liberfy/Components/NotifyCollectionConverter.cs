using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Liberfy
{
    /// <summary>
    /// 動的通知コレクションの変換クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public class NotifyCollectionConverter<T, U> : IDisposable, IReadOnlyCollection<U>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private bool _isDisposed = false;
        private readonly List<U> _collection = new();
        private IEnumerable<T> _notifyCollection;
        private readonly Func<T, U> _createFunc;
        private readonly Func<U, T, U> _updateFunc;

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public NotifyCollectionConverter(IEnumerable<T> collection, Func<T, U> createFunc, Func<U, T, U> replaceFunc)
        {
            if (collection is INotifyCollectionChanged notifyCollection)
            {
                this._notifyCollection = collection;
                notifyCollection.CollectionChanged += this.OnCollectionChanged;
            }
            else
            {
                throw new NotSupportedException();
            }

            this._createFunc = createFunc;
            this._updateFunc = replaceFunc;

            var newItems = collection.Select(s => this._createFunc(s)).ToArray();
            this._collection.AddRange(newItems);
        }

        /// <summary>
        /// コレクション変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnAdded(e.NewItems.Cast<T>(), e.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.OnRemoved(e.OldStartingIndex, e.OldItems.Count);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this.OnReplaced(e.NewItems.Cast<T>(), e.OldStartingIndex, e.OldItems.Count);
                    break;

                case NotifyCollectionChangedAction.Move:
                    this.OnMoved(e.OldStartingIndex, e.NewStartingIndex, e.NewItems.Count);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.OnReset();
                    break;
            }
        }

        /// <summary>
        /// 追加時
        /// </summary>
        /// <param name="newItems"></param>
        /// <param name="newStartingIndex"></param>
        private void OnAdded(IEnumerable<T> newItems, int newStartingIndex)
        {
            var changedItems = newItems.Select(s => this._createFunc(s)).ToArray();
            this._collection.InsertRange(newStartingIndex, changedItems);
            this.CollectionChanged?.Invoke(this,
                new(NotifyCollectionChangedAction.Add, (IList)changedItems, newStartingIndex));

            if (changedItems.Any())
            {
                this.RaisePropertyChanged(nameof(Count));
            }
        }

        /// <summary>
        /// 削除時
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        private void OnRemoved(int startIndex, int count)
        {
            var removeItems = this._collection.GetRange(startIndex, count);
            this._collection.RemoveRange(startIndex, count);
            this.CollectionChanged?.Invoke(this,
                new(NotifyCollectionChangedAction.Remove, (IList)removeItems, count));

            if (removeItems.Any())
            {
                this.RaisePropertyChanged(nameof(Count));
            }
        }

        /// <summary>
        /// 変更時
        /// </summary>
        /// <param name="newItems"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        private void OnReplaced(IEnumerable<T> newItems, int startIndex, int count)
        {
            var collection = this._collection;
            var targetItems = collection.GetRange(startIndex, count);
            var changedItems = newItems
                .Zip(targetItems)
                .Select(current => this._updateFunc(current.Second, current.First))
                .ToArray();

            int endedIndex = Math.Min(collection.Count, startIndex + changedItems.Length);
            for (int idx = startIndex; idx < endedIndex; ++idx)
            {
                collection[idx] = changedItems[idx];
            }

            this.CollectionChanged?.Invoke(this,
                new(NotifyCollectionChangedAction.Replace, (IList)targetItems, (IList)changedItems, startIndex));
        }

        /// <summary>
        /// 移動時
        /// </summary>
        /// <param name="oldIndex"></param>
        /// <param name="newIndex"></param>
        /// <param name="count"></param>
        private void OnMoved(int oldIndex, int newIndex, int count)
        {
            var targetItems = this._collection.GetRange(oldIndex, count);
            this._collection.RemoveRange(oldIndex, count);
            this._collection.InsertRange(newIndex, targetItems);

            this.CollectionChanged?.Invoke(this,
                new(NotifyCollectionChangedAction.Move, (IList)targetItems, oldIndex, newIndex));
        }

        /// <summary>
        /// 初期化時
        /// </summary>
        private void OnReset()
        {
            this._collection.Clear();
            this.CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// プロパティの変更通知を行う。
        /// </summary>
        /// <param name="propertyName"></param>
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposableの実装
        void IDisposable.Dispose()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(nameof(CollectionConverter));
            }

            this._isDisposed = true;

            ((INotifyCollectionChanged)this._notifyCollection).CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion IDisposableの実装

        #region IReadOnlyCollection<U>の実装

        public int Count => this._collection.Count;

        public IEnumerator<U> GetEnumerator() => this._collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._collection.GetEnumerator();

        #endregion IReadOnlyCollection<U>の実装
    }
}
