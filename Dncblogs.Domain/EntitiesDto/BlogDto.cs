using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
  public  class BlogDto
    {
        /// <summary>
        /// 博客ID 
        /// </summary>
        public long BlogId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 博客名称
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }


        /// <summary>
        /// 博客内容
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// 博客内容 截取
        /// </summary>
        public string BodyAbs { get; set; }

        /// <summary>
        /// 访问人数
        /// </summary>
        public int VisitCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public int CategoryId { set; get; }


        public CategoryDto CategoryDto { get; set; }

        /// <summary>
        /// 是否在首页上显示
        /// </summary>
        public bool IsHomePage { get; set; }

        /// <summary>
        /// 是否是精华
        /// </summary>
        public bool IsEssence { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用名
        /// </summary>
        public UserDto User { get; set; }
        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
