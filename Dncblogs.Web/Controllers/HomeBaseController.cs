using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dncblogs.Web.Controllers
{
    public class HomeBaseController : Controller
    {
        private WebStaticConfig _webStaticConfig;

        public HomeBaseController(IOptions<WebStaticConfig> options)
        {
            this._webStaticConfig = options.Value;
        }

        protected WebStaticConfig ValueWebStaticConfig
        {
            get { return this._webStaticConfig; }
        }

        /// <summary>
        /// 根据claimType 获取Claim
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="claimType"></param>
        /// <returns>没有返回null</returns>
        protected Claim GetClaim(IEnumerable<Claim> claims, string claimType)
        {
            return claims.FirstOrDefault(p => p.Type == claimType);
        }
    }
}