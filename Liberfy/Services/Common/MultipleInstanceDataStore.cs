using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class MultipleInstanceDataStore<T>
        where T : new()
    {
        private ConcurrentDictionary<string, T> _instances;

        public MultipleInstanceDataStore()
        {
            this._instances = new ConcurrentDictionary<string, T>();
        }

        public T this[Uri uri]
        {
            get => this[uri.Host];
            set => this[uri.Host] = value;
        }

        public T this[string host]
        {
            get => this._instances.GetOrAdd(host, _ => Activator.CreateInstance<T>());
            set => this._instances.AddOrUpdate(host, value, (_, __) => value);
        }
    }
}
