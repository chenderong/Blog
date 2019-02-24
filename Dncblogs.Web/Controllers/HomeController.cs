using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Web.Models;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using XNetCoreCommon;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Dncblogs.Web.Controllers
{
    public class HomeController : HomeBaseController
    {
        private BlogService _blogService;
        private UserService _userService;
        private CategoryService _categoryService;
        private OpenSourceService _openSourceService;
        private BlogCommentService _blogCommentService;
        private NewsService _newsService;
        public HomeController(BlogService blogService, UserService userService, CategoryService categoryService,
                              OpenSourceService openSourceService, BlogCommentService blogCommentService,
                              NewsService newsService,
                              IOptions<WebStaticConfig> options) : base(options)
        {
            this._blogService = blogService;
            this._userService = userService;
            this._categoryService = categoryService;
            this._openSourceService = openSourceService;
            this._blogCommentService = blogCommentService;
            this._newsService = newsService;
        }

        public async Task<IActionResult> Index()
        {
            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            TempData["KeyWord"] = $"{ValueWebStaticConfig.WebKeyWord}";
            TempData["Description"] = $"{ValueWebStaticConfig.WebDescription}";
            ViewBag.UserDto = await this._userService.GetOneUserByUserIdAsync(ValueWebStaticConfig.BlogBackUserId);
            //统计数据
            await StatisticsCount();
            //排行的博客 新闻 
            await TopBlog();
            //获取博客阅读排行
            //var list = await this._blogService.GetTopBlogByCount(0, 0, 10, "VisitCount");

            //DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPage(0, 0, "管理", "CreateDate desc", 20, 1);
            //ViewBag.BlogList = dataResultDto.DataList;

            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDto = new UserDto();
                userDto.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDto.UserName = claim.Value;
                TempData["LoginUser"] = userDto;
            }


            return View();
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        private async Task StatisticsCount()
        {
            //获取用户数量
            var userCount = MemoryCacheTool.GetCacheValue("UserCount");
            if (userCount != null)
            {
                TempData["UserCount"] = (int)userCount;
            }
            else
            {
                int qUserCount = await this._userService.StatisticsUserCount();
                TempData["UserCount"] = qUserCount;
                MemoryCacheTool.SetChacheValue("UserCount", qUserCount, TimeSpan.FromMinutes(30));
            }
            //获取博客数量
            var blogCount = MemoryCacheTool.GetCacheValue("BlogCount");
            if (userCount != null)
            {
                TempData["BlogCount"] = (int)blogCount;
            }
            else
            {
                int qBlogCount = await this._blogService.StatisticsBlogCount(); ;
                TempData["BlogCount"] = qBlogCount;
                MemoryCacheTool.SetChacheValue("BlogCount", qBlogCount, TimeSpan.FromMinutes(30));
            }
            TempData["CmmentCount"] = 0;//this._userService.StatisticsUserCount();


            //获取用户数量
            var cmmentCount = MemoryCacheTool.GetCacheValue("CmmentCount");
            if (userCount != null)
            {
                TempData["CmmentCount"] = (int)cmmentCount;
            }
            else
            {
                int qCmmentCount = await this._blogCommentService.StatisticsCommentCount();
                TempData["CmmentCount"] = qCmmentCount;
                MemoryCacheTool.SetChacheValue("CmmentCount", qCmmentCount, TimeSpan.FromMinutes(30));
            }

            //获取开源项目数量
            var openSourceCount = MemoryCacheTool.GetCacheValue("OpenSourceCount");
            if (userCount != null)
            {
                TempData["OpenSourceCount"] = (int)openSourceCount;
            }
            else
            {
                int qOpenSourceCount = await this._openSourceService.StatisticsOpenSourceCount();
                TempData["OpenSourceCount"] = qOpenSourceCount;
                MemoryCacheTool.SetChacheValue("OpenSourceCount", qOpenSourceCount, TimeSpan.FromMinutes(30));
            }
        }

        /// <summary>
        /// 获取 排行的博客 新闻 
        /// </summary>
        /// <returns></returns>
        private async Task TopBlog()
        {
            // 最新 10 的新闻
            var topNewList = MemoryCacheTool.GetCacheValue("TOPNews");
            if (topNewList == null)
            {
                var obj = await this._newsService.GetTopNewsByCount(10, "Sort,CreateDate desc");
                MemoryCacheTool.SetChacheValue("TOPNews", obj, TimeSpan.FromMinutes(10));
                TempData["TOPNews"] = obj;
            }
            else
            {
                TempData["TOPNews"] = topNewList;
            }
            //浏览最多的博客
            var topReadBlog = MemoryCacheTool.GetCacheValue("TOPReadBlog");
            if (topReadBlog == null)
            {
                var obj = await this._blogService.GetTopBlogByCount(0, 10, "VisitCount desc");
                MemoryCacheTool.SetChacheValue("TOPReadBlog", obj, TimeSpan.FromMinutes(10));
                TempData["TOPReadBlog"] = obj;
            }
            else
            {
                TempData["TOPReadBlog"] = topReadBlog;
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetBlogListByPage(int categoryId, string keyWord, int pageSize, int pageIndex)
        {
            //await Task.Delay(5000);
            DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPage(0, categoryId, keyWord, "CreateDate desc", pageSize, pageIndex);
            return Json(dataResultDto);
        }




        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> GetListByPageInChildCate(int categoryId, string keyWord, int pageSize, int pageIndex)
        {
            DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPageInChildCate(0, categoryId, keyWord, "CreateDate desc", pageSize, pageIndex);
            return Json(dataResultDto);
        }

        [HttpPost]
        public async Task<IActionResult> GetNewsListByPage(string keyWord, int pageSize, int pageIndex)
        {
            DataResultDto<List<NewsDto>> dataResultDto = await this._newsService.GetListByPage(keyWord, "Sort,CreateDate desc", pageSize, pageIndex);
            return Json(dataResultDto);
        }

        public async Task<IActionResult> Category(int id)
        {
            if (id == 1)
            {
                TempData["news"] = true;
                TempData["ask"] = null;
            }
            else
            {
                TempData["news"] = null;
                TempData["ask"] = true;
            }

            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            var cagegory = await this._categoryService.GetOneCategoryByUserId(id, ValueWebStaticConfig.BlogBackUserId);
            ViewBag.CategoryId = id;
            ViewBag.CategoryName = cagegory.CategoryName;
            ViewBag.UserDto = await this._userService.GetOneUserByUserIdAsync(ValueWebStaticConfig.BlogBackUserId);
            //统计数据
            await StatisticsCount();
            //排行的博客 新闻 
            await TopBlog();

            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDto = new UserDto();
                userDto.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDto.UserName = claim.Value;
                TempData["LoginUser"] = userDto;
            }

            return View();
        }


        public async Task<IActionResult> News()
        {
            TempData["news"] = true;
            TempData["ask"] = null;
            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            ViewBag.CategoryId = 0;
            ViewBag.CategoryName = "新闻";
            ViewBag.UserDto = await this._userService.GetOneUserByUserIdAsync(ValueWebStaticConfig.BlogBackUserId);
            //统计数据
            await StatisticsCount();
            //排行的博客 新闻 
            await TopBlog();

            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDto = new UserDto();
                userDto.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDto.UserName = claim.Value;
                TempData["LoginUser"] = userDto;
            }

            return View();
        }

        public async Task<IActionResult> Essence(int id)
        {
            ViewBag.CategoryName = "精华";
            TempData["essence"] = true;
            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            ViewBag.CategoryId = id;
            ViewBag.UserDto = await this._userService.GetOneUserByUserIdAsync(ValueWebStaticConfig.BlogBackUserId);
            //统计数据
            await StatisticsCount();
            //排行的博客 新闻 
            await TopBlog();

            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDto = new UserDto();
                userDto.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDto.UserName = claim.Value;
                TempData["LoginUser"] = userDto;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetEssenceBlogListByPage(string keyWord, int pageSize, int pageIndex)
        {
            DataResultDto<List<BlogDto>> dataResultDto = await this._blogService.GetListByPage(0, 0, keyWord, "CreateDate desc", pageSize, pageIndex, true);
            return Json(dataResultDto);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<IActionResult> OpenSource()
        {
            ViewBag.CategoryName = "开源项目";
            TempData["open"] = true;
            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            ViewBag.UserDto = await this._userService.GetOneUserByUserIdAsync(ValueWebStaticConfig.BlogBackUserId);
            //统计数据
            await StatisticsCount();
            //排行的博客 新闻 
            await TopBlog();
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim != null)
            {
                UserDto userDto = new UserDto();
                userDto.UserId = int.Parse(claim.Value);
                claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Name);
                userDto.UserName = claim.Value;
                TempData["LoginUser"] = userDto;
            }
            return View();
        }

        public async Task<IActionResult> GetOpenSourceListByPage(string keyWord, int pageSize, int pageIndex)
        {
            //await Task.Delay(5000);
            var dataResultDto = await this._openSourceService.GetListByPage(keyWord, "CreateDate desc", pageSize, pageIndex);
            return Json(dataResultDto);
        }


        public IActionResult Error(string errorMessage = "")
        {
            TempData["Title"] = $"{ValueWebStaticConfig.WebName}-程序员的网上家园";
            TempData["Error"] = errorMessage;
            return View();
        }

        //public IActionResult Index2()
        //{
        //    return View();
        //}

    }
}
