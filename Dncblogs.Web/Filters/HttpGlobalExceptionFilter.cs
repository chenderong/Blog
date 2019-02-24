using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dncblogs.Web.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {

        readonly ILoggerFactory _loggerFactory;
        readonly IHostingEnvironment _env;

        public HttpGlobalExceptionFilter(ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var logger = _loggerFactory.CreateLogger(context.Exception.TargetSite.ReflectedType);
            logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);


            //var json = new ErrorResponse("未知错误,请重试");
            //if (_env.IsDevelopment()) json.DeveloperMessage = context.Exception;

            //context.Result = new ApplicationErrorResult(json);
            //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.Redirect("/Home/Error");
            context.ExceptionHandled = true;
        }
    }
}
