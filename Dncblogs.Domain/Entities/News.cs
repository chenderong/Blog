using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Entities
{
    public class News
    {
        /// <summary>
        /// 新闻ID 
        /// </summary>
        public int NewsId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        ///新闻名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 新闻内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 访问人数
        /// </summary>
        public int VisitCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentCount { get; set; }



        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }


        public string OriginalUrl { get; set; }



        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
