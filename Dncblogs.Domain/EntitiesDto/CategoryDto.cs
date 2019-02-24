using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class CategoryDto
    {
        /// <summary>
        /// 主键自增
        /// </summary>
        public int CategoryId { set; get; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Required(ErrorMessage = "分类名称名称不能为空")]
        [MaxLength(50, ErrorMessage = "分类名称名称不能超过50个字符")]
        public string CategoryName { set; get; } = "";

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 博客数量
        /// </summary>
        public int BlogCount { get; set; }

        /// <summary>
        /// 父类ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 子分类
        /// </summary>
        public List<CategoryDto> ChildCategoryDtoList
        {
            get => childCategoryDtoList;
            set => childCategoryDtoList = value;
        }

        private List<CategoryDto> childCategoryDtoList = new List<CategoryDto>();



    }
}
