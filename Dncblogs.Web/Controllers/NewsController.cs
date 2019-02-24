using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XNetCoreCommon;

namespace Dncblogs.Web.Controllers
{
    public class NewsController : ShowBaseController
    {
        private NewsService _newsService;
        private UserService _userService;
        private NewsCommentService _newsCommentService;
        private WebStaticConfig _webStaticConfig;

        public NewsController(NewsService newsService, UserService userService, NewsCommentService newsCommentService, IOptions<WebStaticConfig> options)
        {
            this._newsService = newsService;
            this._userService = userService;
            this._newsCommentService = newsCommentService;

            this._webStaticConfig = options.Value;

        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> NewDetails(int id)
        {
            //获取客户端IP
            string userIP = HttpContext.GetUserIp();
            //获取具体的博客
            NewsDto newsDto = await this._newsService.GetOneNewsByNewsIdAsync(id, userIP, true);
            ViewBag.NewsDto = newsDto;
            //获取用户博客的信息（博客分类 博客信息）
            UserDto userDto = await this._userService.GetOneUserByUserIdAsync(this._webStaticConfig.BlogBackUserId);
            DetailContentLayoutViewModel.UserDto = userDto;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            TempData["Title"] = $"{this._webStaticConfig.WebName}-{newsDto.Title}";
            TempData["KeyWord"] = newsDto.KeyWord;
            TempData["Description"] = newsDto.Description;

            TempData["IsNews"] = true;
            var userNewsCommentList = MemoryCacheTool.GetCacheValue("UserNewsCommentList");
            if (userNewsCommentList != null)
            {
                TempData["UserNewsCommentList"] = (List<NewsCommentDto>)userNewsCommentList;
            }
            else
            {
                DataResultDto<List<NewsCommentDto>> dataResultDto = await this._newsCommentService.GetNewsCommentByUserId(10);
                if (dataResultDto.Code == 0)
                    TempData["UserNewsCommentList"] = dataResultDto.DataList;

                MemoryCacheTool.SetChacheValue("UserNewsCommentList", dataResultDto.DataList, TimeSpan.FromMinutes(30));
            }



            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDtoLogin = new UserDto();
                userDtoLogin.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDtoLogin.UserName = claim.Value;
                TempData["LoginUser"] = userDtoLogin;
            }

            return View();
        }

        public async Task<IActionResult> LookNews(int id)
        {
            //获取客户端IP
            string userIP = HttpContext.GetUserIp();
            //获取具体的博客
            NewsDto newsDto = await this._newsService.GetOneNewsByNewsIdAsync(id, userIP, false);
            ViewBag.NewsDto = newsDto;
            //获取用户博客的信息（博客分类 博客信息）
            UserDto userDto = await this._userService.GetOneUserByUserIdAsync(this._webStaticConfig.BlogBackUserId);
            DetailContentLayoutViewModel.UserDto = userDto;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            TempData["Title"] = $"{this._webStaticConfig.WebName}-{newsDto.Title}";
            TempData["KeyWord"] = newsDto.KeyWord;
            TempData["Description"] = newsDto.Description;

            TempData["IsNews"] = true;
            var userNewsCommentList = MemoryCacheTool.GetCacheValue("UserNewsCommentList");
            if (userNewsCommentList != null)
            {
                TempData["UserNewsCommentList"] = (List<NewsCommentDto>)userNewsCommentList;
            }
            else
            {
                DataResultDto<List<NewsCommentDto>> dataResultDto = await this._newsCommentService.GetNewsCommentByUserId(10);
                if (dataResultDto.Code == 0)
                    TempData["UserNewsCommentList"] = dataResultDto.DataList;

                MemoryCacheTool.SetChacheValue("UserNewsCommentList", dataResultDto.DataList, TimeSpan.FromMinutes(30));
            }



            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDtoLogin = new UserDto();
                userDtoLogin.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDtoLogin.UserName = claim.Value;
                TempData["LoginUser"] = userDtoLogin;
            }

            return View("~/Views/News/NewDetails.cshtml");
        }

        public async Task<IActionResult> ShowNewsCommnet(int id)
        {
            string userName = string.Empty;
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userName = claim.Value;
            }
            var result = await this._newsCommentService.GetAll(id, userName);
            return Json(result);
        }


        public async Task<IActionResult> AddNewsCommnet(NewsCommentDto newsCommentDto)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            baseDataResultDto.Code = 1;
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "评论的格式不对";
            }
            else
            {
                //获取登录用户的userId
                var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
                if (claim != null)
                    newsCommentDto.PostId = int.Parse(claim.Value);

                if (await this._newsCommentService.AddNewsComment(newsCommentDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加评论成功";

                    await _newsService.UpdateCommentNum(newsCommentDto.NewsId, 1);
                }

            }
            return Json(baseDataResultDto);
        }

    }
}