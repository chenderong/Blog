using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncblogs.Core;
using Dncblogs.Domain.Entities;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dncblogs.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OauthController : Controller
    {
        private WebStaticConfig _webStaticConfig;

        private GitHubSetting _gitHubSetting;
        private IHttpClientFactory _httpClientFactory;

        private readonly ILogger _logger;
        private GitHubService _gitHubService;
        private UserService _userService;
        public OauthController(IOptions<WebStaticConfig> webStaticConfig,IOptions<GitHubSetting> gitHubSetting, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory, GitHubService gitHubService, UserService userService)
        {
            this._webStaticConfig = webStaticConfig.Value;
            this._gitHubSetting = gitHubSetting.Value;
            this._httpClientFactory = httpClientFactory;
            _logger = loggerFactory.CreateLogger(typeof(OauthController));
            this._gitHubService = gitHubService;
            this._userService = userService;
        }



        public async Task<IActionResult> GitHubRedirect([FromQuery]string code)
        {
            try
            {
                ViewBag.WebStaticConfig = this._webStaticConfig;
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("accept", "application/json");
                string url_access_token = string.Format(this._gitHubSetting.access_token, this._gitHubSetting.client_id, this._gitHubSetting.client_secret, code);
                var result = await client.GetAsync(url_access_token);
                ViewBag.IsError = true;
                ViewBag.ErrorMessage = "验证失败";
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //{"access_token":"39d8f82e4a67b4cbfd011520264188fd48898f70","token_type":"bearer","scope":""}
                    string resultDataString = await result.Content.ReadAsStringAsync();
                    var jsobj = JsonConvert.DeserializeObject<JObject>(resultDataString);
                    string access_token = jsobj["access_token"].ToString();
                    //string token_type = jsobj["token_type"].ToString();
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"bearer {access_token}");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.0.3) Gecko/2008092417 Firefox/3.0.3");
                    string user_api = string.Format(this._gitHubSetting.user_api, access_token);
                    result = await client.GetAsync(user_api);//获取github 用户信息
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        resultDataString = await result.Content.ReadAsStringAsync();
                        var gitHubUser = JsonConvert.DeserializeObject<GitHubUser>(resultDataString);
                        var oauthUser = await this._gitHubService.GetOauthUser(gitHubUser.id, gitHubUser.login);
                        if (oauthUser != null && oauthUser.UserId != 0)//不存在关联  管理github
                        {
                            await SetIdentity(oauthUser.User.UserId, oauthUser.User.UserName, oauthUser.User.IsAdmin);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            oauthUser = await this._gitHubService.CheckOauthUser(gitHubUser.id, gitHubUser.login);
                            if (oauthUser == null || oauthUser.Id == 0)
                            {
                                await this._gitHubService.AddOauthUserAsync(new OauthUser { Id = gitHubUser.id, LoginID = gitHubUser.login });
                            }
                            ViewBag.GitHubUser = gitHubUser;
                        }
                        ViewBag.IsError = false;
                    }
                    else
                    {
                        resultDataString = await result.Content.ReadAsStringAsync();
                        ViewBag.ErrorMessage = "验证失败";
                        ViewBag.GitHubUser = new GitHubUser();
                        _logger.LogInformation(resultDataString);
                    }
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.IsError = true;
                ViewBag.ErrorMessage = "验证出现异常，请联系管理员";
                ViewBag.GitHubUser = new GitHubUser();
                _logger.LogError(ex, "GitHub登录异常");
            }

            return View();
        }

        private async Task SetIdentity(long userId, string userName, bool isAdmin)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Sid, userId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

        [HttpPost]
        public async Task<IActionResult> BindingAccount(BindingUserDto bindingUserDto)
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

                if (bindingUserDto.Type == 0)
                {
                    var resultDto = await this._userService.CheckUser(new Domain.EntitiesDto.UserDto() { LoginName = bindingUserDto.LoginName, Password = bindingUserDto.Password });
                    if (resultDto.Code == 0)
                    {
                        var loginUser = resultDto.DataList;
                        await this._gitHubService.UpdateOauthUserAsync(loginUser.UserId, bindingUserDto.LoginID);
                        await SetIdentity(loginUser.UserId, loginUser.UserName, loginUser.IsAdmin);
                        dataResultDto.Code = 0;
                        dataResultDto.Msg = resultDto.Msg;
                        dataResultDto.DataList = "/Home/Index";
                    }
                    else
                    {
                        dataResultDto.Code = 1;
                        dataResultDto.Msg = "绑定失败，用户名或者密码不对";
                    }
                }
                else
                {
                    if (await this._userService.CheckLoginNameAsync(bindingUserDto.LoginName))
                    {
                        dataResultDto.Code = 1;
                        dataResultDto.Msg = "登陆用户名已经存在，请重新输入！";
                        dataResultDto.DataList = string.Empty;
                        return Json(dataResultDto);
                    }

                    await this._userService.AddUserAsync(new UserDto() { LoginName = bindingUserDto.LoginName, UserName = bindingUserDto.UserName, Password = bindingUserDto.Password });
                    var resultDto = await this._userService.CheckUser(new Domain.EntitiesDto.UserDto() { LoginName = bindingUserDto.LoginName, Password = bindingUserDto.Password });
                    var loginUser = resultDto.DataList;
                    await this._gitHubService.UpdateOauthUserAsync(loginUser.UserId, bindingUserDto.LoginID);
                    await SetIdentity(loginUser.UserId, loginUser.UserName, loginUser.IsAdmin);
                    dataResultDto.Code = 0;
                    dataResultDto.Msg = resultDto.Msg;
                    dataResultDto.DataList = "/Home/Index";
                }
            }
            catch (Exception ex)
            {
                dataResultDto.Code = 1;
                dataResultDto.Msg = "绑定失败，请联系管理员！";
                _logger.LogError(ex, "登陆出现异常");
            }
            return Json(dataResultDto);
        }
    }
}