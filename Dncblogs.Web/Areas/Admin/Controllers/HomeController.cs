using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Dncblogs.Domain.Models;
using System.Security.Claims;
using Dncblogs.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dncblogs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseAdminController
    {
        private WebStaticConfig _webStaticConfig;
        private UserService _userService;

        public HomeController(UserService userService, IOptions<WebStaticConfig> options)
        {
            this._userService = userService;
            this._webStaticConfig = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            TempData["Title"] = $"{this._webStaticConfig.WebName}-后台管理";

            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);

            var userDto = await this._userService.GetOneUserByUserIdAsync(userID);
            ViewBag.AdminUser = userDto;
            // return View("Index2");
            return View();
        }

        public IActionResult Welcome()
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            string userName = string.Empty;
            claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
            if (claim != null)
                userName = claim.Value;

            TempData["UserName"] = userName;

            return View();
        }

 
        
    }
}