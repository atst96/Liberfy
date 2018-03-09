﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SocialApis
{
    public class Query : IDictionary<string, object>, ICloneable
    {
        private readonly IDictionary<string, object> _dictionary;

        public Query()
        {
            this._dictionary = new Dictionary<string, object>();
        }

        public Query(IDictionary<string, object> dictionary)
        {
            this._dictionary = dictionary == null
                ? new Dictionary<string, object>()
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
                    Add(key, value);
            }
        }

        public IOrderedEnumerable<KeyValuePair<string, object>> GetOrdered()
        {
            return this.OrderBy(kvp => kvp.Key, StringComparer.CurrentCulture);
        }

        public IEnumerable<string> GetRequestParameters(bool sort = false)
        {
            var values = sort ? (IEnumerable<KeyValuePair<string, object>>)this.GetOrdered() : this;

            return values
                .Select(kvp => GetParameterPair(kvp.Key, kvp.Value));
        }

        private static string JoinParameterPais(string name, string value, string valueEnclosure = null)
        {
            return name + "=" + (string.IsNullOrEmpty(valueEnclosure) ? value : (valueEnclosure + value + valueEnclosure));
        }

        public static string GetParamPairString(string name, object value, string valueEnclosure = null)
        {
            if (value is UrlArray urlArray)
            {
                var arrayedName = OAuthHelper.UrlEncode(name) + "[]";

                var valuePairs = urlArray
                    .Select(val => JoinParameterPais(arrayedName, OAuthHelper.UrlEncode(ValueToString(val))));

                return string.Join("&", valuePairs);
            }
            else
            {
                return JoinParameterPais(OAuthHelper.UrlEncode(name), OAuthHelper.UrlEncode(ValueToString(value)), valueEnclosure);
            }
        }

        public string ToUrlParameterString(string connector = "&", bool sort = false)
        {
            return string.Join(connector, this.GetRequestParameters(sort));
        }

        public static string GetParameterPair(string name, object value) => GetParamPairString(name, value);

        public static string GetParameterPairDq(string name, object value) => GetParamPairString(name, value, "\"");

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
                var _type = _typeHandles[typeHandle];
                if (!(_type is Type type))
                {
                    type = Type.GetTypeFromHandle(typeHandle);
                    _typeHandles[typeHandle] = type;
                }

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

                throw new NotSupportedException();
            }
        }

        public ICollection<string> Keys => _dictionary.Keys;

        public ICollection<object> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => _dictionary.IsReadOnly;

        public void Add(string key, object value) => _dictionary.Add(key, value);

        public void Add(KeyValuePair<string, object> item) => _dictionary.Add(item);

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
