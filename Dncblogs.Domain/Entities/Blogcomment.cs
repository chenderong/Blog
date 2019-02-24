using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Entities
{
  public  class BlogComment
    {
        public long BCId { get; set; }

        public long BlogId { get; set; }

        /// <summary>
        /// 引用评论ID
        /// </summary>
        public long ReferenceId { get; set; }

      /// <summary>
      /// 评论人id
      /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// 评论人名称
        /// </summary>
        public string PostName { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// 评论所属用户
        /// </summary>
        public User User { get; set; }


        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
