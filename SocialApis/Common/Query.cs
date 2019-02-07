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
    using IQuery = IEnumerable<KeyValuePair<string, object>>;

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
            get => TryGetValue(key, out var value) ? value : null;
            set
            {
                if (ContainsKey(key))
                    _dictionary[key] = value;
                else
                    this.Add(key, value);
            }
        }

        public static string Join(IQuery query, string separator)
        {
            return string.Join(separator, GetRequestParameters(query));
        }

        public static string Join(IQuery query)
        {
            return Join(query, "&");
        }

        public static IEnumerable<string> GetRequestParameters(IQuery values)
        {
            return values?.Select(kvp => GetParameterPair(kvp.Key, kvp.Value)) ?? Enumerable.Empty<string>();
        }

        private static string JoinParameterPais(string name, string value, string valueEnclosure = null)
        {
            return name + "=" + (string.IsNullOrEmpty(valueEnclosure) ? value : (valueEnclosure + value + valueEnclosure));
        }

        public static string GetParamPairString(string name, object value, string valueEnclosure = null)
        {
            if (value is UrlArray urlArray)
            {
                var arrayedName = HttpHelper.UrlEncode(name) + "[]";

                var valuePairs = urlArray
                    .Select(val => JoinParameterPais(arrayedName, HttpHelper.UrlEncode(ValueToString(val))));

                return string.Join("&", valuePairs);
            }
            else
            {
                return JoinParameterPais(HttpHelper.UrlEncode(name), HttpHelper.UrlEncode(ValueToString(value)), valueEnclosure);
            }
        }

        public static string GetParameterPair(string name, object value) => GetParamPairString(name, value);

        public static string GetParameterPair(KeyValuePair<string, object> kvp) => GetParamPairString(kvp.Key, kvp.Value);

        public static string GetParameterPairDq(string name, object value) => GetParamPairString(name, value, "\"");

        public static string GetParameterPairDq(KeyValuePair<string, object> kvp) => GetParameterPairDq(kvp.Key, kvp.Value);

        private static readonly Hashtable _typeHandles = new Hashtable();

        public static string ValueToString(object value, bool nested = false)
        {
            if (value == null)
                return string.Empty;

            if (value is string stringValue)
                return nested ? $"\"{stringValue}\"" : stringValue;

            else if (value is Array arrayValue)
            {
                var stringValueList = new List<string>(arrayValue.Length);

                foreach (object element in arrayValue)
                {
                    stringValueList.Add(ValueToString(element, true));
                }

                return string.Join(",", stringValueList);
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
                    var enumString = type.GetEnumName(value);

                    var memberInfo = type.GetMember(enumString).FirstOrDefault();
                    var attr = Attribute.GetCustomAttribute(memberInfo, typeof(EnumMemberAttribute), true);

                    if (attr is EnumMemberAttribute enumMember)
                        return enumMember.Value;
                    else
                        return enumString;
                }
                else
                {
                    var typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Char:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.SByte:
                        case TypeCode.Single:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            return value.ToString();

                        case TypeCode.Boolean:
                            return value.ToString().ToLower();
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

        public static Query Merge(Query qLeft, Query qRight)
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

        public static Query operator +(Query qLeft, Query qRight) => Merge(qLeft, qRight);
    }
}
