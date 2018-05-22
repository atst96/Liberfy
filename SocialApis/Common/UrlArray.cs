using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public class UrlArray : IEnumerable, IEnumerable<object>
    {
        public IEnumerable _enumerable;

        public UrlArray(IEnumerable values)
        {
            this._enumerable = values;
        }

        IEnumerator IEnumerable.GetEnumerator() => _enumerable.GetEnumerator();

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (var value in _enumerable)
            {
                yield return value;
            }
        }
    }
}
