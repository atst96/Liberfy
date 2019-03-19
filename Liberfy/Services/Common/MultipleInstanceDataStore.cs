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
        private ConcurrentDictionary<string, T> _instances;

        public MultipleInstanceDataStore()
        {
            this._instances = new ConcurrentDictionary<string, T>();
        }

        public T this[Uri uri]
        {
            get => this._instances.GetOrAdd(uri.Host, _ => (T)Activator.CreateInstance(typeof(T), uri));
            set => this._instances.AddOrUpdate(uri.Host, value, (_, __) => value);
        }
    }
}
