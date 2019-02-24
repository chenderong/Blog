using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    /// <summary>
    /// 建议
    /// </summary>
    public class SuggestDto
    {
        public int SuggestID { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        [MaxLength(100, ErrorMessage = "联系方式长度最大为100")]
        public string SuggestContact { get; set; }

        /// <summary>
        /// 联系内容
        /// </summary>
        [Required]
        [MaxLength(1000, ErrorMessage = "联系内容长度最大为1000")]
        public string SuggestContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
