using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Microsoft.AspNetCore.Mvc;

namespace Dncblogs.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NoteController : BaseAdminController
    {
        private NoteService _noteService;

        public NoteController(NoteService _noteService)
        {
            this._noteService = _noteService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Add(int? id)
        {
            ViewBag.NavTitle = "添加随笔";
            ViewBag.Btn = "添加";
            NoteDto  noteDto  = null;
            if (id != null)
            {
                noteDto = await this._noteService.GetOneNoteAsync((int)id);
                ViewBag.NavTitle = "编辑随笔";
                ViewBag.Btn = "编辑";
            }
            else
            {
                noteDto = new NoteDto();
            }
            ViewBag.NoteDto = noteDto;
            return View();
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Add(NoteDto  noteDto)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            noteDto.UserId = userID;
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "你的输入有误，请检查";
                return Json(baseDataResultDto);
            }
            if (noteDto.NoteID != 0)
            {
                if (await this._noteService.UpdateNote(noteDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "修改随笔成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "修改开源项目失败！";
                }
            }
            else
            {
                if (await this._noteService.AddNote(noteDto))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加随笔成功！";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "添加随笔失败！";
                }
            }
            return Json(baseDataResultDto);
        }




        public async Task<IActionResult> GetListByPage(DateTime? startTime, DateTime? endTime, string keyWord, int pageSize, int pageIndex)
        {
            var bList = await this._noteService.GetListByPage(startTime, endTime, keyWord, "CreateDate desc", pageSize, pageIndex);
            return Json(bList);
        }


        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(int noteID)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (await this._noteService.DeleteNote(noteID))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "删除随笔成功!";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "删除随笔失败!";
            }
            return Json(baseDataResultDto);
        }

    }
}