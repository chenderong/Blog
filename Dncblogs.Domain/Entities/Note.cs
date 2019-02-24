using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Entities
{
    public class Note
    {
        public int NoteID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }


        public bool IsDelete { get; set; }
    }
}
