using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Dncblogs.Web.CustomMiddleware
{
    public class UserRealIpMiddleware
    {
        private readonly RequestDelegate _next;
        public UserRealIpMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers;
            if (headers.ContainsKey("X-Forwarded-For"))
            {
                string[] ipArray = headers["X-Forwarded-For"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                context.Connection.RemoteIpAddress = IPAddress.Parse(ipArray[0]);
            }

            return _next(context);
        }
    }
}
