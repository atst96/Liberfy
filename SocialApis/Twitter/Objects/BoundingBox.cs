using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class BoundingBox
    {
        private BoundingBox()
        {
            Coordinates = new Coordinates[0];
        }

        [DataMember(Name = "coordinates")]
        private long[][][] _coordinates
        {
            get => throw new NotImplementedException();
            set
            {

            }
        }

        [IgnoreDataMember]
        public Coordinates[] Coordinates { get; private set; }
    }
}
