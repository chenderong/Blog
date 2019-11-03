using Dapper;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Dncblogs.Core
{
    public class GitHubService : OauthuserService
    {
        private WebStaticConfig _webStaticConfig;
        public GitHubService(IOptions<DatabaseSetting> options, IOptions<WebStaticConfig> optionw) : base(options, optionw)
        {
            this._webStaticConfig = optionw.Value;
        }
        public override async Task<OauthUser> CheckOauthUser(long id, string loginID)
        {
            string sql = "select o.UserId,o.GitHub_login `LoginID`,o.GitHub_id Id from `oauthuser` o  where o.GitHub_login=@LoginID or o.GitHub_id=@id ";
            using (var connect = CreateConnection())
            {
                return await connect.QueryFirstOrDefaultAsync<OauthUser>(sql, new { LoginID = loginID.Trim(), id = id });
            }
            return null;
        }

        public override async Task<OauthUser> GetOauthUser(long id, string loginID)
        {
            string sql = "select o.GitHub_login `LoginID`,o.GitHub_id Id,u.* from `oauthuser` o left join `user` u on u.UserId = o.UserId where o.GitHub_login=@LoginID or o.GitHub_id=@id ";
            using (var connect = CreateConnection())
            {
                var users = await connect.QueryAsync<OauthUser, User, OauthUser>(sql, (oauthUser, user) =>
                {
                    if (user!=null)
                    {
                        oauthUser.UserId = user.UserId;
                        oauthUser.User = user;
                    }
                    return oauthUser;

                }, new { LoginID = loginID.Trim(), id = id }, splitOn: "UserId");

                if (users != null)
                    return users.FirstOrDefault();
            }

            return null;
        }

        public override async Task<bool> AddOauthUserAsync(OauthUser oauthUser)
        {
            string sql = "insert into `oauthuser`(UserId,GitHub_login,`GitHub_id`) VALUES(@UserId,@GitHub_login,@GitHub_id) ";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { UserId = oauthUser.UserId, GitHub_login = oauthUser.LoginID, GitHub_id = oauthUser.Id }) > 0;
            }
        }

        public override async Task<bool> UpdateOauthUserAsync(long userId,string loginID)
        {
            string sql = "update `oauthuser` set UserId=@UserId where GitHub_login=@GitHub_login ";
            using (var connect = CreateConnection())
            {
                return await connect.ExecuteAsync(sql, new { UserId = userId, GitHub_login = loginID }) > 0;
            }
        }
        
    }
}
