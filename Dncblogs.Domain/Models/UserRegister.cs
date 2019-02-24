using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
   public class UserRegister
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 注册次数
        /// </summary>
        public int RegisterCount { get; set; }

        /// <summary>
        /// 最新注册的时间
        /// </summary>
        public DateTime RegisterTime { get; set; }
    }
}
