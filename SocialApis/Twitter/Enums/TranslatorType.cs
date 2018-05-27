using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    // TODO: どの種類があるのかわからないのでObject.Userには未実装
    public enum TranslatorType
    {
        [EnumMember(Value = "none")]
        None,

        [EnumMember(Value = "regular")]
        Regular,
    }
}
