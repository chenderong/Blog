using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class UserDto
    {

        /// <summary>
        /// 主键自增
        /// </summary>
        public int UserId { set; get; }

        /// <summary>
        /// 用户登陆用户名
        /// </summary>
        [MaxLength(50, ErrorMessage = "用户名不能为空")]
        [Required]
        public string LoginName { set; get; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; } = "";

        /// <summary>
        /// 头像地址
        /// </summary>
        public string UserHeadImaUrl { get; set; } = "";

        /// <summary>
        /// 用户密码 
        /// </summary>
        [MaxLength(50, ErrorMessage = "密码不能为空")]
        [Required]
        public string Password { set; get; }

        /// <summary>
        /// 博客标题
        /// </summary>
        public string BlogTitle { get; set; }

        /// <summary>
        /// 博客名称
        /// </summary>
        public string BlogName { get; set; }

        /// <summary>
        /// 博客描述
        /// </summary>
        public string BlogDesc { get; set; }


        /// <summary>
        /// 公告信息
        /// </summary>
        public string BlogNotice { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { set; get; }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; } = false;

        /// <summary>
        /// 用户状态 
        /// </summary>
        public int Status { get; set; } = 0;

        private List<CategoryDto> categorys = new List<CategoryDto>();

        /// <summary>
        /// 分类列表
        /// </summary>
        public List<CategoryDto> Categorys
        {
            get { return categorys; }
            set { categorys = value; }
        }

        //[Required]
        public string ChcekCode { get; set; }
    }
}
