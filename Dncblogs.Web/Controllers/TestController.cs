using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dncblogs.Core;
using Microsoft.AspNetCore.Mvc;

namespace Dncblogs.Web.Controllers
{
    public class TestController : Controller
    {
        private UserService userService;
        public TestController(UserService  userService)
        {
            this.userService = userService;
        }

        public  IActionResult Index()
        {
            //var u = await this.userService.GetOneUserByUserIdAsync1(1);
            return  Content("访问成功");
        }
    }
}