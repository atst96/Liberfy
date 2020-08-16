using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace SocialApis.Core
{
    using NameValuePair = KeyValuePair<string, string>;
    using QueryElement = KeyValuePair<string, object>;

    internal static class QueryParameterFactory
    {
        public static IEnumerable<NameValuePair> ToStringNameValuePairs(this IEnumerable<QueryElement> query)
        {
            return AddParametersImpl(query);
        }

        private static IEnumerable<NameValuePair> AddParametersImpl(IEnumerable<QueryElement> query, string baseName = null)
        {
            var results = Enumerable.Empty<NameValuePair>();

            foreach (var (key, value) in query)
            {
                var parameterName = baseName != null ? string.Concat(baseName, "[", key, "]") : key;

                results = results.Concat(AddValueImpl(parameterName, value));
            }

            return results;
        }

        private static IEnumerable<NameValuePair> AddValueImpl(string parameterName, object value)
        {
            if (value is IEnumerable<QueryElement> dictParams)
            {
                return AddParametersImpl(dictParams, parameterName);
            }

            if (value is ValueGroup separateParams)
            {
                var values = separateParams.Select(val => Query.ValueToString(val));
                var valueText = string.Join(separateParams.SeparateText, values);

                return Yield(parameterName, valueText);
            }

            if (value is string stringValue)
            {
                return Yield(parameterName, stringValue);
            }

            if (value is IEnumerable collection)
            {
                return AddArrayValueImpl(parameterName, collection);
            }

            return Yield(parameterName, Query.ValueToString(value));

        }

        private static IEnumerable<NameValuePair> AddArrayValueImpl(string parameterName, IEnumerable collection)
        {
            var arrayParameterName = parameterName + "[]";

            var enumerations = Enumerable.Empty<NameValuePair>();
            foreach (object value in collection)
            {
                var values = AddValueImpl(arrayParameterName, value);
                enumerations = enumerations.Concat(values);
            }

            return enumerations;
        }

        private static IEnumerable<T> Yield<T>(T item)
        {
            yield return item;
        }

        private static IEnumerable<NameValuePair> Yield(string key, string value)
        {
            yield return new NameValuePair(key, value);
        }
    }
}
