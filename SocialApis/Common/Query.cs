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
    using IQuery = ICollection<KeyValuePair<string, object>>;

    public class Query : IDictionary<string, object>, ICloneable
    {
        private const int DefaultCapacity = 14;

        private readonly IDictionary<string, object> _dictionary;

        public Query()
        {
            this._dictionary = new Dictionary<string, object>(DefaultCapacity);
        }

        public Query(IEnumerable<KeyValuePair<string, object>> pairs)
        {
            if (pairs == null)
            {
                this._dictionary = new Dictionary<string, object>();
            }
            else
            {
                this._dictionary = new Dictionary<string, object>(DefaultCapacity);
                foreach (var kvp in pairs)
                {
                    this[kvp.Key] = kvp.Value;
                }
            }
        }

        public Query(IDictionary<string, object> dictionary)
        {
            this._dictionary = dictionary == null
                ? new Dictionary<string, object>(DefaultCapacity)
                : new Dictionary<string, object>(dictionary);
        }

        public object this[string key]
        {
            get => this.TryGetValue(key, out var value) ? value : null;
            set
            {
                if (this.ContainsKey(key))
                {
                    this._dictionary[key] = value;
                }
                else
                {
                    this.Add(key, value);
                }
            }
        }

        public static string JoinParameters(IQuery query, string separator)
        {
            return string.Join(separator, Query.EnumerateQueryPairs(query));
        }

        public static string JoinParametersWithAmpersand(IQuery query)
        {
            return Query.JoinParameters(query, "&");
        }

        public static IEnumerable<string> EnumerateQueryPairs(IQuery query)
        {
            var parameters = new QueryParameterCollection(query);

            foreach (var (key, value) in parameters)
            {
                yield return key + "=" + value;
            }

            parameters.Clear();
        }

        private static readonly Hashtable _typeHandles = new Hashtable();

        public static string ValueToString(object value, bool nested = false)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else if (value is string stringValue)
            {
                return nested ? $"\"{stringValue}\"" : stringValue;
            }
            else if (value is ValueGroup valueGroup)
            {
                return string.Join(valueGroup.SeparateText, valueGroup.Select(v => ValueToString(v)));
            }
            else
            {
                var typeHandle = Type.GetTypeHandle(value);
                var type = _typeHandles[typeHandle] as Type;
                if (type == null)
                {
                    type = Type.GetTypeFromHandle(typeHandle);
                    _typeHandles[typeHandle] = type;
                }

                if (type.IsEnum)
                {
                    var enumName = type.GetEnumName(value);
                    var memberInfo = type.GetMember(enumName).FirstOrDefault();
                    var memberAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(EnumMemberAttribute), true);

                    return memberAttribute is EnumMemberAttribute enumMemberAttribute
                        ? enumMemberAttribute.Value
                        : enumName;
                }
                else
                {
                    TypeCode typeCode = Type.GetTypeCode(type);

                    if (typeCode == TypeCode.Boolean)
                    {
                        return value.ToString().ToLower();
                    }

                    if (typeCode >= TypeCode.Char && typeCode <= TypeCode.Decimal)
                    {
                        return value.ToString();
                    }

                    if (typeCode == TypeCode.DateTime)
                    {
                        // TODO
                        return value.ToString();
                    }
                }

                throw new NotSupportedException();
            }
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<object> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(string key, object value) => _dictionary.Add(key, value);

        public void Add(KeyValuePair<string, object> item) => _dictionary.Add(item);

        public void AddOrUpdate(KeyValuePair<string, object> item) => this[item.Key] = item.Value;

        public void Clear() => _dictionary.Clear();

        public bool Contains(KeyValuePair<string, object> item) => _dictionary.Contains(item);

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _dictionary.GetEnumerator();

        public bool Remove(string key) => _dictionary.Remove(key);

        public bool Remove(KeyValuePair<string, object> item) => _dictionary.Remove(item);

        public bool TryGetValue(string key, out object value) => _dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        public Query Clone()
        {
            return new Query(this);
        }

        object ICloneable.Clone() => this.Clone();

        public static Query Merge(Query leftQuery, Query rightQuery)
        {
            if (leftQuery == null && rightQuery == null)
            {
                throw new ArgumentNullException($"{ nameof(leftQuery) } and { nameof(rightQuery) }");
            }
            else if (leftQuery?.Count == 0)
            {
                return rightQuery.Clone();
            }
            else if (rightQuery?.Count == 0)
            {
                return leftQuery.Clone();
            }

            var newQuery = leftQuery.Clone();

            if (rightQuery.Count > 0)
            {
                foreach (var key in rightQuery.Keys.Except(leftQuery.Keys))
                {
                    newQuery[key] = rightQuery[key];
                }
            }

            return newQuery;
        }

        public static Query operator +(Query leftQuery, Query rightQuery) => Query.Merge(leftQuery, rightQuery);
    }
}
