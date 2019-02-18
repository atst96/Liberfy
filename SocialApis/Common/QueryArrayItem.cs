using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public class QueryArrayItem : Collection<object>
    {
        public QueryArrayItem() : base()
        {
        }

        public QueryArrayItem(IList<object> collection)
            : base(collection)
        {
        }

        public QueryArrayItem(object[] collection) : base()
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        public QueryArrayItem(IEnumerable collection) : base()
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }
    }
}
