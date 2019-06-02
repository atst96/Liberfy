using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class MultipleInstanceDataStore<T>
    {
        private readonly ConcurrentDictionary<Uri, T> _instances;

        public MultipleInstanceDataStore()
        {
            this._instances = new ConcurrentDictionary<Uri, T>();
        }

        public T this[Uri uri]
        {
            get => this._instances.GetOrAdd(uri, _ => (T)Activator.CreateInstance(typeof(T), uri));
            set => this._instances.AddOrUpdate(uri, value, (_, __) => value);
        }
    }
}
