using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Liberfy.Test
{
    /// <summary>
    /// <see cref="NotifyCollectionConverter{T, U}"/>のテストクラス
    /// </summary>
    public class NotifyCollectionConverterTest
    {
        private static int Create(string value) => int.Parse(value);
        private static int Update(int currentValue, string nextValue) => int.Parse(nextValue);

        [Fact]
        public void ConstructorTest()
        {
            var collection = new ObservableCollection<string> { "1" };
            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            Assert.Contains(1, converter);
            Assert.Equal(converter.Count, collection.Count);
        }

        [Fact]
        public void ConstructorFailTest()
        {
            Assert.Throws<NotSupportedException>(() =>
            {
                var collection = new List<string>();
                var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);
            });
        }

        [Fact]
        public void AddTest()
        {
            var collection = new ObservableCollection<string>();
            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            collection.Add("10");

            Assert.Contains(10, converter);
            Assert.Equal(converter.Count, collection.Count);
        }

        [Fact]
        public void RemoveTest()
        {
            var collection = new ObservableCollection<string>();
            collection.Add("0");

            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            Assert.Contains(0, converter);

            collection.Remove("0");

            Assert.DoesNotContain(0, converter);
            Assert.Equal(converter.Count, collection.Count);
        }

        [Fact]
        public void ReplaceTest()
        {
            var collection = new ObservableCollection<string> { "150" };
            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            Assert.Equal(150, converter.ElementAt(0));
            Assert.Equal(converter.Count, collection.Count);

            collection[0] = "352";

            Assert.Equal(352, converter.ElementAt(0));
            Assert.Equal(converter.Count, collection.Count);
        }

        [Fact]
        public void MoveTest()
        {
            var collection = new ObservableCollection<string> { "100", "200" };
            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            collection.Move(1, 0);

            Assert.Equal(200, converter.ElementAt(0));
            Assert.Equal(100, converter.ElementAt(1));
            Assert.Equal(converter.Count, collection.Count);
        }

        [Fact]
        public void ResetTest()
        {
            var collection = new ObservableCollection<string> { "1000", "1001" };

            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);

            Assert.NotEmpty(converter);
            Assert.Equal(2, converter.Count);

            collection.Clear();

            Assert.Empty(converter);
        }

        [Fact]
        public void DisposeTest()
        {
            var collection = new ObservableCollection<string> { "1000", "1001" };

            NotifyCollectionChangedEventHandler eventHandler;
            var type = collection.GetType();
            var targetEvent = type.GetField(nameof(INotifyCollectionChanged.CollectionChanged), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            var converter = new NotifyCollectionConverter<string, int>(collection, Create, Update);
            collection.Add("1001");

            eventHandler = (NotifyCollectionChangedEventHandler)targetEvent.GetValue(collection);

            // イベントが1件購読されていることを確認する
            var beforeActual = eventHandler.GetInvocationList();
            Assert.Single(beforeActual);

            ((IDisposable)converter).Dispose();

            // イベント購読解除の確認
            // 購読がない為はnullが返る
            eventHandler = (NotifyCollectionChangedEventHandler)targetEvent.GetValue(collection);
            Assert.Null(targetEvent.GetValue(collection));

            Assert.Throws<ObjectDisposedException>(
                () => ((IDisposable)converter).Dispose());
        }
    }
}
