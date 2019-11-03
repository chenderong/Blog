using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dncblogs.Domain.EntitiesDto
{
  public  class BindingUserDto
    {

        /// 用户登陆用户名
        /// </summary>
        [MaxLength(50, ErrorMessage = "用户名不能为空")]
        [Required]
        public string LoginName { set; get; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; } = "";

        /// <summary>
        /// 用户密码 
        /// </summary>
        [MaxLength(50, ErrorMessage = "密码不能为空")]
        [Required]
        public string Password { set; get; }

        /// <summary>
        /// 0 有账号直接绑定  1 没账号注册账号绑定
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 第三方 登录标识
        /// </summary>
        public string LoginID { get; set; }
    }
}
