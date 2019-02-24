using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Dncblogs.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using XNetCoreCommon;

namespace Dncblogs.Web.Controllers
{
    public class BlogController : ShowBaseController
    {
        private UserService _userService;
        private BlogService _blogService;
        private BlogCommentService _blogCommentService;
        private WebStaticConfig _webStaticConfig;
        private readonly ILogger _logger;


        public BlogController(UserService userService, BlogService blogService, BlogCommentService blogCommentService, IOptions<WebStaticConfig> options, ILoggerFactory loggerFactory)
        {
            this._userService = userService;
            this._blogService = blogService;
            this._blogCommentService = blogCommentService;

            this._webStaticConfig = options.Value;
            _logger = loggerFactory.CreateLogger(typeof(BlogController));

        }

        public async Task<IActionResult> Index(int? id, int? categoryId)
        {
            UserDto userDtoLogin = new UserDto();
            if (categoryId != null && categoryId != 0)
            {
                userDtoLogin = await this._userService.GetOneUserByCategoryIDAsync((int)categoryId);
            }
            else
            {
                if (id == null)
                {
                    var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
                    if (claim != null)
                    {
                        userDtoLogin.UserId = int.Parse(claim.Value);
                        claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                        userDtoLogin.UserName = claim.Value;
                        TempData["LoginUser"] = userDtoLogin;
                    }
                }
                else
                {
                    userDtoLogin = await this._userService.GetOneUserByUserIdAsync((int)id);
                    var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
                    if (claim != null)
                        TempData["LoginUser"] = userDtoLogin;

                }
            }

            ViewBag.UserId = userDtoLogin.UserId;
            ViewBag.categoryId = categoryId == null ? 0 : (int)categoryId;
            DetailContentLayoutViewModel.UserDto = userDtoLogin;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            DetailContentLayoutViewModel.UserDto = userDtoLogin;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            TempData["Title"] = $"{this._webStaticConfig.WebName}-{userDtoLogin.UserName}";
            TempData["KeyWord"] = _webStaticConfig.WebKeyWord;
            TempData["Description"] = _webStaticConfig.WebDescription;

            var userBlogCommentList = MemoryCacheTool.GetCacheValue("UserBlogCommentList");
            if (userBlogCommentList != null)
            {
                TempData["UserBlogCommentList"] = (List<BlogCommentDto>)userBlogCommentList;
            }
            else
            {
                DataResultDto<List<BlogCommentDto>> dataResultDto = await this._blogCommentService.GetBlogCommentByUserId(10);
                if (dataResultDto.Code == 0)
                    TempData["UserBlogCommentList"] = dataResultDto.DataList;

                MemoryCacheTool.SetChacheValue("UserBlogCommentList", dataResultDto.DataList, TimeSpan.FromMinutes(30));
            }

            return View();
        }

        public async Task<IActionResult> ShowBlog(int id)
        {
            //获取客户端IP
            string userIP = HttpContext.GetUserIp();
            this._logger.LogInformation($"查看用户IP： {userIP}");
            //获取具体的博客
            BlogDto blogDto = await this._blogService.GetOneBlogByBlogIdAsync(id, userIP, true);
            ViewBag.BlogDto = blogDto;
            //获取用户博客的信息（博客分类 博客信息）
            UserDto userDto = await this._userService.GetOneUserByUserIdAsync(blogDto.UserId);
            DetailContentLayoutViewModel.UserDto = userDto;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            TempData["Title"] = $"{this._webStaticConfig.WebName}-{blogDto.Title}";
            TempData["KeyWord"] = blogDto.KeyWord;
            TempData["Description"] = blogDto.Description;


            var userBlogCommentList = MemoryCacheTool.GetCacheValue("UserBlogCommentList");
            if (userBlogCommentList != null)
            {
                TempData["UserBlogCommentList"] = (List<BlogCommentDto>)userBlogCommentList;
            }
            else
            {
                DataResultDto<List<BlogCommentDto>> dataResultDto = await this._blogCommentService.GetBlogCommentByUserId(10);
                if (dataResultDto.Code == 0)
                    TempData["UserBlogCommentList"] = dataResultDto.DataList;

                MemoryCacheTool.SetChacheValue("UserBlogCommentList", dataResultDto.DataList, TimeSpan.FromMinutes(30));
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


        public async Task<IActionResult> LookBlog(int id)
        {
            //获取客户端IP
            string userIP = HttpContext.GetUserIp();
            //获取具体的博客
            BlogDto blogDto = await this._blogService.GetOneBlogByBlogIdAsync(id, userIP, false);
            ViewBag.BlogDto = blogDto;
            //获取用户博客的信息（博客分类 博客信息）
            UserDto userDto = await this._userService.GetOneUserByUserIdAsync(blogDto.UserId);
            DetailContentLayoutViewModel.UserDto = userDto;
            ViewBag.DetailContentLayoutViewModel = DetailContentLayoutViewModel;
            TempData["Title"] = $"{this._webStaticConfig.WebName}-{blogDto.Title}";
            TempData["KeyWord"] = blogDto.KeyWord;
            TempData["Description"] = blogDto.Description;


            var userBlogCommentList = MemoryCacheTool.GetCacheValue("UserBlogCommentList");
            if (userBlogCommentList != null)
            {
                TempData["UserBlogCommentList"] = (List<BlogCommentDto>)userBlogCommentList;
            }
            else
            {
                DataResultDto<List<BlogCommentDto>> dataResultDto = await this._blogCommentService.GetBlogCommentByUserId(10);
                if (dataResultDto.Code == 0)
                    TempData["UserBlogCommentList"] = dataResultDto.DataList;

                MemoryCacheTool.SetChacheValue("UserBlogCommentList", dataResultDto.DataList, TimeSpan.FromMinutes(30));
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
            return View("~/Views/Blog/ShowBlog.cshtml");
        }


        public async Task<IActionResult> ShowBlogCommnet(int id)
        {
            string userName = string.Empty;
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userName = claim.Value;
            }
            var result = await _blogCommentService.GetAll(id, userName);
            return Json(result);
        }


        public async Task<IActionResult> AddBlogCommnet(BlogCommentDto blogCommentDto)
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
                    blogCommentDto.PostId = int.Parse(claim.Value);

                if (await _blogCommentService.AddBlogComment(blogCommentDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加评论成功";

                    await _blogService.UpdateCommentNum(blogCommentDto.BlogId, 1);
                }

            }
            return Json(baseDataResultDto);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetBlogListByPage(int userID, int categoryId, string keyWord, int pageSize, int pageIndex)
        {
            DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPage(userID, categoryId, keyWord, "CreateDate desc", pageSize, pageIndex, false);
            return Json(dataResultDto);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetListByPageInChildCate(int userID, int categoryId, string keyWord, int pageSize, int pageIndex)
        {
            DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPageInChildCate(userID, categoryId, keyWord, "CreateDate desc", pageSize, pageIndex, false);
            return Json(dataResultDto);
        }
    }
}