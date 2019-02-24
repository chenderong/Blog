using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
    /// <summary>
    /// 开源项目
    /// </summary>
    public class OpenSourceDto
    {
        public int OpenSourceID { get; set; }

        /// <summary>
        /// 开源项目类型
        /// </summary>
        public string OpenSourceType { get; set; }

        /// <summary>
        /// 开源项目标题
        /// </summary>
        public string OpenSourceTitle { get; set; }

        /// <summary>
        /// 开源项目描述
        /// </summary>
        public string OpenSourceDescribe { get; set; }

        /// <summary>
        /// 开源项目内容
        /// </summary>
        public string OpenSourceContent { get; set; }


        /// <summary>
        /// 0 普通 1 热门  2 编辑推荐
        /// </summary>
        public int HeatType { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }

        public bool IsDelete { get; set; }

    }
}
