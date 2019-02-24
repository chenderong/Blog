using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Enums
{
    public enum UserStatus : int
    {
        /// <summary>
        /// 启用
        /// </summary>
        Active = 0,

        /// <summary>
        /// 审核中
        /// </summary>
        Check = 1,

        /// <summary>
        /// 冻结
        /// </summary>
        Freeze = 2,

        /// <summary>
        /// 禁用
        /// </summary>
        Forbidden = 3

    }
}
