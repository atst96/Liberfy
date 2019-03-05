using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy
{
    internal class ObservableUnsubscriber<T> : IDisposable
    {
        private IObserver<T> _observer;
        public IUnsubscribableObserver<T> _observable;

        public ObservableUnsubscriber(IObserver<T> observer, IUnsubscribableObserver<T> observable)
        {
            this._observer = observer;
            this._observable = observable;
        }

        public void Dispose()
        {
            this._observable.Unsubscribe(this._observer);

            this._observer = null;
            this._observable = null;
        }
    }
}
