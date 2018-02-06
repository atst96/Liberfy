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
        private string _state;
        public string State => _state;

        [DataMember(Name = "check_after_secs")]
        private int _checkAfterSecs;
        public int CheckAfterSecs => _checkAfterSecs;

        [DataMember(Name = "process_psercent")]
        private int _processPercent;
        public int ProcessPercent => _processPercent;

        [DataMember(Name = "error")]
        private ProcessingInfo _error;
        public ProcessingInfo Error => _error;

        [DataContract]
        public class ProcessingError
        {
            [DataMember(Name = "code")]
            private int _code;
            public int Code => _code;

            [DataMember(Name = "name")]
            private string _name;
            public string Name => _name;

            [DataMember(Name = "message")]
            private string _message;
            public string Message => _message;
        }
    }
}
