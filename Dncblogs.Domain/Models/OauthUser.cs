using Dncblogs.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
   public class OauthUser
    {
        /// <summary>
        /// 第三方 标识
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        public User User { get; set; }

        public int UserId { get; set; }
    }
}
