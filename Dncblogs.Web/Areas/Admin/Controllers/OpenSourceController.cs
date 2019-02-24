using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;

namespace Dncblogs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OpenSourceController : BaseAdminController
    {
        private OpenSourceService _openSourceService;

        public OpenSourceController(OpenSourceService openSourceService)
        {
            this._openSourceService = openSourceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Add(int? id)
        {
            ViewBag.NavTitle = "添加开源项目";
            ViewBag.Btn = "添加";
            OpenSourceDto openSourceDto = null;
            if (id != null)
            {
                openSourceDto = await this._openSourceService.GetOneOpensourceIdAsync((int)id);
                ViewBag.NavTitle = "编辑开源项目";
                ViewBag.Btn = "编辑";
            }
            else
            {
                openSourceDto = new OpenSourceDto();
            }
            ViewBag.OpenSourceDto = openSourceDto;
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(OpenSourceDto openSourceDto)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "你的输入有误，请检查";
                return Json(baseDataResultDto);
            }
            if (openSourceDto.OpenSourceID != 0)
            {
                if (await this._openSourceService.UpdateOpensource(openSourceDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "修改开源项目成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "修改开源项目失败！";
                }
            }
            else
            {
                if (await this._openSourceService.AddOpensource(openSourceDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加开源项目成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "添加开源项目失败！";
                }
            }
            return Json(baseDataResultDto);
        }




        public async Task<IActionResult> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, int pageSize, int pageIndex)
        {
            var bList = await this._openSourceService.GetListByPage(startTime, endTime, keyWord, "CreateDate desc", pageSize, pageIndex);
            return Json(bList);
        }


        [HttpGet]
        public async Task<IActionResult> GetListByPageTable(DateTime? startTime, DateTime? endTime, string keyWord, int limit = 15, int page = 1)
        {
            var bList = await this._openSourceService.GetListByPage(startTime, endTime, keyWord, "CreateDate desc", limit, page);
            return Json(bList);
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int openSourceID)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (await this._openSourceService.DeleteOpensource(openSourceID))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "删除开源项目成功!";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "删除开源项目失败!";
            }
            return Json(baseDataResultDto);
        }

    }
}