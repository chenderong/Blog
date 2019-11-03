using Dapper;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dncblogs.Core
{
    public class OauthuserService : BaseService
    {
        private WebStaticConfig _webStaticConfig;
        public OauthuserService(IOptions<DatabaseSetting> options, IOptions<WebStaticConfig> optionw) : base(options)
        {
            this._webStaticConfig = optionw.Value;
        }

        /// <summary>
        /// 检查第三方用户是否在数据库中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<OauthUser> CheckOauthUser(long id, string loginID)
        {
            return null;
        }


        /// <summary>
        /// 检查第三方用户是否在数据库中
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<OauthUser> GetOauthUser(long id, string loginID)
        {
            return null;
        }


        /// <summary>
        /// 添加第三方认证用户信息
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddOauthUserAsync(OauthUser  oauthUser)
        {
            return false;
        }

        public virtual async Task<bool> UpdateOauthUserAsync(long userId, string loginID)
        {
            return false;
        }

    }
}
