using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialApis
{
    public class SortedQuery : IDictionary<string, object>, ICloneable
    {
        private readonly SortedDictionary<string, object> _dictionary;

        public SortedQuery()
        {
            this._dictionary = new SortedDictionary<string, object>(StringComparer.CurrentCulture);
        }

        public SortedQuery(IEnumerable<KeyValuePair<string, object>> pairs)
        {
            if (pairs == null)
            {
                this._dictionary = new SortedDictionary<string, object>(StringComparer.CurrentCulture);
            }
            else
            {
                this._dictionary = new SortedDictionary<string, object>(StringComparer.CurrentCulture);
                foreach (var kvp in pairs)
                {
                    this[kvp.Key] = kvp.Value;
                }
            }
        }

        public SortedQuery(IDictionary<string, object> dictionary)
        {
            this._dictionary = dictionary == null
                ? new SortedDictionary<string, object>(StringComparer.CurrentCulture)
                : new SortedDictionary<string, object>(dictionary, StringComparer.CurrentCulture);
        }

        public object this[string key]
        {
            get => TryGetValue(key, out var value) ? value : null;
            set
            {
                if (ContainsKey(key))
                    _dictionary[key] = value;
                else
                    this.Add(key, value);
            }
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<object> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly { get; } = false;

        public void Add(string key, object value) => _dictionary.Add(key, value);

        public void Add(KeyValuePair<string, object> item) => this._dictionary.Add(item.Key, item.Value);

        public void AddOrUpdate(KeyValuePair<string, object> item) => this[item.Key] = item.Value;

        public void Clear() => _dictionary.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _dictionary.Contains(item);

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();

        public bool Remove(string key) => _dictionary.Remove(key);

        public bool Remove(KeyValuePair<string, object> item) => _dictionary.Remove(item.Key);

        public bool TryGetValue(string key, out object value) => _dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        public SortedQuery Clone()
        {
            return new SortedQuery(this);
        }

        object ICloneable.Clone() => this.Clone();

        public static SortedQuery Merge(SortedQuery qLeft, SortedQuery qRight)
        {
            if (qLeft == null && qRight == null)
                throw new ArgumentNullException($"{ nameof(qLeft) } and { nameof(qRight) }");

            if (qLeft == null)
                return qRight.Clone();

            else if (qRight == null)
                return qLeft.Clone();

            var query = qLeft.Clone();

            if (qRight.Count > 0)
            {
                foreach (var key in qRight.Keys.Except(qLeft.Keys))
                {
                    query[key] = qRight[key];
                }
            }

            return query;
        }

        public static SortedQuery operator +(SortedQuery qLeft, SortedQuery qRight) => Merge(qLeft, qRight);
    }
}
