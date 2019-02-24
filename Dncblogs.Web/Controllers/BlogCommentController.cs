using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dncblogs.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dncblogs.Web.Controllers
{
    //[Produces("application/json")]
    //[Route("api/BlogComment")]
    public class BlogCommentController : Controller
    {
        private BlogCommentService _blogCommentService;
        public BlogCommentController(BlogCommentService blogCommentService)
        {
            this._blogCommentService = blogCommentService;
        }
    }
}