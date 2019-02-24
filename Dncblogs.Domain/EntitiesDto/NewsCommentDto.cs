using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class NewsCommentDto
    {
        public long NId { get; set; }

        public long NewsId { get; set; }

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
        public string Content { get; set; }


        public string ContentAbs { get; set; }
        
        /// <summary>
        /// 评论时间
        /// </summary>
        public string PostDate { get; set; }

        /// <summary>
        /// 评论所属用户
        /// </summary>
        public UserDto UserDto { get; set; }


        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
