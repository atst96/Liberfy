using System;
using System.Collections.Generic;

namespace SocialApis.Core
{
    internal class Unsubscriber<T> : IDisposable
    {
        private IObserver<T> _observer;
        private ICollection<IObserver<T>> _observables;

        internal Unsubscriber(IObserver<T> observer, ICollection<IObserver<T>> observers)
        {
            this._observer = observer;
            this._observables = observers;
        }

        /// <summary>
        /// 購読解除する。
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this._observer == null)
            {
                return;
            }

            this._observables.Remove(this._observer);
            GC.SuppressFinalize(this);
        }

        ~Unsubscriber()
        {
            ((IDisposable)this).Dispose();
        }
    }
}
