using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Entities
{
    /// <summary>
    /// 建议
    /// </summary>
    public class Suggest
    {
        public int SuggestID { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string SuggestContact { get; set; }

        /// <summary>
        /// 联系内容
        /// </summary>
        public string SuggestContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
