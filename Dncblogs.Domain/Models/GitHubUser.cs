using System;
using System.Collections.Generic;
using System.Text;

namespace Dncblogs.Domain.Models
{
    /// <summary>
    /// github 用户的基本信息
    /// </summary>
   public class GitHubUser
    {
       public string  login { get; set; }
       public long id { get; set; }
    }
}
