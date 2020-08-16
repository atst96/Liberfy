using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialApis.Utils;

namespace SocialApis
{
    using NameValuePair = KeyValuePair<string, string>;
    using QueryElement = KeyValuePair<string, object>;

    internal class QueryParameterCollection : ICollection<NameValuePair>
    {
        private readonly List<NameValuePair> parameters;

        public QueryParameterCollection()
        {
            this.parameters = new List<NameValuePair>();
        }

        public QueryParameterCollection(IEnumerable<QueryElement> parameters)
        {
            this.parameters = new List<NameValuePair>();

            this.AddParametersImpl(parameters);
        }

        private void AddParametersImpl(IEnumerable<QueryElement> query, string baseName = null)
        {
            foreach (var (key, value) in query)
            {
                var parameterName = WebUtil.UrlEncode(key);

                if (baseName != null)
                {
                    parameterName = $"{baseName}[{parameterName}]";
                }

                this.AddValueImpl(parameterName, value);
            }
        }

        private void AddValueImpl(string parameterName, object value)
        {
            if (value is IEnumerable<QueryElement> dictParams)
            {
                this.AddParametersImpl(dictParams, parameterName);
            }
            else if (value is ValueGroup separateParams)
            {
                var values = separateParams.Select(val => Query.ValueToString(val));
                var valueText = string.Join(separateParams.SeparateText, values);

                this.Add(parameterName, WebUtil.UrlEncode(valueText));
            }
            else if (value is string stringValue)
            {
                this.Add(parameterName, WebUtil.UrlEncode(stringValue));
            }
            else if (value is IEnumerable collection)
            {
                this.AddArrayValueImpl(parameterName, collection);
            }
            else
            {
                var valueText = Query.ValueToString(value);

                this.Add(parameterName, WebUtil.UrlEncode(valueText));
            }
        }

        private void AddArrayValueImpl(string parameterName, IEnumerable collection)
        {
            var arrayParameterName = parameterName + "[]";

            foreach (object value in collection)
            {
                this.AddValueImpl(arrayParameterName, value);
            }
        }

        public int Count => this.parameters.Count;

        public bool IsReadOnly { get; } = false;

        public void Add(string key, string value)
        {
            this.parameters.Add(new NameValuePair(key, value));
        }

        public void Add(NameValuePair item)
        {
            this.parameters.Add(item);
        }

        public void Clear()
        {
            this.parameters.Clear();
        }

        public bool Contains(NameValuePair item)
        {
            return this.parameters.Contains(item);
        }

        public void CopyTo(NameValuePair[] array, int arrayIndex)
        {
            this.parameters.CopyTo(array, arrayIndex);
        }

        public IEnumerator<NameValuePair> GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }

        public bool Remove(NameValuePair item)
        {
            return this.parameters.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.parameters.GetEnumerator();
        }
    }
}
