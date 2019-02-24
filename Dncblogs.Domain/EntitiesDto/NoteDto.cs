using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    public class NoteDto
    {
        public int NoteID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }


        public bool IsDelete { get; set; }
    }
}
