using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    [DataContract]
    public class IdObject
    {
        [DataMember(Name = "id")]
        private long _id;
        public long Id => _id;
    }

    [DataContract]
    public class IdObject<T>
    {
        [DataMember(Name = "id")]
        private T _id;
        public T Id => _id;
    }
}
