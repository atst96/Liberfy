using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    [DataContract]
    public class ProcessingInfo
    {
        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "check_after_secs")]
        public int CheckAfterSecs { get; set; }

        [DataMember(Name = "process_psercent")]
        public int ProcessPercent { get; set; }

        [DataMember(Name = "error")]
        public ProcessingInfo Error { get; set; }

        [DataContract]
        public class ProcessingError
        {
            [DataMember(Name = "code")]
            public int Code { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }

            [DataMember(Name = "message")]
            public string Message { get; set; }
        }
    }
}
