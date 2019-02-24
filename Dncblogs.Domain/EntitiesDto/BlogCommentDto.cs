using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class BlogCommentDto
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
        [Required]
        [MaxLength(50)]
        public string PostName { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }

        public string ContentAbs { get; set; }


        /// <summary>
        /// 评论时间
        /// </summary>
        public string PostDate { get; set; }

       
    }
}
