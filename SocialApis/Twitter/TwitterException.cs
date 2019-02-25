using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter
{
    public sealed class TwitterException : Exception
    {
        internal TwitterException(string message)
            : base(message)
        {
            this.Errors = new TwitterError[0];
        }

        private TwitterException(WebException wex, TwitterError[] errors)
            : base(wex.Message, wex)
        {
            this.Errors = errors ?? new TwitterError[0];
        }

        public TwitterError[] Errors { get; }

        internal static TwitterException FromWebException(WebException wex)
        {
            using (var response = wex.Response.GetResponseStream())
            {
                try
                {
                    var errors = JsonUtility.Deserialize<TwitterErrorContainer>(response);

                    return new TwitterException(wex, errors.Errors);
                }
                catch (Utf8Json.JsonParsingException ex)
                {
                    response.Position = 0;

                    var message = ex.Message;

                    using (var reader = new StreamReader(response, EncodingUtility.UTF8))
                    {
                        message += "\n\n" + reader.ReadToEnd();
                    }

                    throw new Exception(message, ex);
                }
            }
        }
    }
}
