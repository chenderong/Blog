using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dncblogs.Web.Areas.User.Controllers
{
    [Area("User")]
    public class BlogController : BaseUserController
    {
        private BlogService _blogService;
        private CategoryService _categoryService;
        private WebStaticConfig _webStaticConfig;
        public BlogController(CategoryService categoryService, BlogService blogService, IOptions<WebStaticConfig> options)
        {
            this._categoryService = categoryService;
            this._blogService = blogService;

            this._webStaticConfig = options.Value;
        }

        public async Task<IActionResult> Index()
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            ViewBag.CyList = await this._categoryService.GetAllCategoryByUserId(userID);
            return View();
        }

        public async Task<IActionResult> Add(int? id)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            ViewBag.CyList = await this._categoryService.GetAllCategoryByUserId(userID);
            ViewBag.NavTitle = "添加博客";
            ViewBag.Btn = "添加";
            BlogDto blogDto = null;
            if (id != null)
            {
                blogDto = await this._blogService.GetOneBlogByBlogIdAsync((int)id);
                ViewBag.NavTitle = "编辑博客";
                ViewBag.Btn = "编辑";
            }
            else
            {
                blogDto = new BlogDto();
            }
            ViewBag.Blog = blogDto;
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(BlogDto blogDto)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            blogDto.UserId = userID;
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "你的输入有误，请检查";
                return Json(baseDataResultDto);
            }
            //ViewBag.CyList = await this._categoryService.GetAllCategoryByUserId(1);
            if (blogDto.BlogId != 0)
            {
                if (await this._blogService.UpdateBlog(blogDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "修改博客成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "修改博客失败！";
                }
            }
            else
            {
                if (await this._blogService.AddBlog(blogDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加博客成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "添加博客失败！";
                }

            }
            return Json(baseDataResultDto);
        }

        public async Task<IActionResult> GetListByPage(DateTime? startTime, DateTime? endTime, int categoryId, string keyWord, int pageSize, int pageIndex)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            var bList = await this._blogService.GetListByPage(startTime, endTime, userID, categoryId, keyWord, "CreateDate desc", pageSize, pageIndex);

            return Json(bList);
        }


        [HttpGet]
        public async Task<IActionResult> GetListByPageTable(DateTime? startTime, DateTime? endTime, int categoryId, string keyWord, int page = 1, int limit = 15)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return Json(new DataResultDto<List<BlogDto>>());
            int userID = int.Parse(claim.Value);
            var bList = await this._blogService.GetListByPage(startTime, endTime, userID, categoryId, keyWord, "CreateDate desc", limit, page);

            return Json(bList);
        }



        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int blogId)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (await this._blogService.DeleteBlog(blogId))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "删除博客成功!";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "删除博客失败!";
            }
            return Json(baseDataResultDto);
        }



        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Essence(int blogId, bool isEssence)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (await this._blogService.EssenceBlog(blogId, isEssence))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = !isEssence ? "设置精华博客成功！" : "取消精华博客成功！";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = !isEssence ? "设置精华博客失败！" : "取消精华博客失败！";
            }
            return Json(baseDataResultDto);
        }



        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> SetHomePage(int blogId, bool isHomePage)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = !isHomePage ? "设置博客首页上显示失败！" : "取消博客首页上显示失败！";
                return Json(baseDataResultDto);
            }
            int userID = int.Parse(claim.Value);
            if (await this._blogService.SetIsHomePage(userID,blogId, isHomePage))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = !isHomePage ? "设置博客首页上显示成功！" : "取消博客首页上显示成功！";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = !isHomePage ? "设置博客首页上显示失败！" : "取消博客首页上显示失败！";
            }
            return Json(baseDataResultDto);
        }
    }
}