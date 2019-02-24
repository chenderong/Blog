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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using XNetCoreCommon;

namespace Dncblogs.Web.Controllers
{
    public class UserController : Controller
    {
        const int MaxCount = 5;
        const int MaxMinute = 60;

        readonly ILogger logger;
        private UserService _userService;
        private WebStaticConfig _webStaticConfig;
        public UserController(UserService userService, ILoggerFactory loggerFactory, IOptions<WebStaticConfig> options)
        {
            this._userService = userService;
            this._webStaticConfig = options.Value;
            logger = loggerFactory.CreateLogger(typeof(UserController));

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Signin()
        {
            ViewBag.WebStaticConfig = this._webStaticConfig;
            return View();
        }

        public async Task<IActionResult> SingOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            DataResultDto<string> dataResultDto = new DataResultDto<string>();
            try
            {
                if (!ModelState.IsValid)
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "请输入正确的用户名和密码";
                    dataResultDto.DataList = string.Empty;
                    return Json(dataResultDto);
                }
                //string vCode = HttpContext.Session.GetString("LoginValidateCode");
                //logger.LogDebug($"zhongzw sessionid:{HttpContext.Session.Id},{vCode}");
                //if (string.IsNullOrEmpty(vCode))
                //{
                //    dataResultDto.Code = 1;
                //    dataResultDto.Msg = "验证码已过期，请重新刷新！";
                //    dataResultDto.DataList = string.Empty; 
                //    return Json(dataResultDto);
                //}
                //if (vCode.ToUpper().Trim() != userDto.ChcekCode.ToUpper().Trim())
                //{
                //    dataResultDto.Code = 1;
                //    dataResultDto.Msg = "验证码不正确，请重新输入！";
                //    dataResultDto.DataList = string.Empty;
                //    return Json(dataResultDto);
                //}
                var resultDto = await this._userService.CheckUser(new Domain.EntitiesDto.UserDto() { LoginName = userDto.LoginName, Password = userDto.Password });
                if (resultDto.Code == 0)
                {
                    var loginUser = resultDto.DataList;
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, loginUser.UserId.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, loginUser.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, loginUser.IsAdmin ? "Admin" : "User"));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    dataResultDto.Code = 0;
                    dataResultDto.Msg = resultDto.Msg;
                    if (loginUser.IsAdmin)
                        dataResultDto.DataList = "/Admin/Home/Index";
                    else
                        dataResultDto.DataList = "/User/Home/Index";
                }
                else
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = resultDto.Msg;
                }
            }
            catch
            {
                dataResultDto.Code = 1;
                dataResultDto.Msg = "登陆出现异常，请联系管理员！";
            }
            return Json(dataResultDto);
        }

        public IActionResult Signin1()
        {
            return View();
        }

        public IActionResult Register()
        {
            ViewBag.WebStaticConfig = this._webStaticConfig;
            return View();
        }


        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            DataResultDto<string> dataResultDto = new DataResultDto<string>();
            try
            {
                if (!ModelState.IsValid)
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "输入的格式不对";
                    dataResultDto.DataList = string.Empty;
                    return Json(dataResultDto);
                }
                //string vCode = HttpContext.Session.GetString("LoginValidateCode");
                //if (string.IsNullOrEmpty(vCode))
                //{
                //    dataResultDto.Code = 1;
                //    dataResultDto.Msg = "验证码已过期，请重新刷新！";
                //    dataResultDto.DataList = string.Empty;
                //    return Json(dataResultDto);
                //}
                //if (vCode.ToUpper().Trim() != userDto.ChcekCode.ToUpper().Trim())
                //{
                //    dataResultDto.Code = 1;
                //    dataResultDto.Msg = "验证码不正确，请重新输入！";
                //    dataResultDto.DataList = string.Empty;
                //    return Json(dataResultDto);
                //}
                if (await this._userService.CheckLoginNameAsync(userDto.LoginName))
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "登陆用户名已经存在，请重新输入！";
                    dataResultDto.DataList = string.Empty;
                    return Json(dataResultDto);
                }
                userDto.LoginName = userDto.LoginName.Trim();
                string userIP = HttpContext.GetUserIp();
                var mObj = MemoryCacheTool.GetCacheValue(userIP);
                if (mObj == null)
                {
                    var userRegister = new UserRegister();
                    mObj = userRegister;
                    MemoryCacheTool.SetChacheValue(userIP, userRegister, TimeSpan.FromMinutes(120));
                }
                else
                {
                    var ur = (UserRegister)mObj;
                    if (ur.RegisterCount > MaxCount && DateTime.Now.Subtract(ur.RegisterTime).TotalMinutes <= MaxMinute)
                    {
                        dataResultDto.Code = 1;
                        dataResultDto.Msg = "你的IP注册次数过多！";
                        dataResultDto.DataList = string.Empty;
                        return Json(dataResultDto);
                    }
                }

                if (await this._userService.AddUserAsync(userDto))
                {
                    userDto = await this._userService.GetUserByLoginNameAsync(userDto.LoginName);
                    if (userDto == null)
                    {
                        dataResultDto.Code = 1;
                        dataResultDto.Msg = "注册程序异常，请联系管理员！";
                        return Json(dataResultDto);
                    }

                    var ur = (UserRegister)mObj;
                    ur.ClientIP = userIP;
                    ur.RegisterCount++;
                    ur.RegisterTime = DateTime.Now;

                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid, userDto.UserId.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.Name, userDto.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    dataResultDto.Code = 0;
                    dataResultDto.Msg = "注册成功！";
                    dataResultDto.DataList = "/User/Home/Index";
                }
                else
                {
                    dataResultDto.Code = 1;
                    dataResultDto.Msg = "注册失败！";
                }
            }
            catch (Exception ex)
            {
                logger.LogError("注册失败", ex);
                dataResultDto.Code = 1;
                dataResultDto.Msg = "注册程序异常，请联系管理员！";
            }
            return Json(dataResultDto);
        }

        public async Task<IActionResult> LoginOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Signin");
        }
    }
}