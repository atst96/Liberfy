using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public abstract class TokenApiBase
    {
        protected Tokens Tokens { get; }

        internal TokenApiBase(Tokens tokens)
        {
            this.Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }
    }
}
