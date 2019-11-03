using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
    public class GitHubSetting
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public string authorize_uri { get; set; }
        public string redirect_uri { get; set; }
        public string oauth_authorize { get; set; }

        public string access_token { get; set; }

        public string user_api { get; set; }
    }
}
