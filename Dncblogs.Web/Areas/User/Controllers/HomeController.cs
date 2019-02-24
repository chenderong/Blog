using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using Dncblogs.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Dncblogs.Web.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : BaseUserController
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
            ViewBag.User = userDto;

            return View();
        }

        public IActionResult Welcome()
        {
            string userName = string.Empty;
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
            if (claim != null)
                userName = claim.Value;

            TempData["UserName"] = userName;

            return View();
        }

        //public async Task<IActionResult> LoginOut()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        //    return View("/User/Signin");
        //}
    }
}