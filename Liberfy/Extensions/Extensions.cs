using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Liberfy
{
    public static class Extensions
    {
        public static T CastOrDefault<T>(this object @object)
        {
            return @object is T value ? value : default;
        }

        public static bool Contains(this string value, string find, StringComparison comparison)
        {
            return value.IndexOf(find, comparison) >= 0;
        }

        public static void DisposeAll<T>(this IEnumerable<T> collection) where T : IDisposable
        {
            foreach (var item in collection)
            {
                item.Dispose();
            }
        }

        public static IEnumerable<T> Combine<T>(this IEnumerable<IEnumerable<T>> collection)
            where T : class
        {
            var result = Enumerable.Empty<T>();

            foreach (var items in collection.Where(item => item != null))
            {
                result = result.Concat(items);
            }

            return result.Where(item => item != null);
        }

        public static string GetMessage(this Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                var messages = from ex in aggregateException.InnerExceptions select ex.Message;

                return string.Join(Environment.NewLine + Environment.NewLine, messages);
            }
            else
            {
                return exception?.Message;
            }
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<IEnumerable<T>> collection)
            where T : class
        {
            var result = Enumerable.Empty<T>();

            foreach (var items in collection.Where(item => item != null))
            {
                result = result.Union(items);
            }

            return result.Where(item => item != null);
        }

        public static string Replace(this string content, string oldValue, string newValue, StringComparison comparisonType)
        {
            return content.Replace(oldValue, newValue, comparisonType, 30);
        }

        public static string Replace(this string content, string oldValue, string newValue, StringComparison comparisonType, int bufferMargin)
        {
            bool foundValues = false;
            int foundIndex = 0;
            int previousIndex = 0;

            StringBuilder strBuilder = default;

            while ((foundIndex = content.IndexOf(oldValue, previousIndex, comparisonType)) >= 0)
            {
                if (!foundValues)
                {
                    foundValues = true;
                    strBuilder = new StringBuilder(content.Length + (bufferMargin * (Math.Max(0, newValue.Length - oldValue.Length))));
                }

                if (foundIndex != previousIndex)
                {
                    strBuilder.Append(content, previousIndex, foundIndex - previousIndex);
                }

                strBuilder.Append(newValue);

                previousIndex = foundIndex + oldValue.Length;
            }

            if (!foundValues)
            {
                return content;
            }
            else
            {
                if (previousIndex < (content.Length - 1))
                {
                    strBuilder.Append(content, previousIndex, content.Length - previousIndex);
                }

                return strBuilder.ToString();
            }
        }
    }
}
