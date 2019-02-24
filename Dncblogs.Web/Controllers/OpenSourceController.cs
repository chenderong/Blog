using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Core;
using Microsoft.Extensions.Options;
using Dncblogs.Domain.Models;
using Dncblogs.Domain.EntitiesDto;
using System.Security.Claims;

namespace Dncblogs.Web.Controllers
{
    public class OpenSourceController : Controller
    {
        private UserService _userService;
        private OpenSourceService _openSourceServic;
        private WebStaticConfig _webStaticConfig;
        public OpenSourceController(OpenSourceService openSourceService, UserService userService, IOptions<WebStaticConfig> options)
        {
            this._openSourceServic = openSourceService;
            this._userService = userService;
            this._webStaticConfig = options.Value;
        }

        /// <summary>
        /// 根据claimType 获取Claim
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="claimType"></param>
        /// <returns>没有返回null</returns>
        private Claim GetClaim(IEnumerable<Claim> claims, string claimType)
        {
            return claims.FirstOrDefault(p => p.Type == claimType);
        }

        public async Task<IActionResult> ShowOpenSource(int id)
        {
            //登陆用户
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
                if (claim != null)
                {
                    UserDto userDtoLogin = new UserDto();
                    userDtoLogin.UserId = int.Parse(claim.Value);
                    claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                    userDtoLogin.UserName = claim.Value;
                    TempData["LoginUser"] = userDtoLogin;
                }
            }
            //获取具体开源项目
            OpenSourceDto openSourceDto = await this._openSourceServic.GetOneOpensourceIdAsync(id);
            ViewBag.OpenSourceDto = openSourceDto;
            //获取管理员
            UserDto userDto = await this._userService.GetOneUserByUserIdAsync(this._webStaticConfig.BlogBackUserId);
            ViewBag.UserDto = userDto;
            //热门开源项目
            var dataResultDto = await this._openSourceServic.GetList(1, 15);
            if (dataResultDto.Code == 0)
                ViewData["HeatOpenSource"] = dataResultDto.DataList;
            //开源项目
            dataResultDto = await this._openSourceServic.GetList(0, 15);
            if (dataResultDto.Code == 0)
                ViewData["OpenSource"] = dataResultDto.DataList;
            ViewData["Title"] = $"{this._webStaticConfig.WebName}-{openSourceDto.OpenSourceTitle}";
            ViewData["KeyWord"] = $"开源,github,码云,.net core,net,c#,java,python";
            ViewData["Description"] = openSourceDto.OpenSourceDescribe;
            return View();
        }
    }
}