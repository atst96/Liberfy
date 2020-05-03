using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis
{
    public class ValueGroup : List<object>
    {
        public string SeparateText { get; private set; }

        public ValueGroup() : this(",")
        {
        }

        public ValueGroup(string separateText) : base()
        {
            this.SeparateText = separateText;
        }

        public ValueGroup(IEnumerable collection) : this(collection, ",")
        {
        }

        public ValueGroup(IEnumerable collection, string separateText) : this(separateText)
        {
            foreach(var value in collection)
            {
                this.Add(value);
            }
        }
    }
}
