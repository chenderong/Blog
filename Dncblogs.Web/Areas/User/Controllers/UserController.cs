using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Core;
using System.Security.Claims;
using Dncblogs.Domain.EntitiesDto;

namespace Dncblogs.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UserController : BaseUserController
    {
        private UserService _userService;
        public UserController(UserService userService)
        {
            this._userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            ViewBag.User = await this._userService.GetOneUserByUserIdAsync(userID);
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(UserDto userDto)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            userDto.UserId = userID;
            if (await this._userService.UpdateUserAsync(userDto))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "修改成功！";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "修改失败！";
            }
            return Json(baseDataResultDto);
        }
    }
}