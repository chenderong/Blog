using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XNetCoreCommon;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Dncblogs.Web.Controllers
{
    public class ValidateCodeController : Controller
    {
        readonly ILogger logger;
        public ValidateCodeController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(typeof(ValidateCodeController));
        }

        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <returns></returns>
        public IActionResult ValidateCode()
        {
            string code = VierificationCodeTool.RndNum(4);
            MemoryStream ms = VierificationCodeTool.Create(code);
            HttpContext.Session.SetString("LoginValidateCode", code);
            logger.LogDebug($"zhongzw sessionid:{HttpContext.Session.Id},{code}");
            return File(ms.ToArray(), @"image/png");
        }
    }
}