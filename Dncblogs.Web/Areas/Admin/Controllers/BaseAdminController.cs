using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Dncblogs.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaseAdminController : Controller
    {
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