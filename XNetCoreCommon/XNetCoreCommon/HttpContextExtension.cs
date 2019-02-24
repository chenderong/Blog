using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XNetCoreCommon
{
    public static class HttpContextExtension
    {

        /// <summary>
        /// 获取真实用户IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
    }
}
