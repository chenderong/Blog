using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
   public class WebStaticConfig
    {
        /// <summary>
        /// 博客后台管理员Id
        /// </summary>
        public int BlogBackUserId { get; set; }

        /// <summary>
        /// 新闻分类id
        /// </summary>
        public int NewsCategoryId { get; set; }

        /// <summary>
        /// 编程杂谈分类id
        /// </summary>
        public int PgossipCategoryId { get; set; }

        /// <summary>
        /// MD5加密固定字符串
        /// </summary>
        public string MD5Code { get; set; }

        /// <summary>
        /// 最大上传图片
        /// </summary>
        public int ImgMaxSize { get; set; }

        /// <summary>
        /// 网站名称
        /// </summary>
        public string WebName { get; set; }

        /// <summary>
        /// 网站关键字
        /// </summary>
        public string WebKeyWord { get; set; }

        
        /// <summary>
        /// 描述
        /// </summary>
        public string WebDescription { get; set; }


        /// <summary>
        /// 登陆界面文字标题
        /// </summary>
        public string LoginPageTitle { get; set; }

        /// <summary>
        /// 登陆界面文字内容
        /// </summary>
        public string LoginPageDesc { get; set; }
    }
}
