using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Dncblogs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : BaseAdminController
    {
        private NewsService _newsService;
        private WebStaticConfig _webStaticConfig;
        public NewsController(NewsService newsService, IOptions<WebStaticConfig> options)
        {
            this._newsService = newsService;
            this._webStaticConfig = options.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Add(int? id)
        {
            ViewBag.NavTitle = "添加新闻";
            ViewBag.Btn = "添加";
            NewsDto newsDto = null;
            if (id != null)
            {
                newsDto = await this._newsService.GetOneNewsByNewsIdAsync((int)id);
                ViewBag.NavTitle = "编辑新闻";
                ViewBag.Btn = "编辑";
            }
            else
            {
                newsDto = new NewsDto();
            }
            ViewBag.News = newsDto;
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(NewsDto newsDto)
        {

            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "你的输入有误，请检查";
                return Json(baseDataResultDto);
            }

            if (newsDto.NewsId != 0)
            {
                if (await this._newsService.UpdateNews(newsDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "修改新闻成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "修改新闻失败！";
                }
            }
            else
            {
                if (await this._newsService.AddNews(newsDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加新闻成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "添加新闻失败！";
                }

            }
            return Json(baseDataResultDto);
        }



        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int newsId)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (await this._newsService.DeleteNews(newsId))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "删除新闻成功!";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "删除新闻失败!";
            }
            return Json(baseDataResultDto);
        }



        public async Task<IActionResult> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, int pageSize, int pageIndex)
        {
            var bList = await this._newsService.GetListByPage(startTime, endTime, keyWord, "Sort,CreateDate desc", pageSize, pageIndex);

            return Json(bList);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByPageTable(DateTime? startTime, DateTime? endTime, string keyWord, int limit = 15, int page = 1)
        {
            var bList = await this._newsService.GetListByPage(startTime, endTime, keyWord, "Sort,CreateDate desc", limit, page);

            return Json(bList);
        }
    }
}