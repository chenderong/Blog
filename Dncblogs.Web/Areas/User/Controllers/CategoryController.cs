using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dncblogs.Core;
using Dncblogs.Domain.EntitiesDto;
using Dncblogs.Domain.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dncblogs.Web.Areas.User.Controllers
{
    [Area("User")]
    public class CategoryController : BaseUserController
    {

        private CategoryService _categoryService;
        private WebStaticConfig _webStaticConfig;

        public CategoryController(CategoryService categoryService, IOptions<WebStaticConfig> options)
        {
            this._categoryService = categoryService;
            this._webStaticConfig = options.Value;

        }

        public async Task<IActionResult> Index()
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();
            int userID = int.Parse(claim.Value);
            ViewBag.TreeNodeList = JsonConvert.SerializeObject(await this._categoryService.GetNodeListByUserIdAsync(userID));
            ViewBag.CyList = await this._categoryService.GetAllCategoryByUserId(userID);
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Add(CategoryDto categorysDto)
        {
            var claim = GetClaim(HttpContext.User.Claims, ClaimTypes.Sid);
            if (claim == null)
                return NotFound();

            int userID = int.Parse(claim.Value);
            categorysDto.UserId = userID;
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "输入的格式不对";
            }
            else
            {
                CategoryDto pcategory = null;
                if (categorysDto.ParentId != 0)
                {
                    pcategory = await this._categoryService.GetOneCategoryByUserId(categorysDto.ParentId, categorysDto.UserId);
                    if (pcategory == null)
                    {
                        baseDataResultDto.Code = 1;
                        baseDataResultDto.Msg = "选择的父类不存在";
                        return Json(baseDataResultDto);
                    }
                }
                CategoryDto cy = new CategoryDto();
                cy.ParentId = categorysDto.ParentId;
                cy.CategoryName = categorysDto.CategoryName;
                cy.Sort = categorysDto.Sort;
                cy.UserId = categorysDto.UserId;
                if (await this._categoryService.AddCategory(cy))
                {
                    baseDataResultDto.Code = 0;
                    baseDataResultDto.Msg = "添加成功";
                }
                else
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "添加失败";
                }
            }
            return Json(baseDataResultDto);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(int categoryId, int userId)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();
            var cy = await this._categoryService.GetOneCategoryByUserId(categoryId, userId);
            if (cy.CategoryId == 0)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "该分类不存在";
                return Json(baseDataResultDto);
            }

            if (await this._categoryService.DeleteCategory(userId, categoryId))
            {
                baseDataResultDto.Code = 0;
                baseDataResultDto.Msg = "删除成功";
            }
            else
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "删除失败";
            }
            return Json(baseDataResultDto);

        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Modify(CategoryDto categoryDto)
        {
            BaseDataResultDto baseDataResultDto = new BaseDataResultDto();


            if (!ModelState.IsValid)
            {
                baseDataResultDto.Code = 1;
                baseDataResultDto.Msg = "输入的格式不对";
            }
            else
            {
                CategoryDto pcategory = null;
                if (categoryDto.ParentId != 0)
                {
                    pcategory = await this._categoryService.GetOneCategoryByUserId(categoryDto.CategoryId, categoryDto.UserId);
                    if (pcategory.CategoryId == 0)
                    {
                        baseDataResultDto.Code = 1;
                        baseDataResultDto.Msg = "选择的父类不存在";
                        return Json(baseDataResultDto);
                    }
                }

                var cy = await this._categoryService.GetOneCategoryByUserId(categoryDto.CategoryId, categoryDto.UserId); ;
                if (cy == null)
                {
                    baseDataResultDto.Code = 1;
                    baseDataResultDto.Msg = "选择的分类不存在";
                }
                else
                {
                    if (await this._categoryService.UpdateCategory(categoryDto))
                    {
                        baseDataResultDto.Code = 0;
                        baseDataResultDto.Msg = "修改成功";
                    }
                    else
                    {
                        baseDataResultDto.Code = 1;
                        baseDataResultDto.Msg = "修改失败";
                    }
                }

            }

            return Json(baseDataResultDto);
        }
    }
}