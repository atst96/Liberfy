﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApis.Twitter.Apis
{
    public class UsersApi : TokenApiBase
    {
        internal UsersApi(Tokens tokens) : base(tokens)
        {
        }

        public Task<UserResponse> ReportSpam(string screenName)
        {
            return this.ReportSpam(screenName, true);
        }

        public Task<UserResponse> ReportSpam(string screenName, bool performBlock)
        {
            var query = new Query
            {
                ["screen_name"] = screenName,
                ["perform_block"] = performBlock,
            };

            return this.Tokens.PostRequestRestApiAsync<UserResponse>("users/report_spam", query);
        }

        public Task<UserResponse> ReportSpam(long usreId)
        {
            return this.ReportSpam(usreId, true);
        }

        public Task<UserResponse> ReportSpam(long userId, bool performBlock)
        {
            var query = new Query
            {
                ["user_id"] = userId,
                ["perform_block"] = performBlock,
            };

            return this.Tokens.PostRequestRestApiAsync<UserResponse>("users/report_spam", query);
        }
    }
}
