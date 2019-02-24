using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Web.Models;
using System.Security.Claims;

namespace Dncblogs.Web.Controllers
{
    public class ShowBaseController : Controller
    {
        public DetailContentLayoutViewModel DetailContentLayoutViewModel { get; set; }

        public ShowBaseController()
        {
            DetailContentLayoutViewModel = new DetailContentLayoutViewModel();
            //ViewBag.BlogLayoutViewModel = BlogLayoutViewModel;
            //TempData["BlogLayoutViewModel"] = BlogLayoutViewModel;
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