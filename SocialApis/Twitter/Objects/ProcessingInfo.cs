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
        public string State { get; private set; }

        [DataMember(Name = "check_after_secs")]
        public int CheckAfterSecs { get; private set; }

        [DataMember(Name = "process_psercent")]
        public int ProcessPercent { get; private set; }

        [DataMember(Name = "error")]
        public ProcessingInfo Error { get; private set; }

        [DataContract]
        public class ProcessingError
        {
            [DataMember(Name = "code")]
            public int Code { get; private set; }

            [DataMember(Name = "name")]
            public string Name { get; private set; }

            [DataMember(Name = "message")]
            public string Message { get; private set; }
        }
    }
}
