using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Liberfy
{
    internal class ColumnManager : ICollection<ColumnBase>
    {
        private readonly IAccount _account;
        private readonly List<ColumnBase> _columns;
        private readonly NotifiableCollection<ColumnBase> _allColumns;

        public ColumnManager(IAccount account, NotifiableCollection<ColumnBase> allColumns)
        {
            this._account = account;
            this._allColumns = allColumns;
            this._columns = new List<ColumnBase>(10);
            this.ResetColumns(this._columns);

            this._allColumns.CollectionChanged += this.OnCollectionChanged;
        }

        private IEnumerable<ColumnBase> FilterAccountColumns(IEnumerable<ColumnBase> columns)
        {
            return columns.Where(c => c.Account == this._account);
        }

        private void ResetColumns(IEnumerable<ColumnBase> columns)
        {
            this._columns.Clear();

            var accountColumns = this.FilterAccountColumns(columns);
            this._columns.AddRange(accountColumns);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var accountColumns = this._columns;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var columns = this.FilterAccountColumns(e.NewItems.Cast<ColumnBase>());
                    accountColumns.AddRange(columns);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ColumnBase item in e.OldItems)
                    {
                        accountColumns.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (ColumnBase item in e.OldItems)
                    {
                        accountColumns.Remove(item);
                    }

                    var newColumns = this.FilterAccountColumns(e.NewItems.Cast<ColumnBase>());
                    accountColumns.AddRange(newColumns);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    accountColumns.Clear();

                    if (sender is IEnumerable<ColumnBase> allColumns)
                    {
                        this.ResetColumns(allColumns);
                    }
                    break;
            }
        }

        public int Count => this._columns.Count;

        public bool IsReadOnly { get; } = false;

        public IEnumerable<ColumnBase> GetsTypeOf(ColumnType type)
        {
            return this._columns.Where(c => c.Type == type);
        }

        public void Add(ColumnBase item)
        {
            this._columns.Add(item);
            this._allColumns.Add(item);
        }

        public void Clear()
        {
            this._columns.Clear();
            this._allColumns.RemoveAll(c => c.Account == this._account);
        }

        public bool Contains(ColumnBase item)
        {
            return this._columns.Contains(item);
        }

        public bool Remove(ColumnBase item)
        {
            return this._columns.Remove(item) && this._columns.Remove(item);
        }

        void ICollection<ColumnBase>.CopyTo(ColumnBase[] array, int arrayIndex)
        {
            this._columns.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ColumnBase> GetEnumerator() => this._columns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this._columns.GetEnumerator();
    }
}
