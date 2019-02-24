using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
    public class DatabaseSetting
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DBType { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
