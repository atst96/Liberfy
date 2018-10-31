using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal static class CollectionExtensions
    {
        public static void ItemIndexDecrement<T>(this NotifiableCollection<T> collection, int oldIndex, int count = 1)
        {
            collection?.Move(oldIndex, oldIndex - count);
        }

        public static void ItemIndexDecrement<T>(this NotifiableCollection<T> collection, T item, int count = 1)
        {
            if (collection != null)
                collection.Move(collection.IndexOf(item), count);
        }

        public static bool CanItemIndexDecrement<T>(this NotifiableCollection<T> collection, int currentIndex, int count)
        {
            return MathEx.IsWithin(currentIndex, count, collection.Count - 1);
        }

        public static bool CanItemIndexDecrement<T>(this NotifiableCollection<T> collection, T item, int upCount = 1)
        {
            return collection.CanItemIndexDecrement(collection.IndexOf(item), upCount);
        }


        public static void ItemIndexIncrement<T>(this NotifiableCollection<T> collection, int oldIndex, int count = 1)
        {
            collection.Move(oldIndex, oldIndex + count);
        }

        public static void ItemIndexIncrement<T>(this NotifiableCollection<T> collection, T item, int count = 1)
        {
            collection.ItemIndexIncrement(collection.IndexOf(item), count);
        }

        public static bool CanItemIndexIncrement<T>(this NotifiableCollection<T> collection, int currentIndex, int downCount = 1)
        {
            return MathEx.IsWithin(currentIndex, 0, collection.Count - (1 + downCount));
        }

        public static bool CanItemIndexIncrement<T>(this NotifiableCollection<T> collection, T item, int downCount = 1)
        {
            return collection.CanItemIndexIncrement(collection.IndexOf(item), downCount);
        }
    }
}
