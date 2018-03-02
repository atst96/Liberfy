using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Mastodon
{
    public abstract class TokenApiBase
    {
        protected Tokens Tokens { get; }

        protected TokenApiBase(Tokens tokens)
        {
            this.Tokens = tokens;
        }
    }
}
