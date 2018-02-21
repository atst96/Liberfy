using System.Runtime.Serialization;

namespace SocialApis.Twitter
{
    public enum CoordinateType
    {
        [EnumMember(Value = "Point")]
        Point,

        [EnumMember(Value = "Polygon")]
        Polygon,
    }
}
