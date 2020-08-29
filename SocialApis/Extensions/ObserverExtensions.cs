using System;
using System.Collections.Generic;

namespace SocialApis.Extensions
{
    internal static class ObserverExtensions
    {
        internal static void OnNext<T>(this IEnumerable<IObserver<T>> obervers, T item)
        {
            foreach (var observer in obervers)
            {
                observer.OnNext(item);
            }
        }

        internal static void OnError<T>(this IEnumerable<IObserver<T>> observers, Exception exception)
        {
            foreach (var observer in observers)
            {
                observer.OnError(exception);
            }
        }

        internal static void OnComplete<T>(this IEnumerable<IObserver<T>> observers)
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }
        }
    }
}
